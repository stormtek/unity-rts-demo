using UnityEngine;
using RTS;

public class Resource : WorldObject {
	
	//Public variables
	public float capacity;
	
	//Variables accessible by subclass
	protected float amountLeft;
	protected ResourceType resourceType;
	
	/*** Game Engine methods, all can be overridden by subclass ***/
	
	protected override void Start () {
		base.Start();
		amountLeft = capacity;
		resourceType = ResourceType.Unknown;
	}
	
	/*** Public methods ***/
	
	public void Remove(float amount) {
		amountLeft -= amount;
		if(amountLeft < 0) amountLeft = 0;
	}
	
	public bool isEmpty() {
		return amountLeft <= 0;
	}
	
	public ResourceType GetResourceType() {
		return resourceType;
	}
	
	protected override void CalculateCurrentHealth () {
		healthPercentage = amountLeft / capacity;
		healthStyle.normal.background = ResourceManager.GetResourceHealthBar(resourceType);
	}
}