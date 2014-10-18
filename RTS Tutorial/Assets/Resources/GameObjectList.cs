using UnityEngine;
using RTS;

/**
 * Singleton that keeps track of all the game objects that can be created
 * in the world. This persists across all maps to allow us to readily create
 * prefabs while loading from a saved file or when constructing new Units and
 * Buildings.
 */

public class GameObjectList : MonoBehaviour {
	
	public GameObject[] buildings;
	public GameObject[] units;
	public GameObject[] worldObjects;
	public GameObject player;
	public Texture2D[] avatars;
	
	private static bool created = false;
	
	void Awake() {
		if(!created) {
			DontDestroyOnLoad(transform.gameObject);
			ResourceManager.SetGameObjectList(this);
			PlayerManager.Load();
			PlayerManager.SetAvatarTextures(avatars);
			created = true;
		} else {
			Destroy(this.gameObject);
		}
	}
	
	public GameObject GetBuilding(string name) {
		for(int i=0; i<buildings.Length; i++) {
			Building building = buildings[i].GetComponent<Building>();
			if(building && building.name == name) return buildings[i];
		}
		return null;
	}
	
	public GameObject GetUnit(string name) {
		for(int i=0; i<units.Length; i++) {
			Unit unit = units[i].GetComponent<Unit>();
			if(unit && unit.name == name) return units[i];
		}
		return null;
	}
	
	public GameObject GetWorldObject(string name) {
		foreach(GameObject worldObject in worldObjects) {
			if(worldObject.name == name) return worldObject;
		}
		return null;
	}
	
	public GameObject GetPlayerObject() {
		return player;
	}
	
	public Texture2D GetBuildImage(string name) {
		for(int i=0; i<buildings.Length; i++) {
			Building building = buildings[i].GetComponent<Building>();
			if(building && building.name == name) return building.buildImage;
		}
		for(int i=0; i<units.Length; i++) {
			Unit unit = units[i].GetComponent<Unit>();
			if(unit && unit.name == name) return unit.buildImage;
		}
		return null;
	}
	
	public Texture2D[] GetAvatars() {
		return avatars;
	}
}
