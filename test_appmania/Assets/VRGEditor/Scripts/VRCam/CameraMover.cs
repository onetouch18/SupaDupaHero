using UnityEngine;
using System.Collections;

public class CameraMover : MonoBehaviour {

	public float sensitivityVert;
	public float sensitivityHor;

	private Vector3 pos;

	void Start(){
		pos = transform.position;
	}

	void Update () {
		#if UNITY_EDITOR


		Vector3 rot = transform.eulerAngles;
		rot.x -= Input.GetAxis("Mouse Y") * sensitivityVert;
		float delta = Input.GetAxis("Mouse X") * sensitivityHor;
		rot.y += delta;
		rot.z = 0;

		transform.eulerAngles = rot;
		transform.position += (Input.GetAxis ("Horizontal") * transform.right + Input.GetAxis ("Vertical") *  transform.forward)/2;
		Debug.Log(Input.GetAxis ("Horizontal"));
		#elif UNITY_ANDROID

		transform.position += (GearVRInput.GetAxisX * transform.GetChild(0).right + GearVRInput.GetAxisY *  transform.GetChild(0).forward)/4;


		#endif

	}
}
