using UnityEngine;
using System.Collections;
using RTS;

public class MainMenu : Menu {
	
	protected override void SetButtons () {
		buttons = new string[] {"New Game", "Quit Game"};
	}
	
	protected override void HandleButton (string text) {
		switch(text) {
			case "New Game": NewGame(); break;
			case "Quit Game": ExitGame(); break;
			default: break;
		}
	}
	
	private void NewGame() {
		ResourceManager.MenuOpen = false;
		Application.LoadLevel("Map");
		//makes sure that the loaded level runs at normal speed
		Time.timeScale = 1.0f;
	}
}