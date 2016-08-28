using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

public enum Mobs
{
    Mob1 = 0,
    Mob2,
//	Mob3,
	//Mob1_1,
	//Mob1_old
}
[System.Serializable]
public class EnemiesSpawner : MonoBehaviour
{

    public SpawnerType type;
    public int number;
    public int waves;
    public float spawnTime;
	public float DelayTime = 0f;
    public Mobs MobType;
    public float MovementSpeed;
	public int Chance = 100;
	[HideInInspector]
    public States StartState = States.Walk;
	[HideInInspector]
    public bool KeepRotation;
	[HideInInspector]
	public int Mob3SpawnNumber = 1;
	[HideInInspector]
	public bool Empty;

    [HideInInspector]
    public int waveNum = 0;
    [HideInInspector]
    public int enem = 0;
    private string[] enemies = { "Mob1", "Mob2", "Mob3", "Mob_1_1","Mob1old" };
	[HideInInspector]
	public EnemiesSpawner[] spawners;
	[HideInInspector]
	public bool Spawned = false;
	[HideInInspector]
    public float Delay;

    [HideInInspector]
    public Animations _animation;
    [HideInInspector]
    public bool _isMirror;
    [HideInInspector]
    public HittableGlass _glass;
    [HideInInspector]
    public float _delay;
    [HideInInspector]
    public float _time;

#if UNITY_EDITOR
    [CustomEditor(typeof(EnemiesSpawner))]
    class EnemiesSpawnerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var obj = target as EnemiesSpawner;

            DrawDefaultInspector();

            switch (obj.StartState)
            {
                case States.Walk:
                    break;
                case States.UseAbility:
                    break;
                case States.Crawling:
                    EditorGUILayout.FloatField("Crawl time: ", obj._time);

                    
                    break;
                case States.GlassBreak:
                    obj._glass = (HittableGlass)EditorGUILayout.ObjectField("Glass: ", obj._glass, typeof(HittableGlass), true);
                    break;
                case States.Animation:
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label("Animation:");
                    //GUILayout.FlexibleSpace();
                    obj._animation = (Animations)EditorGUILayout.EnumPopup(obj._animation);
                    //GUILayout.FlexibleSpace();
                    GUILayout.Label("Is mirror:");
                    obj._isMirror = EditorGUILayout.Toggle(obj._isMirror);
                    EditorGUILayout.EndHorizontal();
                    break;
                default:
                    break;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif

    public void SpawnEnemies()
    {	
		if (!Spawned) {
			Spawned = true;
			GetSpawners ();
		}
		if (type == SpawnerType.WAVE)
            StartCoroutine("spawnWave");
        else if (type == SpawnerType.CONTINUOS)
            StartCoroutine("continuosSpawn");
    }
		

    IEnumerator spawnWave()
    {	
		enem = 0;
        while (enem < number)
        {	
			enem++;
            CrateEnemy();
            
            yield return new WaitForSeconds(Random.Range(0, 60) / 60);
        }
        waveNum++;
    }

    IEnumerator continuosSpawn()
    {
		while (waveNum < waves)
        {	
			for (int i = 0; i < number; i++) {
				CrateEnemy ();
			}
			waveNum++;
            yield return new WaitForSeconds(spawnTime);
        }
    }

	public void SpawnerUpdate(){
		if (isEmpty () && type == SpawnerType.WAVE && waveNum < waves  && Spawned) {
			SpawnEnemies ();
		} else if(isEmpty() && waveNum == waves){
			foreach (EnemiesSpawner sp in spawners) {
				if (sp != null && !sp.Spawned) {
					sp.Invoke("SpawnEnemies", sp.DelayTime);
				}else if(sp != null)
				sp.SpawnerUpdate ();
			}
			Empty = CheckForEmpty ();
		}

	}


    private void CrateEnemy()
    {
        GameObject o;
		int r = Random.Range (0, 100);
		if (r <= Chance) {
			if (PhotonNetwork.connected)
				o = PhotonNetwork.Instantiate (enemies [(int)MobType], transform.position,
					Quaternion.identity, 0);
			else
				o = Instantiate (Resources.Load (enemies [(int)MobType]), transform.position, Quaternion.identity) as GameObject;
			o.transform.parent = transform;
			o.transform.gameObject.SetActive (true);
			if (KeepRotation) {
				o.transform.rotation = transform.rotation;
			}
			var enemi = o.GetComponentInChildren<EnemyBase> ();
           // enemi.Glass = _glass;
          //  enemi.Delay = Delay;
            enemi.Animation = _animation;
            enemi.IsMirror = _isMirror;
            enemi.StateExitTime = _time;
            enemi.Init (StartState);
   

            if (MovementSpeed != 0)
				enemi.Speed = MovementSpeed;
			//if(MobType == Mobs.Mob3)
				//o.GetComponentInChildren<Mob3>().SpawnCount = Mob3SpawnNumber;
			o.SetActive (true);
		}
    }

	public bool isEmpty(){
		int e = 0;
		foreach (Transform t in transform) {
			if (t.tag == "Enemy")
				e++;
		}
		return e == 0;
	}


	void GetSpawners(){
		EnemiesSpawner[] e = new EnemiesSpawner[transform.childCount];
		for (int i = 0; i < transform.childCount; i++) {
			e[i] = transform.GetChild (i).GetComponent<EnemiesSpawner> ();
		}
		spawners = e;
	}

	public bool CheckForEmpty(){
		int e = 0;
		foreach (EnemiesSpawner sp in spawners) {
			if (sp != null && sp.Empty && sp.Spawned)
				e++;
		}
		return e == transform.childCount;
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
					if (sp.transform.GetChild (i).tag == "Enemy")
						PhotonNetwork.Destroy (sp.transform.GetChild (i).gameObject);
				}
			}
		}
	}

}
