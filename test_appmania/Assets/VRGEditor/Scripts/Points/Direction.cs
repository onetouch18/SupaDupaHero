using UnityEngine;
using System.Collections;
using UnityEditor;

public enum SpawnerType{
	WAVE,
	CONTINUOS,
}

public class Direction : MonoBehaviour {


	[HideInInspector]
	public bool changeDirection = false;

	public Door door;
	public float waitTime;

	private EnemiesSpawner[] spawners;


    public void RefreshSpawners()
    {
        EnemiesSpawner[] s = new EnemiesSpawner[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            s[i] = transform.GetChild(i).GetComponent<EnemiesSpawner>();
        }
        spawners = s;
    }
	public void StartDir(){

        RefreshSpawners();

        Spawn ();
		//Conditions cond = GetComponent<Conditions> ();
		//if (cond) {
			//cond.StartCondition ();
		//}
	}

	public void Spawn(){
		foreach (var sp in spawners) {
			sp.Invoke ("SpawnEnemies", sp.DelayTime);
		}
	}

	public void DirectionUpdate(){
		foreach (var sp in spawners) {
			sp.SpawnerUpdate ();
		}
	}

	public void Reset(){
		foreach (var sp in spawners) {
			if (sp != null) {
				sp.enem = 0;
				sp.waveNum = 0;
				sp.Spawned = false;
				sp.Reset ();
				sp.StopAllCoroutines ();
				for (int i = 0; i < sp.transform.childCount; i++) {
					if (sp.transform.GetChild (i).tag == "Enemy") {
						PhotonNetwork.Destroy (sp.transform.GetChild (i).gameObject);
					}
				}
			}
		}
	}
		
	public bool CheckForEmpty(){
		int e = 0;
		foreach (var sp in spawners) {
			if (sp.Empty && sp.Spawned)
				e++;
		}
		return e == transform.childCount;
	}
}


[CustomEditor(typeof(Direction))]
public class DirectionEditor : Editor{

	public override void OnInspectorGUI () {
		var obj = target as Direction;
		DrawDefaultInspector();
		EditorGUI.BeginChangeCheck();
		EditorGUILayout.BeginHorizontal ();

		if (GUILayout.Button ("Add Spawner")) {
			Vector3 tr;
			if (obj.transform.childCount == 0) {
				tr = obj.transform.position;
			} else {
				tr = obj.transform.GetChild (obj.transform.childCount - 1).position + Vector3.forward;
			}
			var go = Instantiate (Resources.Load ("Cube"), tr, Quaternion.identity) as GameObject;
			go.transform.parent = obj.transform;
			go.name = "Spawner " + obj.transform.childCount;
		}
	}
}