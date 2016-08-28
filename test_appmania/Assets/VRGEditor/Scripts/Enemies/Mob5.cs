using UnityEngine;
using System.Collections;

public class Mob5 : EnemyBase{

	public GameObject AttackEffect;


	void Awake(){
		//Init (new MovingState ());
	}

	void Start(){
		_closenessLevel = Random.value;
	}

	public override void Attack ()
	{	
		GameObject o = Instantiate (AttackEffect, transform.position, Quaternion.LookRotation (transform.forward)) as GameObject;
		Vector3 ang = o.transform.eulerAngles;
		ang.y -= 90;
		o.transform.localScale *= 2;
		o.transform.eulerAngles = ang;
		Destroy (o, 1f);
		base.Attack ();
	}
}
