using UnityEngine;
using RTS;
using System.Collections.Generic;

public class WorldObject : MonoBehaviour {
	
	//Public variables
	public string objectName = "WorldObject";
	public Texture2D buildImage;
	public int cost = 100, sellValue = 10, hitPoints = 100, maxHitPoints = 100;
	public float weaponRange = 10.0f, weaponRechargeTime = 1.0f, weaponAimSpeed = 1.0f;
	
	//Variables accessible by subclass
	protected Player player;
	protected string[] actions = {};
	protected bool currentlySelected = false;
	protected Bounds selectionBounds;
	protected Rect playingArea = new Rect(0.0f, 0.0f, 0.0f, 0.0f);
	protected GUIStyle healthStyle = new GUIStyle();
	protected float healthPercentage = 1.0f;
	protected WorldObject target = null;
	protected bool attacking = false, movingIntoPosition = false, aiming = false;
	
	//Private variables
	private List<Material> oldMaterials = new List<Material>();
	private float currentWeaponChargeTime;
	
	/*** Game Engine methods, all can be overridden by subclass ***/
	
	protected virtual void Awake() {
		selectionBounds = ResourceManager.InvalidBounds;
		CalculateBounds();
		currentWeaponChargeTime = weaponRechargeTime;
	}
	
	protected virtual void Start () {
		SetPlayer();
		if(player) SetTeamColor();
	}

	protected virtual void Update () {
		currentWeaponChargeTime += Time.deltaTime;
		if(attacking && !movingIntoPosition && !aiming) PerformAttack();
	}
	
	protected virtual void OnGUI() {
		if(currentlySelected) DrawSelection();
	}

	/*** Public methods ***/
	
	public void SetPlayer() {
		player = transform.root.GetComponentInChildren<Player>();
	}
	
	public virtual void SetSelection(bool selected, Rect playingArea) {
		currentlySelected = selected;
		if(selected) this.playingArea = playingArea;
	}
	
	public void SetPlayingArea(Rect playingArea) {
		this.playingArea = playingArea;
	}
	
	public string[] GetActions() {
		//should we be checking that the player who owns this is the one who asked for this???
		return actions;
	}
	
	public virtual void PerformAction(string actionToPerform) {
		//it is up to children with specific actions to determine what to do with each of those actions
	}
	
	public virtual void MouseClick(GameObject hitObject, Vector3 hitPoint, Player controller) {
		//only handle input if currently selected
		if(currentlySelected && hitObject && hitObject.name != "Ground") {
			WorldObject worldObject = hitObject.transform.parent.GetComponent<WorldObject>();
			//clicked on another selectable object
			if(worldObject) {
				Resource resource = hitObject.transform.parent.GetComponent<Resource>();
				if(resource && resource.isEmpty()) return;
				Player owner = hitObject.transform.root.GetComponent<Player>();
				if(owner) { //the object is controlled by a player
					if(player && player.human) { //this object is controlled by a human player
						//start attack if object is not owned by the same player and this object can attack, else select
						if(player.username != owner.username && CanAttack()) BeginAttack(worldObject);
						else ChangeSelection(worldObject, controller);
					} else ChangeSelection(worldObject, controller);
				} else ChangeSelection(worldObject, controller);
			}
		}
	}
	
	public virtual void SetHoverState(GameObject hoverObject) {
		//only handle input if owned by a human player and currently selected
		if(player && player.human && currentlySelected) {
			//something other than the ground is being hovered over
			if(hoverObject.name != "Ground") {
				Player owner = hoverObject.transform.root.GetComponent<Player>();
				Unit unit = hoverObject.transform.parent.GetComponent<Unit>();
				Building building = hoverObject.transform.parent.GetComponent<Building>();
				if(owner) { //the object is owned by a player
					if(owner.username == player.username) player.hud.SetCursorState(CursorState.Select);
					else if(CanAttack()) player.hud.SetCursorState(CursorState.Attack);
					else player.hud.SetCursorState(CursorState.Select);
				} else if(unit || building && CanAttack()) player.hud.SetCursorState(CursorState.Attack);
				else player.hud.SetCursorState(CursorState.Select);
			}
		}
	}
	
	public void CalculateBounds() {
		selectionBounds = new Bounds(transform.position, Vector3.zero);
		foreach(Renderer r in GetComponentsInChildren<Renderer>()) {
			selectionBounds.Encapsulate(r.bounds);
		}
	}
	
	public bool IsOwnedBy(Player owner) {
		if(player && player.Equals(owner)) {
			return true;
		} else {
			return false;
		}
	}
	
	public Bounds GetSelectionBounds() {
		return selectionBounds;
	}
	
	public void SetColliders(bool enabled) {
		Collider[] colliders = GetComponentsInChildren<Collider>();
		foreach(Collider collider in colliders) collider.enabled = enabled;
	}
	
	public void SetTransparentMaterial(Material material, bool storeExistingMaterial) {
		if(storeExistingMaterial) oldMaterials.Clear();
		Renderer[] renderers = GetComponentsInChildren<Renderer>();
		foreach(Renderer renderer in renderers) {
			if(storeExistingMaterial) oldMaterials.Add(renderer.material);
			renderer.material = material;
		}
	}
	
	public void RestoreMaterials() {
		Renderer[] renderers = GetComponentsInChildren<Renderer>();
		if(oldMaterials.Count == renderers.Length) {
			for(int i=0; i<renderers.Length; i++) {
				renderers[i].material = oldMaterials[i];
			}
		}
	}
	
	public virtual bool CanAttack() {
		//default behaviour needs to be overidden by children
		return false;
	}
	
	public virtual bool CanMove() {
		//default behaviour needs to be overidden by children
		return false;
	}
	
	public void TakeDamage(int damage) {
		hitPoints -= damage;
		if(hitPoints<=0) Destroy(gameObject);
	}
	
	/*** Private worker methods ***/
	
	private void ChangeSelection(WorldObject worldObject, Player controller) {
		//this should be called by the following line, but there is an outside chance it will not
		SetSelection(false, playingArea);
		if(controller.SelectedObject) controller.SelectedObject.SetSelection(false, playingArea);
		controller.SelectedObject = worldObject;
		worldObject.SetSelection(true, controller.hud.GetPlayingArea());
	}
	
	private void DrawSelection() {
		GUI.skin = ResourceManager.SelectBoxSkin;
		Rect selectBox = WorkManager.CalculateSelectionBox(selectionBounds, playingArea);
		//Draw the selection box around the currently selected object, within the bounds of the playing area
		GUI.BeginGroup(playingArea);
		DrawSelectionBox(selectBox);
		GUI.EndGroup();
	}
	
	private void BeginAttack(WorldObject target) {
		this.target = target;
		if(TargetInRange()) {
			attacking = true;
			PerformAttack();
		} else AdjustPosition();
	}
	
	private void PerformAttack() {
		if(!target) {
			attacking = false;
			return;
		}
		if(!TargetInRange()) AdjustPosition();
		else if(!TargetInFrontOfWeapon()) AimAtTarget();
		else if(ReadyToFire()) UseWeapon();
	}
	
	private bool TargetInRange() {
		Vector3 targetLocation = target.transform.position;
		Vector3 direction = targetLocation - transform.position;
		if(direction.sqrMagnitude < weaponRange * weaponRange) {
			return true;
		}
		return false;
	}
	
	private bool TargetInFrontOfWeapon() {
		Vector3 targetLocation = target.transform.position;
		Vector3 direction = targetLocation - transform.position;
		if(direction.normalized == transform.forward.normalized) return true;
		else return false;
	}
	
	private Vector3 FindNearestAttackPosition() {
		Vector3 targetLocation = target.transform.position;
		Vector3 direction = targetLocation - transform.position;
		float targetDistance = direction.magnitude;
		float distanceToTravel = targetDistance - (0.9f * weaponRange);
		return Vector3.Lerp(transform.position, targetLocation, distanceToTravel / targetDistance);
	}
	
	private void AdjustPosition() {
		Unit self = this as Unit;
		if(self) {
			movingIntoPosition = true;
			Vector3 attackPosition = FindNearestAttackPosition();
			self.StartMove(attackPosition);
			attacking = true;
		} else attacking = false;
	}
	
	private bool ReadyToFire() {
		if(currentWeaponChargeTime >= weaponRechargeTime) return true;
		return false;
	}
	
	/* Internal worker methods that can be accessed by subclass */
	
	protected virtual void DrawSelectionBox(Rect selectBox) {
		GUI.Box(selectBox, "");
		CalculateCurrentHealth(0.35f, 0.65f);
		DrawHealthBar(selectBox, "");
	}
	
	protected virtual void CalculateCurrentHealth(float lowSplit, float highSplit) {
		healthPercentage = (float)hitPoints / (float)maxHitPoints;
		if(healthPercentage > highSplit) healthStyle.normal.background = ResourceManager.HealthyTexture;
		else if(healthPercentage > lowSplit) healthStyle.normal.background = ResourceManager.DamagedTexture;
		else healthStyle.normal.background = ResourceManager.CriticalTexture;
	}
	
	protected void DrawHealthBar(Rect selectBox, string label) {
		healthStyle.padding.top = -20;
		healthStyle.fontStyle = FontStyle.Bold;
		GUI.Label(new Rect(selectBox.x, selectBox.y - 7, selectBox.width * healthPercentage, 5), label, healthStyle);
	}
	
	protected void SetTeamColor() {
		TeamColor[] teamColors = GetComponentsInChildren<TeamColor>();
		foreach(TeamColor teamColor in teamColors) teamColor.renderer.material.color = player.teamColor;
	}
	
	protected virtual void UseWeapon() {
		currentWeaponChargeTime = 0.0f;
		//this behaviour needs to be specified by a specific object
	}
	
	protected virtual void AimAtTarget() {
		aiming = true;
		//this behaviour needs to be specified by a specific object
	}
}
