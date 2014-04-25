using UnityEngine;
using RTS;

public class SaveMenu : MonoBehaviour {
	
	public GUISkin mySkin, selectionSkin, confirmationSkin;
	
	private string saveName = "NewGame";
	private ConfirmDialog confirmDialog = new ConfirmDialog();
	
	void Start () {
		Activate();
	}
	
	void Update () {
		//handle escape key	
		if(Input.GetKeyDown(KeyCode.Escape)) {
			if(confirmDialog.IsConfirming()) confirmDialog.EndConfirmation();
			else CancelSave();
		}
		//handle enter key in confirmation dialog
		if(Input.GetKeyDown(KeyCode.Return) && confirmDialog.IsConfirming()) {
			confirmDialog.EndConfirmation();
			SaveGame();
		}
	}
	
	void OnGUI() {
		if(confirmDialog.IsConfirming()) {
			string message = "\"" + saveName + "\" already exists. Do you wish to continue?";
			confirmDialog.Show(message, mySkin);
		} else if(confirmDialog.MadeChoice()) {
			if(confirmDialog.ClickedYes()) SaveGame();
			confirmDialog.EndConfirmation();
		} else {
			if(SelectionList.MouseDoubleClick()) {
				saveName = SelectionList.GetCurrentEntry();
				StartSave();
			}
			GUI.skin = mySkin;
			DrawMenu();
			//handle enter being hit when typing in the text field
			if(Event.current.keyCode == KeyCode.Return) StartSave();
		}
	}
	
	public void Activate() {
		SelectionList.LoadEntries(PlayerManager.GetSavedGames());
		if(ResourceManager.LevelName != null && ResourceManager.LevelName != "") saveName = ResourceManager.LevelName;
	}
	
	private void DrawMenu() {
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
		if(GUI.Button(new Rect(leftPos, topPos, ResourceManager.ButtonWidth, ResourceManager.ButtonHeight), "Save Game")) {
			StartSave();
		}
		leftPos += ResourceManager.ButtonWidth + ResourceManager.Padding;
		if(GUI.Button(new Rect(leftPos, topPos, ResourceManager.ButtonWidth, ResourceManager.ButtonHeight), "Cancel")) {
			CancelSave();
		}
		//text area for player to type new name
		float textTop = menuHeight - 2 * ResourceManager.Padding - ResourceManager.ButtonHeight - ResourceManager.TextHeight;
		float textWidth = ResourceManager.MenuWidth - 2 * ResourceManager.Padding;
		saveName = GUI.TextField(new Rect(ResourceManager.Padding, textTop, textWidth, ResourceManager.TextHeight), saveName, 60);
		SelectionList.SetCurrentEntry(saveName);
		GUI.EndGroup();
		
		//selection list, needs to be called outside of the group for the menu
		string prevSelection = SelectionList.GetCurrentEntry();
		float selectionLeft = groupRect.x + ResourceManager.Padding;
		float selectionTop = groupRect.y + ResourceManager.Padding;
		float selectionWidth = groupRect.width - 2 * ResourceManager.Padding;
		float selectionHeight = groupRect.height - GetMenuItemsHeight() - ResourceManager.Padding;
		SelectionList.Draw(selectionLeft,selectionTop,selectionWidth,selectionHeight,selectionSkin);
		string newSelection = SelectionList.GetCurrentEntry();
		//set saveName to be name selected in list if selection has changed
		if(prevSelection != newSelection) saveName = newSelection;
	}
	
	private float GetMenuHeight() {
		return 250 + GetMenuItemsHeight();
	}
	
	private float GetMenuItemsHeight() {
		return ResourceManager.ButtonHeight + ResourceManager.TextHeight + 3 * ResourceManager.Padding;
	}
	
	private void StartSave() {
		//prompt for override of name if necessary
		if(SelectionList.Contains(saveName)) confirmDialog.StartConfirmation();
		else SaveGame();
	}
	
	private void CancelSave() {
		GetComponent<SaveMenu>().enabled = false;
		PauseMenu pause = GetComponent<PauseMenu>();
		if(pause) pause.enabled = true;
	}
	
	private void SaveGame() {
		SaveManager.SaveGame(saveName);
		ResourceManager.LevelName = saveName;
		GetComponent<SaveMenu>().enabled = false;
		PauseMenu pause = GetComponent<PauseMenu>();
		if(pause) pause.enabled = true;
	}
}
