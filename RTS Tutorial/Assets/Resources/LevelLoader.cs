using UnityEngine;
using RTS;

/**
 * Singleton that handles loading level details. This includes making sure
 * that all world objects have an objectId set.
 */

public class LevelLoader : MonoBehaviour {
	
	private static int nextObjectId = 0;
	private static bool created = false;
	private bool initialised = false;
	
	void Awake() {
		if(!created) {
			DontDestroyOnLoad(transform.gameObject);
			created = true;
			initialised = true;
		} else {
			Destroy(this.gameObject);
		}
		if(initialised) {
			SelectPlayerMenu menu = GameObject.FindObjectOfType(typeof(SelectPlayerMenu)) as SelectPlayerMenu;
			if(!menu) {
				//we have started from inside a map, rather than the main menu
				//this happens if we launch Unity from inside a map file for testing
				Player[] players = GameObject.FindObjectsOfType(typeof(Player)) as Player[];
				foreach(Player player in players) {
					if(player.human) {
						PlayerManager.SelectPlayer(player.username, 0);
					}
				}
				SetObjectIds();
			}
		}
	}
	
	void OnLevelWasLoaded() {
		if(initialised) {
			if(ResourceManager.LevelName != null && ResourceManager.LevelName != "") {
				LoadManager.LoadGame(ResourceManager.LevelName);
			} else {
				SetObjectIds();
			}
			Time.timeScale = 1.0f;
			ResourceManager.MenuOpen = false;
		}
	}
	
	public int GetNewObjectId() {
		nextObjectId++;
		if(nextObjectId >= int.MaxValue) nextObjectId = 0;
		return nextObjectId;
	}
	
	private void SetObjectIds() {
		WorldObject[] worldObjects = GameObject.FindObjectsOfType(typeof(WorldObject)) as WorldObject[];
		foreach(WorldObject worldObject in worldObjects) {
			worldObject.ObjectId = nextObjectId++;
			if(nextObjectId >= int.MaxValue) nextObjectId = 0;
		}
	}
}
