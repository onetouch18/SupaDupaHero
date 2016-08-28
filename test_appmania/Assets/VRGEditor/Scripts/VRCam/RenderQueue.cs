using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RenderQueue : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Image[] im = GetComponentsInChildren<Image> ();
		Debug.Log (im.Length);
		foreach (Image i in im) {
			Debug.Log (i.materialForRendering.renderQueue);
			//i.canvas.renderMode = RenderMode.ScreenSpaceOverlay;
		}

	}

}
