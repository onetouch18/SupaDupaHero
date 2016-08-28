using UnityEngine;
using System.Collections;

public class CameraLook : MonoBehaviour {

	public GameObject player;
	public float CameraHeight;

	void Start(){
		/*
		player = GameObject.FindGameObjectWithTag ("CameraPos");
		transform.position = player.transform.position;
		Vector3 rot = player.transform.rotation.eulerAngles;
		rot.x = 0f;
		rot.z = 0f;
		Quaternion r = transform.rotation;
		r.eulerAngles = rot;
		transform.rotation = r;
		*/
		Vector3 pos = new Vector3 (0, CameraHeight, 0);
		transform.rotation = GameManager.localPlayer.transform.rotation;
		transform.parent = GameManager.localPlayer.transform;
		transform.localPosition = pos;
	}


	// Update is called once per frame
	void Update () {
		/*
		Vector3 p = player.transform.position;
		p.y = transform.position.y;	
		transform.position = p;
		transform.rotation = GameManager.localPlayer.transform.rotation;

		/*
		Vector3 rot = transform.eulerAngles;
		//rot.x += Input.GetAxis("Mouse Y") * sensitivityVert;
		float delta = Input.GetAxis("Mouse X") * sensitivityHor;
		rot.y += delta;
		//rot.z = 0;

		transform.eulerAngles = rot;
		*/
	}
	/*
	void OnGUI(){
		int size = 12;
		float posX = Camera.main.pixelWidth / 2 - size / 4;
		float posY = Camera.main.pixelHeight / 2 - size / 2;
		GUI.Label(new Rect(posX,posY, size, size), "*");
	}
	*/
}
