using UnityEngine;

public class RallyPoint : MonoBehaviour {

	public void Enable () {
		Renderer[] renderers = GetComponentsInChildren<Renderer>();
		foreach(Renderer renderer in renderers) renderer.enabled = true;
	}
	
	public void Disable () {
		Renderer[] renderers = GetComponentsInChildren<Renderer>();
		foreach(Renderer renderer in renderers) renderer.enabled = false;
	}
}
