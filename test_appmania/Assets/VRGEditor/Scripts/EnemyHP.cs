using UnityEngine;
using System.Collections;

public class EnemyHP : MonoBehaviour {


	public TextMesh text;
	private float hp;
	private EnemyBase enemy;
	// Use this for initialization
	void Start () {
		enemy = GetComponent<EnemyBase> ();
		text.gameObject.SetActive (true);
		hp = enemy.Health;
	}
	
	// Update is called once per frame
	void Update () {
		text.transform.LookAt (2 * text.transform.position - Camera.main.transform.position);
		text.text = enemy.Health + "/" + hp;
	}
}
