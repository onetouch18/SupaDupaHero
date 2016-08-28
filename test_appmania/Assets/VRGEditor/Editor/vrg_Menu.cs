
using UnityEngine;
using System.Collections;
using UnityEditor;

public class vrg_Menu : MonoBehaviour{

	[MenuItem("Tools/VRG/AddController")]
	static void AddController(){
		if(Camera.main != null)
		DestroyImmediate (Camera.main.gameObject);
		Instantiate (Resources.Load("Code"), Vector3.zero, Quaternion.identity);
		Instantiate (Resources.Load("VRCam"), Vector3.zero, Quaternion.identity);
	}
}
