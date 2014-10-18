using UnityEngine;
using System.Collections.Generic;
using RTS;

public class LoadMenu : MonoBehaviour {

	public GUISkin mainSkin, selectionSkin;
	public AudioClip clickSound;
	public float clickVolume = 1.0f;
	
	private AudioElement audioElement;

	void Start() {
		Activate();
		if(clickVolume < 0.0f) clickVolume = 0.0f;
		if(clickVolume > 1.0f) clickVolume = 1.0f;
		List<AudioClip> sounds = new List<AudioClip>();
		List<float> volumes = new List<float>();
		sounds.Add(clickSound);
		volumes.Add (clickVolume);
		audioElement = new AudioElement(sounds, volumes, "LoadMenu", null);
	}
	
	void Update() {
		if(Input.GetKeyDown(KeyCode.Escape)) CancelLoad();
	}
	
	void OnGUI() {
		if(SelectionList.MouseDoubleClick()) {
			PlayClick();
			StartLoad();
		}
		
		GUI.skin = mainSkin;
		float menuHeight = GetMenuHeight();
		float groupLeft = Screen.width / 2 - ResourceManager.MenuWidth / 2;
		float groupTop = Screen.height / 2 - menuHeight / 2;
		Rect groupRect = new Rect(groupLeft, groupTop, ResourceManager.MenuWidth, menuHeight);
		GUI.BeginGroup(groupRect);
		//background box
		GUI.Box(new Rect(0, 0, ResourceManager.MenuWidth, menuHeight), "");
		//menu buttons
		float leftPos = ResourceManager.Padding;
		float topPos = menuHeight - ResourceManager.Padding - ResourceManager.ButtonHeight;
		if(GUI.Button(new Rect(leftPos, topPos, ResourceManager.ButtonWidth, ResourceManager.ButtonHeight), "Load Game")) {
			PlayClick();
			StartLoad();
		}
		leftPos += ResourceManager.ButtonWidth + ResourceManager.Padding;
		if(GUI.Button(new Rect(leftPos, topPos, ResourceManager.ButtonWidth, ResourceManager.ButtonHeight), "Cancel")) {
			PlayClick();
			CancelLoad();
		}
		GUI.EndGroup();
		
		//selection list, needs to be called outside of the group for the menu
		float selectionLeft = groupRect.x + ResourceManager.Padding;
		float selectionTop = groupRect.y + ResourceManager.Padding;
		float selectionWidth = groupRect.width - 2 * ResourceManager.Padding;
		float selectionHeight = groupRect.height - GetMenuItemsHeight() - ResourceManager.Padding;
		SelectionList.Draw(selectionLeft,selectionTop,selectionWidth,selectionHeight,selectionSkin);
	}
	
	private void PlayClick() {
		if(audioElement != null) audioElement.Play(clickSound);
	}
	
	private float GetMenuHeight() {
		return 250 + GetMenuItemsHeight();
	}
	
	private float GetMenuItemsHeight() {
		return ResourceManager.ButtonHeight + 2 * ResourceManager.Padding;
	}
	
	public void Activate() {
		SelectionList.LoadEntries(PlayerManager.GetSavedGames());
	}
	
	private void StartLoad() {
		string newLevel = SelectionList.GetCurrentEntry();
		if(newLevel!="") {
			ResourceManager.LevelName = newLevel;
			if(Application.loadedLevelName != "BlankMap1") Application.LoadLevel("BlankMap1");
			else if(Application.loadedLevelName != "BlankMap2") Application.LoadLevel("BlankMap2");
			//makes sure that the loaded level runs at normal speed
			Time.timeScale = 1.0f;
		}
	}
	
	private void CancelLoad() {
		GetComponent<LoadMenu>().enabled = false;
		PauseMenu pause = GetComponent<PauseMenu>();
		if(pause) pause.enabled = true;
		else {
			MainMenu main = GetComponent<MainMenu>();
			if(main) main.enabled = true;
		}
	}

}