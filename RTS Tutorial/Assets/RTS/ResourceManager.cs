using UnityEngine;
using System.Collections.Generic;

namespace RTS {
	public static class ResourceManager {
		public static int ScrollWidth { get { return 15; } }
		public static float ScrollSpeed { get { return 25; } }
		public static float RotateAmount { get { return 10; } }
		public static float RotateSpeed { get { return 100; } }
		public static float MinCameraHeight { get { return 10; } }
		public static float MaxCameraHeight { get { return 40; } }
		
		private static Vector3 invalidPosition = new Vector3(-99999, -99999, -99999);
		private static Bounds invalidBounds = new Bounds(new Vector3(-99999, -99999, -99999), new Vector3(0, 0, 0));
		public static Vector3 InvalidPosition { get { return invalidPosition; } }
		public static Bounds InvalidBounds { get { return invalidBounds; } }
		
		public static bool MenuOpen { get; set; }
		public static string LevelName { get; set; }
		
		private static float buttonHeight = 40;
		private static float headerHeight = 32, headerWidth = 256;
		private static float textHeight = 25, padding = 10;
		public static float MenuWidth { get { return headerWidth + 2 * padding; } }
		public static float ButtonHeight { get { return buttonHeight; } }
		public static float ButtonWidth { get { return (MenuWidth - 3 * padding) / 2; } }
		public static float HeaderHeight { get { return headerHeight; } }
		public static float HeaderWidth { get { return headerWidth; } }
		public static float TextHeight { get { return textHeight; } }
		public static float Padding { get { return padding; } }
		
		private static GUISkin selectBoxSkin;
		public static GUISkin SelectBoxSkin { get { return selectBoxSkin; } }
		
		private static Texture2D healthyTexture, damagedTexture, criticalTexture;
		public static Texture2D HealthyTexture { get { return healthyTexture; } }
		public static Texture2D DamagedTexture { get { return damagedTexture; } }
		public static Texture2D CriticalTexture { get { return criticalTexture; } }
		
		private static Dictionary<ResourceType, Texture2D> resourceHealthBarTextures;
		
		public static void StoreSelectBoxItems(GUISkin skin, Texture2D healthy, Texture2D damaged, Texture2D critical) {
			selectBoxSkin = skin;
			healthyTexture = healthy;
			damagedTexture = damaged;
			criticalTexture = critical;
		}
		
		public static int BuildSpeed { get { return 2; } }
		
		private static GameObjectList gameObjectList;
		public static void SetGameObjectList(GameObjectList objectList) {
			gameObjectList = objectList;
		}
		
		public static GameObject GetBuilding(string name) {
			return gameObjectList.GetBuilding(name);
		}
		
		public static GameObject GetUnit(string name) {
			return gameObjectList.GetUnit(name);
		}
		
		public static GameObject GetWorldObject(string name) {
			return gameObjectList.GetWorldObject(name);
		}
		
		public static GameObject GetPlayerObject() {
			return gameObjectList.GetPlayerObject();
		}
		
		public static Texture2D GetBuildImage(string name) {
			return gameObjectList.GetBuildImage(name);
		}
		
		public static void SetResourceHealthBarTextures(Dictionary<ResourceType, Texture2D> images) {
			resourceHealthBarTextures = images;
		}
		
		public static Texture2D GetResourceHealthBar(ResourceType resourceType) {
			if(resourceHealthBarTextures != null && resourceHealthBarTextures.ContainsKey(resourceType)) return resourceHealthBarTextures[resourceType];
			return null;
		}
		
		public static int GetNewObjectId() {
			LevelLoader loader = (LevelLoader)GameObject.FindObjectOfType(typeof(LevelLoader));
			if(loader) return loader.GetNewObjectId();
			return -1;
		}
		
		public static Texture2D[] GetAvatars() {
			return gameObjectList.GetAvatars();
		}
	}
}