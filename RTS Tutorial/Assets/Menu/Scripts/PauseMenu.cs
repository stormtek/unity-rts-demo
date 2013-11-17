using UnityEngine;
using RTS;

public class PauseMenu : MonoBehaviour {
	
	public GUISkin mySkin;
	public Texture2D header;
	
	private Player player;
	private string[] buttons = {"Resume", "Exit Game"};
	
	void Start () {
		player = transform.root.GetComponent<Player>();
	}
	
	void Update () {
		if(Input.GetKeyDown(KeyCode.Escape)) Resume();
	}
	
	void OnGUI() {
		GUI.skin = mySkin;
		
		float groupLeft = Screen.width / 2 - ResourceManager.MenuWidth / 2;
		float groupTop = Screen.height / 2 - ResourceManager.PauseMenuHeight / 2;
		GUI.BeginGroup(new Rect(groupLeft, groupTop, ResourceManager.MenuWidth, ResourceManager.PauseMenuHeight));
		
		//background box
		GUI.Box(new Rect(0, 0, ResourceManager.MenuWidth, ResourceManager.PauseMenuHeight), "");
		//header image
		GUI.DrawTexture(new Rect(ResourceManager.Padding, ResourceManager.Padding, ResourceManager.HeaderWidth, ResourceManager.HeaderHeight), header);
		
		//menu buttons
		float leftPos = ResourceManager.MenuWidth / 2 - ResourceManager.ButtonWidth / 2;
		float topPos = 2 * ResourceManager.Padding + header.height;
		for(int i=0; i<buttons.Length; i++) {
			if(i > 0) topPos += ResourceManager.ButtonHeight + ResourceManager.Padding;
			if(GUI.Button(new Rect(leftPos, topPos, ResourceManager.ButtonWidth, ResourceManager.ButtonHeight), buttons[i])) {
				switch(buttons[i]) {
					case "Resume": Resume(); break;
					case "Exit Game": ExitGame(); break;
					default: break;
				}
			}
		}
		
		GUI.EndGroup();
	}
	
	private void Resume() {
		Time.timeScale = 1.0f;
		GetComponent<PauseMenu>().enabled = false;
		if(player) player.GetComponent<UserInput>().enabled = true;
		Screen.showCursor = false;
		ResourceManager.MenuOpen = false;
	}
	
	private void ExitGame() {
		Application.Quit();
	}
}