using UnityEngine;
using System.Collections;

public class OnOffVR : MonoBehaviour {


	public Transform rCam, lCam, cam;
	public bool isVR;
	// Use this for initialization
	void Start () {
		#if UNITY_ANDROID && !UNITY_EDITOR
		isVR = true;
		#endif

		if (isVR) {
			gameObject.SetActive (false);
		} else {
			gameObject.SetActive (true);
			rCam.gameObject.SetActive (false);
			rCam.gameObject.SetActive (false);

		}

	}

}
