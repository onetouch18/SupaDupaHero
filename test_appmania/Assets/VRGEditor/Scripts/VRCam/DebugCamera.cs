using UnityEngine;
using System.Collections;

public class DebugCamera : MonoBehaviour {

	public float sensitivityVert;
	public float sensitivityHor;

	void Start(){
		#if !ENABLE_VR
		GetComponent<OculusCanvas>().enabled = false;
		#endif
		#if UNITY_ANDROID && !UNITY_EDITOR
		this.enabled = false;
		#endif
	}
	
	// Update is called once per frame
	void Update () {
		
		Vector3 rot = transform.eulerAngles;
		rot.x -= Input.GetAxis("Mouse Y") * sensitivityVert;
		float delta = Input.GetAxis("Mouse X") * sensitivityHor;
		rot.y += delta;
		rot.z = 0;

		transform.eulerAngles = rot;

	}
}
