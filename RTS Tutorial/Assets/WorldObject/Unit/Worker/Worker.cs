using UnityEngine;
using Newtonsoft.Json;
using System.Collections.Generic;
using RTS;

public class Worker : Unit {
	
	public int buildSpeed;
	public AudioClip finishedJobSound;
	public float finishedJobVolume = 1.0f;
	
	private Building currentProject;
	private bool building = false;
	private float amountBuilt = 0.0f;
	private int loadedProjectId = -1;
	
	/*** Game Engine methods, all can be overridden by subclass ***/
	
	protected override void Start () {
		base.Start();
		actions = new string[] {"Refinery", "WarFactory", "Wonder", "Turret"};
		if(player && loadedSavedValues && loadedProjectId >= 0) {
			WorldObject obj = player.GetObjectForId(loadedProjectId);
			if(obj.GetType().IsSubclassOf(typeof(Building))) currentProject = (Building)obj;
		}
	}
	
	protected override void InitialiseAudio () {
		base.InitialiseAudio ();
		if(finishedJobVolume < 0.0f) finishedJobVolume = 0.0f;
		if(finishedJobVolume > 1.0f) finishedJobVolume = 1.0f;
		List<AudioClip> sounds = new List<AudioClip>();
		List<float> volumes = new List<float>();
		sounds.Add(finishedJobSound);
		volumes.Add (finishedJobVolume);
		audioElement.Add(sounds, volumes);
	}
	
	protected override void Update () {
		base.Update();
		if(!moving && !rotating) {
			if(building && currentProject && currentProject.UnderConstruction()) {
				amountBuilt += buildSpeed * Time.deltaTime;
				int amount = Mathf.FloorToInt(amountBuilt);
				if(amount > 0) {
					amountBuilt -= amount;
					currentProject.Construct(amount);
					if(!currentProject.UnderConstruction()) {
						building = false;
						if(audioElement != null) audioElement.Play(finishedJobSound);
					}
				}
			}
		}
	}
	
	/*** Public Methods ***/
	
	public override void SetBuilding (Building project) {
		base.SetBuilding (project);
		currentProject = project;
		StartMove(currentProject.transform.position, currentProject.gameObject);
		building = true;
	}
	
	public override void PerformAction (string actionToPerform) {
		base.PerformAction (actionToPerform);
		CreateBuilding(actionToPerform);
	}
	
	public override void StartMove(Vector3 destination) {
		base.StartMove(destination);
		amountBuilt = 0.0f;
		building = false;
	}
	
	public override void MouseClick (GameObject hitObject, Vector3 hitPoint, Player controller) {
		bool doBase = true;
		//only handle input if owned by a human player and currently selected
		if(player && player.human && currentlySelected && !WorkManager.ObjectIsGround(hitObject)) {
			Building building = hitObject.transform.parent.GetComponent<Building>();
			if(building) {
				if(building.UnderConstruction()) {
					SetBuilding(building);
					doBase = false;
				}
			}
		}
		if(doBase) base.MouseClick(hitObject, hitPoint, controller);
	}
	
	public override void SaveDetails (JsonWriter writer) {
		base.SaveDetails (writer);
		SaveManager.WriteBoolean(writer, "Building", building);
		SaveManager.WriteFloat(writer, "AmountBuilt", amountBuilt);
		if(currentProject) SaveManager.WriteInt(writer, "CurrentProjectId", currentProject.ObjectId);
	}
	
	protected override void HandleLoadedProperty (JsonTextReader reader, string propertyName, object readValue) {
		base.HandleLoadedProperty (reader, propertyName, readValue);
		switch(propertyName) {
			case "Building": building = (bool)readValue; break;
			case "AmountBuilt": amountBuilt = (float)(double)readValue; break;
			case "CurrentProjectId": loadedProjectId = (int)(System.Int64)readValue; break;
			default: break;
		}
	}
	
	protected override bool ShouldMakeDecision () {
		if(building) return false;
		return base.ShouldMakeDecision();
	}
	
	protected override void DecideWhatToDo () {
		base.DecideWhatToDo ();
		List<WorldObject> buildings = new List<WorldObject>();
		foreach(WorldObject nearbyObject in nearbyObjects) {
			if(nearbyObject.GetPlayer() != player) continue;
			Building nearbyBuilding = nearbyObject.GetComponent<Building>();
			if(nearbyBuilding && nearbyBuilding.UnderConstruction()) buildings.Add(nearbyObject);
		}
		WorldObject nearestObject = WorkManager.FindNearestWorldObjectInListToPosition(buildings, transform.position);
		if(nearestObject) {
			Building closestBuilding = nearestObject.GetComponent<Building>();
			if(closestBuilding) SetBuilding(closestBuilding);
		}
	}
	
	/*** Private Methods ***/
	
	private void CreateBuilding(string buildingName) {
		Vector3 buildPoint = new Vector3(transform.position.x, transform.position.y, transform.position.z + 10);
		if(player) player.CreateBuilding(buildingName, buildPoint, this, playingArea);
	}
}
