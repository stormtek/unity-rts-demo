using UnityEngine;

public class WorldObject : MonoBehaviour {
	
	//Public variables
	public string objectName;
	public Texture2D buildImage;
	public int cost, sellValue, hitPoints, maxHitPoints;
	
	//Variables accessible by subclass
	protected Player player;
	protected string[] actions = {};
	protected bool currentlySelected = false;
	
	/*** Game Engine methods, all can be overridden by subclass ***/
	
	protected virtual void Awake() {
		
	}
	
	protected virtual void Start () {
		player = transform.root.GetComponentInChildren<Player>();
	}

	protected virtual void Update () {
	
	}
	
	protected virtual void OnGUI() {
		
	}

	/*** Public methods ***/
	
	public void SetSelection(bool selected) {
		currentlySelected = selected;
	}
	
	public string[] GetActions() {
		return actions;
	}
	
	public virtual void PerformAction(string actionToPerform) {
		//it is up to children with specific actions to determine what to do with each of those actions
	}
}
