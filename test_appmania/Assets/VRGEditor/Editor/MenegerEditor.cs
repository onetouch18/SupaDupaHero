using UnityEngine;
using System.Collections;
using UnityEditor;

[InitializeOnLoad]
[CustomEditor(typeof(LevelManager))]
public class MenegerEditor : Editor {

	private LevelManager manager;

	void OnEnable(){
		manager = target as LevelManager;
	}

	public override void OnInspectorGUI () {
		EditorGUI.BeginChangeCheck();

		DrawDefaultInspector();

		EditorGUILayout.BeginHorizontal ();
		if (GUILayout.Button ("Add Player")) {
			var go = new GameObject();
			System.Array.Resize (ref manager.players, manager.players.Length + 1);
			manager.players [manager.players.Length - 1] = go.transform;
			go.name = "Player " + manager.players.Length + " Points";
			go.AddComponent<Points> ();
		}
		serializedObject.ApplyModifiedProperties();
	}
}
