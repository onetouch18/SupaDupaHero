using UnityEngine;
using System.Collections;
using UnityEditor;

public class Points : MonoBehaviour {
	
	void Start(){
		foreach (MeshRenderer r in GetComponentsInChildren<MeshRenderer>()) {
			r.enabled = false;
		}
	}

	public void Reset(){

		for (int i = 0; i < transform.childCount; i++) {
			transform.GetChild (i).GetComponent<Room> ().Reset ();
		}
	}
}

[CustomEditor(typeof(Points))]
public class PointEditor : Editor{

	public override void OnInspectorGUI () {
		var obj = target as Points;

		EditorGUI.BeginChangeCheck();
		EditorGUILayout.BeginHorizontal ();

		if (GUILayout.Button ("Add Point")) {
			Vector3 tr;
			if (obj.transform.childCount == 0) {
				tr = obj.transform.position;
			} else {
				tr = obj.transform.GetChild (obj.transform.childCount - 1).position + Vector3.forward;
			}
			var go = Instantiate (Resources.Load ("player_point"), tr, Quaternion.identity) as GameObject;
			go.transform.parent = obj.transform;
			go.name = "Point " + obj.transform.childCount;
		}
	}
}
