using UnityEngine;
using System.Collections;

[System.Serializable]
public class LevelManager : /*Photon.*/MonoBehaviour {


	public Transform[] players;
	public float RespawnTime = 4f;

	private Player player;
	public static int point;
	private Room curentRoom;

	// Use this for initialization
	void Start () {
		/*
		GameObject g;

		if (PhotonNetwork.connected) {
			g = PhotonNetwork.Instantiate ("Mob1", Vector3.zero, Quaternion.identity, 0);
			g.GetComponentInChildren<EnemyBase>().Init (StateFactory.GetState (States.Walk));
			g.SetActive (true);
			StartCoroutine (Deact (g, 1f));
			g = PhotonNetwork.Instantiate ("Mob2", Vector3.zero, Quaternion.identity, 0);
			g.GetComponentInChildren<EnemyBase>().Init (StateFactory.GetState (States.Walk));
			g.SetActive (true);
			StartCoroutine (Deact (g, 1f));
			g = PhotonNetwork.Instantiate ("Mob3", Vector3.zero, Quaternion.identity, 0);
			g.GetComponentInChildren<EnemyBase>().Init (StateFactory.GetState (States.Walk));
			g.SetActive (true);
			StartCoroutine (Deact (g, 1f));
		} else {
		*/
		/*	
			g = Instantiate(Resources.Load ("Mob1"), Vector3.zero, Quaternion.identity) as GameObject;
			g.GetComponentInChildren<EnemyBase>().Init (States.Walk);
			g.SetActive (true);
			StartCoroutine (Deact (g, 1f));
			g = Instantiate(Resources.Load ("Mob2"), Vector3.zero, Quaternion.identity) as GameObject;
			g.GetComponentInChildren<EnemyBase>().Init (States.Walk);
			g.SetActive (true);
			StartCoroutine (Deact (g, 1f));
			g = Instantiate(Resources.Load ("Mob3"), Vector3.zero, Quaternion.identity) as GameObject;
			g.GetComponentInChildren<EnemyBase>().Init (States.Walk);
			g.SetActive (true);
			StartCoroutine (Deact(g, 1f));
			
			//Resources.LoadAsync ("Mob2");
			//Resources.LoadAsync ("Mob3");
		*/
		Resources.LoadAll ("");
		point = 0;
		player = GameManager.localPlayer.GetComponent<Player> ();
		player.OnAction += Died;
		curentRoom =  players [GameManager.playerNum - 1].GetChild (point).GetComponent<Room> ();

		//curentRoom.StartRoom ();
		foreach (Transform t in players) {
			if (t.childCount > point && t.GetChild (point).GetComponent<Room> ().debugPlayer) {
				t.GetChild (point).GetComponent<Room> ().spawnPlayer ();
			}
		}

	}

	IEnumerator Deact(GameObject o, float delayTime){

		yield return new WaitForSeconds (delayTime);

		o.SetActive (false);
	}

	float time;
	bool pressed = false;
	// Update is called once per frame
	void Update () {
		if (GameManager.ready) {
			curentRoom.RoomUpdate ();
			if (point <= players [0].childCount - 1) {
				curentRoom.canGo = players [0].GetChild (point).GetComponent<Room> ().canGo;
				if (curentRoom.canGo /*&& player.state != PlayerState.DIE*/) {
						changeRoom ();
				}
				if (point > players [GameManager.playerNum - 1].childCount - 1) {
					player.Finish ();
					curentRoom.canGo = false;
					player.GetComponentInChildren<Rifle> ().canShoot = false;
				}
				if (curentRoom.changeDirection) {
					curentRoom.changeDirection = false;
					if (curentRoom.Way == null)
						player.StartCoroutine ("RotateTo", players [0].GetChild (point).GetComponent<Room> ().dir.transform.position);
					player.Reset ();
				}
			}

			if (player.state == PlayerState.DIE || player.state == PlayerState.WAIT) {
				CheckForDie ();
			}
		} else checkPlayers ();
	}
		

	void CheckForDie(){
		int i = 0;
		foreach (Transform p in GameManager.players) {
			if (p.GetComponent<Player> ().state == PlayerState.DIE || p.GetComponent<Player> ().state == PlayerState.WAIT)
				i++;
			//Debug.LogError (i);
		}
		if (i == PhotonNetwork.playerList.Length) {
			player.CheckDie (true, RespawnTime);
		} else {
			player.CheckDie (false, RespawnTime);
		}
	}

	void Died(){
		if(PhotonNetwork.isMasterClient || !PhotonNetwork.connected)
		curentRoom.Reset ();
		if (curentRoom.Way == null) {
			player.MoveTo (curentRoom.transform.position);
		} else
			player.StartCoroutine (player.MoveByCurve (curentRoom.Way));

	}

	void checkPlayers(){
		int i = 0;
		foreach (Transform t in GameManager.players) {
			if (GameManager.IsTutor) {
				//t.GetComponent<Player> ().state = PlayerState.READY;
			}
			if (t.GetComponent<Player> ().state == PlayerState.READY) {
				i++;
			}
		}
		if (i == GameManager.players.Count) {
			GameManager.ready = true;
			if (curentRoom.Way == null) {
				player.MoveTo (curentRoom.transform.position);
			} else
				player.StartCoroutine (player.MoveByCurve (curentRoom.Way));
		}
	}
		
	public void StartFromPoint(int Point){
		point = Point-1;
		changeRoom ();
		player.gameObject.SetActive (false);
		player.transform.position = curentRoom.transform.position;
		if(curentRoom.dir)
		player.transform.LookAt(new Vector3(curentRoom.dir.transform.position.x, player.transform.position.y, curentRoom.dir.transform.position.z));
		player.gameObject.SetActive (true);
		if(curentRoom.Way == null){
			player.MoveTo (curentRoom.transform.position);
			//player.StartCoroutine ("RotateTo", players [0].GetChild (point).GetComponent<Room> ().dir.transform.position);
		} else  
			player.StartCoroutine (player.MoveByCurve (curentRoom.Way));


	}

		void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info){
			if (stream.isWriting) {
			if (PhotonNetwork.isMasterClient){ 
				stream.SendNext (point);
				stream.SendNext(curentRoom.changeDirection);
				stream.SendNext (curentRoom.directionNum);
			}
			} else {
			if (!PhotonNetwork.isMasterClient) {
				point = (int)stream.ReceiveNext ();
				curentRoom.changeDirection = (bool) stream.ReceiveNext();
				curentRoom.directionNum = (int) stream.ReceiveNext();;
			}
			}

		}

	private void Reset (){
		point = 0;
		players[(GameManager.playerNum - 1)].GetComponent<Points> ().Reset ();
		GameManager.Reset ();
	}
	[PunRPC]
	private void changeRoom(){
		foreach (Transform t in players) {
			if (t.childCount > point  && t.GetChild (point).GetComponent<Room> ().debugPlayer) {
				t.GetChild (point).GetComponent<Room> ().deletePlayer();
			}
		}
		point++;
		if (point <= players [GameManager.playerNum - 1].childCount - 1) {
			
			curentRoom = players [GameManager.playerNum - 1].GetChild (point).GetComponent<Room> ();
			player.checkPoint = players [GameManager.playerNum - 1].GetChild (point - 1).position;
			//curentRoom.StartRoom ();
			if (player.state == PlayerState.DIE) {
				player.Resp ();
			}
			if (curentRoom.Way == null) {
				player.StopAllCoroutines ();
				player.MoveTo (curentRoom.transform.position);
			} else {
				player.StopAllCoroutines ();
				player.StartCoroutine (player.MoveByCurve (curentRoom.Way));
			}
		} 

		foreach (Transform t in players) {
			if (t.childCount > point  && t.GetChild (point).GetComponent<Room> ().debugPlayer) {
				t.GetChild (point).GetComponent<Room> ().spawnPlayer ();
			}
		}
			
	}
		
		
}
