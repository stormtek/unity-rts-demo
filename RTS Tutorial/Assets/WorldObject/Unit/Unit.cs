using UnityEngine;

public class Unit : WorldObject {

	/*** Game Engine methods, all can be overridden by subclass ***/
	
	protected override void Awake() {
		base.Awake();
	}
	
	protected override void Start () {
		base.Start();
	}
	
	protected override void Update () {
		base.Update();
	}
	
	protected override void OnGUI() {
		base.OnGUI();
	}
}
