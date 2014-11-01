using UnityEngine;
using System.Collections;
using RTS;

/**
 * Singleton that handles the management of game state. This includes
 * detecting when a game has been finished and what to do from there.
 */

public class GameManager : MonoBehaviour {
	
	private static bool created = false;
	private bool initialised = false;
	private VictoryCondition[] victoryConditions;
	private HUD hud;
	
	void Awake() {
		if(!created) {
			DontDestroyOnLoad(transform.gameObject);
			created = true;
			initialised = true;
		} else {
			Destroy(this.gameObject);
		}
		if(initialised) {
			LoadDetails();
		}
	}
	
	void OnLevelWasLoaded() {
		if(initialised) {
			LoadDetails();
		}
	}
	
	private void LoadDetails() {
		Player[] players = GameObject.FindObjectsOfType(typeof(Player)) as Player[];
		foreach(Player player in players) {
			if(player.human) hud = player.GetComponentInChildren<HUD>();
		}
		victoryConditions = GameObject.FindObjectsOfType(typeof(VictoryCondition)) as VictoryCondition[];
		if(victoryConditions != null) {
			foreach(VictoryCondition victoryCondition in victoryConditions) {
				victoryCondition.SetPlayers(players);
			}
		}
	}
	
	void Update() {
		if(victoryConditions != null) {
			foreach(VictoryCondition victoryCondition in victoryConditions) {
				if(victoryCondition.GameFinished()) {
					ResultsScreen resultsScreen = hud.GetComponent<ResultsScreen>();
					resultsScreen.SetMetVictoryCondition(victoryCondition);
					resultsScreen.enabled = true;
					Time.timeScale = 0.0f;
					Screen.showCursor = true;
					ResourceManager.MenuOpen = true;
					hud.enabled = false;
				}
			}
		}
	}
	
}