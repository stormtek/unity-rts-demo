using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RTS;

public class Menu : MonoBehaviour {
	
	public GUISkin mySkin;
	public Texture2D header;
	public AudioClip clickSound;
	public float clickVolume = 1.0f;
	
	protected string[] buttons;
	
	private AudioElement audioElement;
	
	protected virtual void Start () {
		SetButtons();
		if(clickVolume < 0.0f) clickVolume = 0.0f;
		if(clickVolume > 1.0f) clickVolume = 1.0f;
		List<AudioClip> sounds = new List<AudioClip>();
		List<float> volumes = new List<float>();
		sounds.Add(clickSound);
		volumes.Add (clickVolume);
		audioElement = new AudioElement(sounds, volumes, "Menu", null);
	}
	
	protected virtual void OnGUI() {
		DrawMenu();
	}
	
	protected virtual void DrawMenu() {
		//default implementation for a menu consisting of a vertical list of buttons
		GUI.skin = mySkin;
		float menuHeight = GetMenuHeight();
		
		float groupLeft = Screen.width / 2 - ResourceManager.MenuWidth / 2;
		float groupTop = Screen.height / 2 - menuHeight / 2;
		GUI.BeginGroup(new Rect(groupLeft, groupTop, ResourceManager.MenuWidth, menuHeight));
		
		//background box
		GUI.Box(new Rect(0, 0, ResourceManager.MenuWidth, menuHeight), "");
		//header image
		GUI.DrawTexture(new Rect(ResourceManager.Padding, ResourceManager.Padding, ResourceManager.HeaderWidth, ResourceManager.HeaderHeight), header);
		
		//welcome message
		float leftPos = ResourceManager.Padding;
		float topPos = 2 * ResourceManager.Padding + header.height;
		GUI.Label(new Rect(leftPos, topPos, ResourceManager.MenuWidth - 2 * ResourceManager.Padding, ResourceManager.TextHeight), "Welcome " + PlayerManager.GetPlayerName());
		
		//menu buttons
		if(buttons != null) {
			leftPos = ResourceManager.MenuWidth / 2 - ResourceManager.ButtonWidth / 2;
			topPos += ResourceManager.TextHeight + ResourceManager.Padding;
			for(int i=0; i<buttons.Length; i++) {
				if(i > 0) topPos += ResourceManager.ButtonHeight + ResourceManager.Padding;
				if(GUI.Button(new Rect(leftPos, topPos, ResourceManager.ButtonWidth, ResourceManager.ButtonHeight), buttons[i])) {
					HandleButton(buttons[i]);
				}
			}
		}
		
		GUI.EndGroup();
	}
	
	protected virtual void SetButtons() {
		//a child class needs to set this for buttons to appear
	}
	
	protected virtual void HandleButton(string text) {
		if(audioElement != null) audioElement.Play(clickSound);
		//a child class needs to set this to handle button clicks
	}
	
	protected virtual float GetMenuHeight() {
		float buttonHeight = 0;
		if(buttons != null) buttonHeight = buttons.Length * ResourceManager.ButtonHeight;
		float paddingHeight = 2 * ResourceManager.Padding;
		if(buttons != null) paddingHeight += buttons.Length * ResourceManager.Padding;
		float messageHeight = ResourceManager.TextHeight + ResourceManager.Padding;
		return ResourceManager.HeaderHeight + buttonHeight + paddingHeight + messageHeight;
	}
	
	protected void LoadGame() {
		HideCurrentMenu();
		LoadMenu loadMenu = GetComponent<LoadMenu>();
		if(loadMenu) {
			loadMenu.enabled = true;
			loadMenu.Activate();
		}
	}
	
	protected virtual void HideCurrentMenu() {
		//a child class needs to set this to hide itself when appropriate
	}
	
	protected void ExitGame() {
		Application.Quit();
	}
}