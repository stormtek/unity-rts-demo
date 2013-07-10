using UnityEngine;
using System.Collections.Generic;
using RTS;

public class Player : MonoBehaviour {
	
	public string username;
	public bool human;
	public HUD hud;
	public WorldObject SelectedObject { get; set; }
	public int startMoney, startMoneyLimit, startPower, startPowerLimit;
	public Material notAllowedMaterial, allowedMaterial;
	
	private Dictionary<ResourceType,int> resources, resourceLimits;
	private Building tempBuilding;
	private Unit tempCreator;
	private bool findingPlacement = false;
	
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
			if(findingPlacement) {
				tempBuilding.CalculateBounds();
				if(CanPlaceBuilding()) tempBuilding.SetTransparentMaterial(allowedMaterial, false);
				else tempBuilding.SetTransparentMaterial(notAllowedMaterial, false);
			}
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
	
	public void AddUnit(string unitName, Vector3 spawnPoint, Vector3 rallyPoint, Quaternion rotation, Building creator) {
		Units units = GetComponentInChildren<Units>();
		GameObject newUnit = (GameObject)Instantiate(ResourceManager.GetUnit(unitName),spawnPoint, rotation);
		newUnit.transform.parent = units.transform;
		Unit unitObject = newUnit.GetComponent<Unit>();
		if (unitObject) {
			unitObject.SetBuilding(creator);
			if(spawnPoint != rallyPoint) unitObject.StartMove(rallyPoint);
		} else Destroy(newUnit);
	}
	
	public void CreateBuilding(string buildingName, Vector3 buildPoint, Unit creator, Rect playingArea) {
		GameObject newBuilding = (GameObject)Instantiate(ResourceManager.GetBuilding(buildingName), buildPoint, new Quaternion());
		tempBuilding = newBuilding.GetComponent<Building>();
		if (tempBuilding) {
			tempCreator = creator;
			findingPlacement = true;
			tempBuilding.SetTransparentMaterial(notAllowedMaterial, true);
			tempBuilding.SetColliders(false);
			tempBuilding.SetPlayingArea(playingArea);
		} else Destroy(newBuilding);
	}
	
	public bool IsFindingBuildingLocation() {
		return findingPlacement;
	}
	
	public void FindBuildingLocation() {
		Vector3 newLocation = WorkManager.FindHitPoint(Input.mousePosition);
		newLocation.y = 0;
		tempBuilding.transform.position = newLocation;
	}
	
	public bool CanPlaceBuilding() {
		bool canPlace = true;
		
		Bounds placeBounds = tempBuilding.GetSelectionBounds();
		//shorthand for the coordinates of the center of the selection bounds
		float cx = placeBounds.center.x;
		float cy = placeBounds.center.y;
		float cz = placeBounds.center.z;
		//shorthand for the coordinates of the extents of the selection box
		float ex = placeBounds.extents.x;
		float ey = placeBounds.extents.y;
		float ez = placeBounds.extents.z;
		
		//Determine the screen coordinates for the corners of the selection bounds
		List<Vector3> corners = new List<Vector3>();
		corners.Add(Camera.mainCamera.WorldToScreenPoint(new Vector3(cx+ex,cy+ey,cz+ez)));
		corners.Add(Camera.mainCamera.WorldToScreenPoint(new Vector3(cx+ex,cy+ey,cz-ez)));
		corners.Add(Camera.mainCamera.WorldToScreenPoint(new Vector3(cx+ex,cy-ey,cz+ez)));
		corners.Add(Camera.mainCamera.WorldToScreenPoint(new Vector3(cx-ex,cy+ey,cz+ez)));
		corners.Add(Camera.mainCamera.WorldToScreenPoint(new Vector3(cx+ex,cy-ey,cz-ez)));
		corners.Add(Camera.mainCamera.WorldToScreenPoint(new Vector3(cx-ex,cy-ey,cz+ez)));
		corners.Add(Camera.mainCamera.WorldToScreenPoint(new Vector3(cx-ex,cy+ey,cz-ez)));
		corners.Add(Camera.mainCamera.WorldToScreenPoint(new Vector3(cx-ex,cy-ey,cz-ez)));
		
		foreach(Vector3 corner in corners) {
			GameObject hitObject = WorkManager.FindHitObject(corner);
			if(hitObject && hitObject.name != "Ground") {
				WorldObject worldObject = hitObject.transform.parent.GetComponent<WorldObject>();
				if(worldObject && placeBounds.Intersects(worldObject.GetSelectionBounds())) canPlace = false;
			}
		}
		return canPlace;
	}
	
	public void StartConstruction() {
		findingPlacement = false;
		Buildings buildings = GetComponentInChildren<Buildings>();
		if(buildings) tempBuilding.transform.parent = buildings.transform;
		tempBuilding.SetPlayer();
		tempBuilding.SetColliders(true);
		tempCreator.SetBuilding(tempBuilding);
		tempBuilding.StartConstruction();
	}
	
	public void CancelBuildingPlacement() {
		findingPlacement = false;
		Destroy(tempBuilding.gameObject);
		tempBuilding = null;
		tempCreator = null;
	}
}