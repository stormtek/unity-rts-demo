using UnityEngine;
using System.Collections.Generic;
using RTS;

public class Building : WorldObject {
	
	public float maxBuildProgress;
	
	protected Queue<string> buildQueue;
	
	private float currentBuildProgress = 0.0f;
	private Vector3 spawnPoint;
	
	/*** Game Engine methods, all can be overridden by subclass ***/
	
	protected override void Awake() {
		base.Awake();
		buildQueue = new Queue<string>();
		float spawnX = selectionBounds.center.x + transform.forward.x * selectionBounds.extents.x + transform.forward.x * 10;
		float spawnZ = selectionBounds.center.z + transform.forward.z + selectionBounds.extents.z + transform.forward.z * 10;
		spawnPoint = new Vector3(spawnX, 0.0f, spawnZ);
	}
	
	protected override void Start () {
		base.Start();
	}
	
	protected override void Update () {
		base.Update();
		ProcessBuildQueue();
	}
	
	protected override void OnGUI() {
		base.OnGUI();
	}
	
	/*** Internal worker methods that can be accessed by subclass ***/
	
	protected void CreateUnit(string unitName) {
		buildQueue.Enqueue(unitName);
	}
	
	protected void ProcessBuildQueue() {
		if(buildQueue.Count > 0) {
			currentBuildProgress += Time.deltaTime * ResourceManager.BuildSpeed;
			if(currentBuildProgress > maxBuildProgress) {
				if(player) player.AddUnit(buildQueue.Dequeue(), spawnPoint, transform.rotation);
				currentBuildProgress = 0.0f;
			}
		}
	}
	
	/*** Public methods ***/
	
	public string[] getBuildQueueValues() {
		string[] values = new string[buildQueue.Count];
		int pos=0;
		foreach(string unit in buildQueue) values[pos++] = unit;
		return values;
	}
	
	public float getBuildPercentage() {
		return currentBuildProgress / maxBuildProgress;
	}
}
