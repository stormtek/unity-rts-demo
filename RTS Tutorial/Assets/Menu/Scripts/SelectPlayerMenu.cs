using UnityEngine;
using RTS;

public class SelectPlayerMenu : MonoBehaviour {
	
	public GUISkin mySkin, selectionSkin;
	public Texture2D[] avatars;
	
	private string playerName = "NewPlayer";
	private int avatarIndex = -1;
	
	// Use this for initialization
	void Start () {
		PlayerManager.Load();
		if(avatars.Length > 0) avatarIndex = 0;
		PlayerManager.SetAvatarTextures(avatars);
		SelectionList.LoadEntries(PlayerManager.GetPlayerNames());
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnGUI() {
		if(SelectionList.MouseDoubleClick()) {
			playerName = SelectionList.GetCurrentEntry();
			SelectPlayer();
		}
		
		GUI.skin = mySkin;
		
		float menuHeight = GetMenuHeight();
		float groupLeft = Screen.width / 2 - ResourceManager.MenuWidth / 2;
		float groupTop = Screen.height / 2 - menuHeight / 2;
		Rect groupRect = new Rect(groupLeft, groupTop, ResourceManager.MenuWidth, menuHeight);
		
		GUI.BeginGroup(groupRect);
		//background box
		GUI.Box(new Rect(0, 0, ResourceManager.MenuWidth, menuHeight), "");
		//menu buttons
		float leftPos = ResourceManager.MenuWidth / 2 - ResourceManager.ButtonWidth / 2;
		float topPos = menuHeight - ResourceManager.Padding - ResourceManager.ButtonHeight;
		if(GUI.Button(new Rect(leftPos, topPos, ResourceManager.ButtonWidth, ResourceManager.ButtonHeight), "Select")) {
			SelectPlayer();
		}
		//text area for player to type new name
		float textTop = menuHeight - 2 * ResourceManager.Padding - ResourceManager.ButtonHeight - ResourceManager.TextHeight;
		float textWidth = ResourceManager.MenuWidth - 2 * ResourceManager.Padding;
		playerName = GUI.TextField(new Rect(ResourceManager.Padding, textTop, textWidth, ResourceManager.TextHeight), playerName, 14);
		SelectionList.SetCurrentEntry(playerName);
		//avatar selection list
		if(avatarIndex >= 0) {
			float avatarLeft = ResourceManager.MenuWidth / 2 - avatars[avatarIndex].width / 2;
			float avatarTop = textTop - ResourceManager.Padding - avatars[avatarIndex].height;
			float avatarWidth = avatars[avatarIndex].width;
			float avatarHeight = avatars[avatarIndex].height;
			GUI.DrawTexture(new Rect(avatarLeft, avatarTop, avatarWidth, avatarHeight), avatars[avatarIndex]);
			float buttonTop = textTop - ResourceManager.Padding - ResourceManager.ButtonHeight;
			float buttonLeft = ResourceManager.Padding;
			if(GUI.Button(new Rect(buttonLeft, buttonTop, ResourceManager.ButtonHeight, ResourceManager.ButtonHeight), "<")) {
				avatarIndex -= 1;
				if(avatarIndex < 0) avatarIndex = avatars.Length - 1;
			}
			buttonLeft = ResourceManager.MenuWidth - ResourceManager.Padding - ResourceManager.ButtonHeight;
			if(GUI.Button(new Rect(buttonLeft, buttonTop, ResourceManager.ButtonHeight, ResourceManager.ButtonHeight), ">")) {
				avatarIndex = (avatarIndex+1) % avatars.Length;
			}
		}
		GUI.EndGroup();
		
		//selection list, needs to be called outside of the group for the menu
		string prevSelection = SelectionList.GetCurrentEntry();
		float selectionLeft = groupRect.x + ResourceManager.Padding;
		float selectionTop = groupRect.y + ResourceManager.Padding;
		float selectionWidth = groupRect.width - 2 * ResourceManager.Padding;
		float selectionHeight = groupRect.height - GetMenuItemsHeight() - ResourceManager.Padding;
		SelectionList.Draw(selectionLeft, selectionTop, selectionWidth, selectionHeight, selectionSkin);
		string newSelection = SelectionList.GetCurrentEntry();
		//set saveName to be name selected in list if selection has changed
		if(prevSelection != newSelection) {
			playerName = newSelection;
			avatarIndex = PlayerManager.GetAvatar(playerName);
		}
	}
	
	private float GetMenuHeight() {
		return 250 + GetMenuItemsHeight();
	}
	
	private float GetMenuItemsHeight() {
		float avatarHeight = 0;
		if(avatars.Length > 0) avatarHeight = avatars[0].height + 2 * ResourceManager.Padding;
		return avatarHeight + ResourceManager.ButtonHeight + ResourceManager.TextHeight + 3 * ResourceManager.Padding;
	}
	
	private void SelectPlayer() {
		PlayerManager.SelectPlayer(playerName, avatarIndex);
		GetComponent<SelectPlayerMenu>().enabled = false;
		MainMenu main = GetComponent<MainMenu>();
		if(main) main.enabled = true;
	}
}
