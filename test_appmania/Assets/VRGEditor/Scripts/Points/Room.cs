using UnityEngine;
using System.Collections;
using UnityEditor;

public enum RoomType{
	WALK,
	LIFT
}
	

public class Room : MonoBehaviour {


	public RoomType type;
	public bool debugPlayer;
	//public Destruction destruct;
	[HideInInspector]
	public bool canGo = false;
	[HideInInspector]
	public bool changeDirection = false;
	[HideInInspector]
	public int directionNum = 0;
	[HideInInspector]
	public  Direction dir;
	private bool changed = false;
	[HideInInspector]
	public bool isEnter = false;
	private GameObject player;
	[HideInInspector]
	public MGCurve Way;

    //public float Timer = 0f;

	void Start(){
		Way = GetComponent<MGCurve> ();
		if (PhotonNetwork.isMasterClient || !PhotonNetwork.connected) {
			if (GetComponent<SphereCollider> ().radius < 0.5f)
				GetComponent<SphereCollider> ().radius = 0.5f;
			if (getDirection() != null) {
				dir = getDirection ();
			}
		}

        //dir.RefreshSpawners();

    }

	void OnTriggerEnter(Collider collider){
		if (collider.tag == "Player") {
			if (dir) {
				//dir.StartDir ();
			}
			if (transform.childCount == 0 && type != RoomType.LIFT) {
				canGo = true;
			}
			isEnter = true;
		}


	}

	public void RoomUpdate(){
        //Timer += Time.deltaTime;

		if ((isEnter && transform.childCount > 0 && (PhotonNetwork.connected && PhotonNetwork.isMasterClient)) || 
			(isEnter && transform.childCount > 0 && !PhotonNetwork.connected)) {
			if (!changed && dir) {
				changed = true;
				changeDirection = true;
				dir.StartDir ();
			}
			dir.DirectionUpdate ();
			if (dir.CheckForEmpty ()) {
				directionNum++;

				if (dir.door){
					if (dir.door.Type == DoorType.AUTOMATIC)
						dir.door.Invoke ("Open", dir.waitTime);
					//else {
						//dir.door.transform.GetChild (0).gameObject.SetActive (true);
					//}
				}

				if (directionNum < transform.childCount) {
					changeDirection = true;
					dir = getDirection ();
					dir.StartDir ();

				} /*else if (destruct && !destruct.isDesctruct)
					destruct.Desctruct ();
				else
				*/
				else
				canGo = true;
			}
		}
	}

	public void spawnPlayer(){
		player = (GameObject) Instantiate (Resources.Load("Debug"), transform.position, Quaternion.identity);
	}

	public void deletePlayer(){
		Destroy (player);
	}

	public void Reset(){
		changed = false;
		canGo = false;
		isEnter = false;
		dir.Reset();
		directionNum = 0;
		dir = getDirection ();

	}

	private Direction getDirection(){
		if (transform.childCount > directionNum)
			return transform.GetChild (directionNum).GetComponent<Direction> ();
		else
			return null;
	}
		
}

[CustomEditor(typeof(Room))]
public class RoomEditor : Editor{

	public override void OnInspectorGUI () {
		var obj = target as Room;
		DrawDefaultInspector();
		EditorGUI.BeginChangeCheck();
		EditorGUILayout.BeginHorizontal ();

		if (GUILayout.Button ("Add Direction")) {
			Vector3 tr;
			if (obj.transform.childCount == 0) {
				tr = obj.transform.position;
			} else {
				tr = obj.transform.GetChild (obj.transform.childCount - 1).position + Vector3.forward;
			}
			var go = Instantiate (Resources.Load ("Direction"), tr, Quaternion.identity) as GameObject;
			go.transform.parent = obj.transform;
			go.name = "Direction " + obj.transform.childCount;
		}
	}
}
