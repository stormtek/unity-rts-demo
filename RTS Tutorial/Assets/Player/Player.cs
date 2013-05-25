using UnityEngine;
using System.Collections.Generic;
using RTS;

public class Player : MonoBehaviour {
	
	public string username;
	public bool human;
	public HUD hud;
	public WorldObject SelectedObject { get; set; }
	public int startMoney, startMoneyLimit, startPower, startPowerLimit;
	
	private Dictionary<ResourceType,int> resources, resourceLimits;
	
	/*** Game Engine Methods ***/
	
	void Awake() {
		resources = InitResourceList();
		resourceLimits = InitResourceList();
	}
	
	void Start () {
		hud = GetComponentInChildren<HUD>();
		AddStartResourceLimits();
		AddStartResources();
	}
	
	void Update () {
		if(human) {
			hud.SetResourceValues(resources, resourceLimits);
		}
	}
	
	/*** Private Worker Methods ***/
	
	private Dictionary<ResourceType, int> InitResourceList() {
		Dictionary<ResourceType, int> list = new Dictionary<ResourceType, int>();
		list.Add(ResourceType.Money, 0);
		list.Add(ResourceType.Power, 0);
		return list;
	}
	
	private void AddStartResourceLimits() {
		IncrementResourceLimit(ResourceType.Money, startMoneyLimit);
		IncrementResourceLimit(ResourceType.Power, startPowerLimit);
	}
	
	private void AddStartResources() {
		AddResource(ResourceType.Money, startMoney);
		AddResource(ResourceType.Power, startPower);
	}
	
	/*** Public Methods ***/
	
	public void AddResource(ResourceType type, int amount) {
		resources[type] += amount;
	}
	
	public void IncrementResourceLimit(ResourceType type, int amount) {
		resourceLimits[type] += amount;
	}
	
	public void AddUnit(string unitName, Vector3 spawnPoint, Quaternion rotation) {
		Units units = GetComponentInChildren<Units>();
		GameObject newUnit = (GameObject)Instantiate(ResourceManager.GetUnit(unitName),spawnPoint, rotation);
		newUnit.transform.parent = units.transform;
	}
}