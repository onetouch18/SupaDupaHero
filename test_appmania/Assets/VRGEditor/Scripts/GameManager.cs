using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[System.Serializable]
public class GameManager : Photon.MonoBehaviour
{

    public GUISkin skin;
   // public Transform[] playersPrefab;
    public static Transform localPlayer;
	public static int playerNum;
    public static List<Transform> players = new List<Transform>();
	public bool Tutorial;
	public static int score = 0;
	public TextMesh text;
	public static bool ready = false;
	public static bool IsTutor = false;
	public static bool God = false;

	public Transform[] resp;

	static Vector3 pResp;
	public static int Chapter = 1;

    public static Transform GetClosestPlayer(Vector3 fromPos)
	{	
		Vector3 k = new Vector3 ();
        Transform close = null;
        float dist = -1;
        foreach (Transform tra in players)
        {
            if (tra == null)
            {
                continue;
            }
            float thisD = Vector3.Distance(tra.position, fromPos);
            if (dist == -1 || thisD < dist)
            {
                dist = thisD;
                close = tra;
            }
        }
        return close;
    }

	public void GodMode(){
		God ^= true;
		if (God == true) {
			localPlayer.GetComponent<Player> ().granades++;
		}else 
			localPlayer.GetComponent<Player> ().granades--;
	}

	public void DebugMod(){
		GameObject GO;
		Vector3 t = resp[PhotonNetwork.playerList.Length - 1].position;
		if (PhotonNetwork.connected) {
			PhotonNetwork.Destroy (localPlayer.gameObject);
		} else {
			Destroy (localPlayer.gameObject);
		}
		GO = Instantiate (Resources.Load ("DebugCamera"), t, resp[PhotonNetwork.playerList.Length - 1].rotation) as GameObject;
		GO.transform.forward = resp[PhotonNetwork.playerList.Length - 1].forward;
		localPlayer = GO.transform;
		GameObject.Find ("VRCam").SetActive (false);
	}

    public static void AddPlayer(Transform tra)
    {
        players.Add(tra);
		foreach (Transform t in players) {
			Debug.Log (t.name);
		}
    }
    public static void RemovePlayer(Transform tra)
    {
        players.Remove(tra);
    }

    void Awake()
	{	//Input.simulateMouseWithTouches = true;
		NavMeshBuilder.BuildNavMesh();
		ready = false;
		players.Clear ();
		localPlayer = null;
		playerNum = 0;
		StopAllCoroutines ();

		IsTutor = Tutorial;
		/*
		if (!IsTutor)
		{
		    //Time.timeScale = 1;
			if (Application.loadedLevel < 1) {
				// Debug.LogError("Configuration error: You have not yet added any scenes to your buildsettings. The current scene should be preceded by the mainmenu scene. Please see the README file for instructions on setting up the buildsettings.");
				// return;
			}
			//PhotonNetwork.sendRateOnSerialize = 10;
			//PhotonNetwork.logLevel = NetworkLogLevel.Full;

			// Connect to the main photon server. This is the only IP and port we ever need to set(!)

			if (!PhotonNetwork.connected || PhotonNetwork.room == null) {
				Application.LoadLevel (0);
				return;
			}
		}
		*/
       //
        //Spawn our local player
		PhotonNetwork.isMessageQueueRunning = true;
		Vector3 t = resp[PhotonNetwork.playerList.Length - 1].position;

		GameObject GO;
			if (PhotonNetwork.connected)
			GO = PhotonNetwork.Instantiate ("Player 1", t, resp[PhotonNetwork.playerList.Length - 1].rotation, 0);
			else
			GO = Instantiate (Resources.Load("Player 1"), t, resp[PhotonNetwork.playerList.Length - 1].rotation) as GameObject;
			GO.name = "Player" + PhotonNetwork.playerList.Length;
			GO.GetComponent<Player> ().checkPoint = t;
			localPlayer = GO.transform;
			playerNum = PhotonNetwork.playerList.Length;
			pResp = t;
			//GO.transform.forward = resp.GetChild (PhotonNetwork.playerList.Length - 1).forward;
		
    }

    void OnGUI()
    {
        GUI.skin = skin;
        GameGUI();

    }

	bool showDebug = true;

    void Update()
	{	
		if(text)
		text.text = "score:" + Input.GetAxis("Mouse X");
        if (Input.GetKeyDown(KeyCode.Q))
            showDebug = !showDebug;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

    }

	public static void Reset(){
		localPlayer.position = pResp;
	}
		
    /// <summary>
    /// Check if the game is allowed to click here (for starting FIRE etc.)
    /// </summary>
    /// <returns></returns>
    public static bool GameCanClickHere()
    {
        List<Rect> rects = new List<Rect>();
        rects.Add(new Rect(0, 0, 110, 55));//Topleft Button
        rects.Add(new Rect(0, Screen.height - 35, 275, 35));//Chat

        Vector2 pos = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
        foreach (Rect re in rects)
        {
            if (re.Contains(pos))
                return false;
        }
        return true;

    }

    void GameGUI()
    {
        GUILayout.Space(32);
        if (GUILayout.Button("Leave"))
        {
            PhotonNetwork.LeaveRoom();
            Application.LoadLevel(Application.loadedLevel + 1);
        }

        if (showDebug)
        {
            //GUILayout.Label("isMasterClient: " + PhotonNetwork.isMasterClient);
            GUILayout.Label("Players: " + PhotonNetwork.playerList.Length);
            GUILayout.Label("Ping: " + PhotonNetwork.GetPing());
			GUILayout.Label("FPS: " + 1/Time.deltaTime);
			GUILayout.Label("Point:" + (LevelManager.point + 1));
			GUILayout.Label("Score:" + score);
			//GUILayout.Label("MouseY" + Input.GetAxis("Mouse Y"));

        }
    }
	/*
	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info){
		if (stream.isWriting) {
			stream.SendNext (ready);
		} else {
			state = (PlayerState)stream.ReceiveNext();
		}

	}
	*/
}

[InitializeOnLoad]
[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor {

	public override void OnInspectorGUI () {
		var manager = target as GameManager;

		EditorGUI.BeginChangeCheck();

		DrawDefaultInspector();
		EditorGUILayout.BeginHorizontal ();
		if (GUILayout.Button ("Add Resp Point")) {
			var go = Instantiate (Resources.Load("Respaun"), Vector3.zero, Quaternion.identity) as GameObject;
			System.Array.Resize (ref manager.resp, manager.resp.Length + 1);
			manager.resp [manager.resp.Length - 1] = go.transform;
			go.name = "Player " + manager.resp.Length + " Respaun";
		}
		serializedObject.ApplyModifiedProperties();
	}
}