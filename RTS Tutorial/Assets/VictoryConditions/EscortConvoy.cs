using UnityEngine;
using System.Collections;

public class EscortConvoy : VictoryCondition {
	
	public Vector3 destination = new Vector3(0.0f, 0.0f, 0.0f);
	public Texture2D highlight;
	
	void Start() {
		GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
		cube.name = "Ground";
		cube.transform.localScale = new Vector3(3, 0.01f, 3);
		cube.transform.position = new Vector3(destination.x, 0.005f, destination.z);
		if(highlight) cube.renderer.material.mainTexture = highlight;
		cube.transform.parent = this.transform;
	}
	
	public override string GetDescription () {
		return "Escort Convoy Truck";
	}
	
	public override bool PlayerMeetsConditions (Player player) {
		ConvoyTruck truck = player.GetComponentInChildren<ConvoyTruck>();
		return player && !player.IsDead() && TruckInPosition(truck);
	}
	
	private bool TruckInPosition(ConvoyTruck truck) {
		if(!truck) return false;
		float closeEnough = 3.0f;
		Vector3 truckPos = truck.transform.position;
		bool xInPos = truckPos.x > destination.x - closeEnough && truckPos.x < destination.x + closeEnough;
		bool zInPos = truckPos.z > destination.z - closeEnough && truckPos.z < destination.z + closeEnough;
		return xInPos && zInPos;
	}
}