using UnityEngine;
using RTS;

public class Harvester : Unit {
	
	public float capacity, collectionAmount, depositAmount;
	public Building resourceStore;
	
	private bool harvesting = false, emptying = false;
	private float currentLoad = 0.0f, currentDeposit = 0.0f;
	private ResourceType harvestType;
	private Resource resourceDeposit;
	
	/*** Game Engine methods, all can be overridden by subclass ***/
	
	protected override void Start () {
		base.Start();
		harvestType = ResourceType.Unknown;
	}
	
	protected override void Update () {
		base.Update();
		if(!rotating && !moving) {
			if(harvesting || emptying) {
				Arms[] arms = GetComponentsInChildren<Arms>();
				foreach(Arms arm in arms) arm.renderer.enabled = true;
				if(harvesting) {
					Collect();
					if(currentLoad >= capacity || resourceDeposit.isEmpty()) {
						//make sure that we have a whole number to avoid bugs
						//caused by floating point numbers
						currentLoad = Mathf.Floor(currentLoad);
						harvesting = false;
						emptying = true;
						foreach(Arms arm in arms) arm.renderer.enabled = false;
						StartMove (resourceStore.transform.position, resourceStore.gameObject);
					}
				} else {
					Deposit();
					if(currentLoad <= 0) {
						emptying = false;
						foreach(Arms arm in arms) arm.renderer.enabled = false;
						if(!resourceDeposit.isEmpty()) {
							harvesting = true;
							StartMove (resourceDeposit.transform.position, resourceDeposit.gameObject);
						}
					}
				}
			}
		}
	}
	
	/* Public Methods */
	
	public override void Init (Building creator) {
		base.Init (creator);
		resourceStore = creator;
	}
	
	public override void SetHoverState(GameObject hoverObject) {
		base.SetHoverState(hoverObject);
		//only handle input if owned by a human player and currently selected
		if(player && player.human && currentlySelected) {
			if(hoverObject.name != "Ground") {
				Resource resource = hoverObject.transform.parent.GetComponent<Resource>();
				if(resource && !resource.isEmpty()) player.hud.SetCursorState(CursorState.Harvest);
			}
		}
	}
	
	public override void MouseClick(GameObject hitObject, Vector3 hitPoint, Player controller) {
		base.MouseClick(hitObject, hitPoint, controller);
		//only handle input if owned by a human player
		if(player && player.human) {
			if(hitObject.name != "Ground") {
				Resource resource = hitObject.transform.parent.GetComponent<Resource>();
				if(resource && !resource.isEmpty()) {
					//make sure that we select harvester remains selected
					if(player.SelectedObject) player.SelectedObject.SetSelection(false, playingArea);
					SetSelection(true, playingArea);
					player.SelectedObject = this;
					StartHarvest(resource);
				}
			} else StopHarvest();
		}
	}
	
	/* Private Methods */
	
	private void StartHarvest(Resource resource) {
		resourceDeposit = resource;
		StartMove(resource.transform.position, resource.gameObject);
		//we can only collect one resource at a time, other resources are lost
		if(harvestType == ResourceType.Unknown || harvestType != resource.GetResourceType()) {
			harvestType = resource.GetResourceType();
			currentLoad = 0.0f;
		}
		harvesting = true;
		emptying = false;
	}
	
	private void StopHarvest() {
		
	}
	
	private void Collect() {
		float collect = collectionAmount * Time.deltaTime;
		//make sure that the harvester cannot collect more than it can carry
		if(currentLoad + collect > capacity) collect = capacity - currentLoad;
		resourceDeposit.Remove(collect);
		currentLoad += collect;
	}
	
	private void Deposit() {
		currentDeposit += depositAmount * Time.deltaTime;
		int deposit = Mathf.FloorToInt(currentDeposit);
		if(deposit >= 1) {
			if(deposit > currentLoad) deposit = Mathf.FloorToInt(currentLoad);
			currentDeposit -= deposit;
			currentLoad -= deposit;
			ResourceType depositType = harvestType;
			if(harvestType == ResourceType.Ore) depositType = ResourceType.Money;
			player.AddResource(depositType, deposit);
		}
	}
	
	protected override void DrawSelectionBox (Rect selectBox) {
		base.DrawSelectionBox(selectBox);
		float percentFull = currentLoad / capacity;
		float maxHeight = selectBox.height - 4;
		float height = maxHeight * percentFull;
		float leftPos = selectBox.x + selectBox.width - 7;
		float topPos = selectBox.y + 2 + (maxHeight - height);
		float width = 5;
		Texture2D resourceBar = ResourceManager.GetResourceHealthBar(harvestType);
		if(resourceBar) GUI.DrawTexture(new Rect(leftPos, topPos, width, height), resourceBar);
	}
}