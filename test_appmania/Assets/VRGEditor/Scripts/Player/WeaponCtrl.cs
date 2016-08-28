using UnityEngine;
using System.Collections;

public class WeaponCtrl : MonoBehaviour {

	void Start(){
		//transform.parent = null;
	}
	void Update () {

		Vector3 ang = transform.eulerAngles;
		ang.y +=  Input.GetAxis ("Horizontal");
		ang.x +=  Input.GetAxis ("Vertical");
		transform.eulerAngles = ang;
	}
}
