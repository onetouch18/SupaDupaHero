using UnityEngine;
using System.Collections;

public class Mob3 : EnemyBase {

	public int SpawnCount;
	public GameObject effect, die;

	public override void UseAbility(Ability a){

		switch (a.Type){
		case SpecialAbilities.ERUCTATE:
			Target.SpecialAbility(a, this);
			Eructate ();
			break;
		}
	}

	private void Eructate(){
		AnimationCompnent.UseAbility();
		GameObject o = Instantiate (effect, transform.position + Vector3.up*1.5f, Quaternion.LookRotation (transform.forward)) as GameObject;
		Vector3 ang = o.transform.eulerAngles;
		ang.y -= 90;
		o.transform.localScale *= 2;
		o.transform.eulerAngles = ang;
		Destroy (o, 1f);
		AnimationCompnent.Ide ();
	}

	public override void Die ()
	{	
		GameObject b = Instantiate (die, transform.position, Quaternion.LookRotation (transform.forward)) as GameObject;
		Vector3 ang = b.transform.eulerAngles;
		ang.y -= 90;
		b.transform.localScale *= 2;
		b.transform.eulerAngles = ang;
		Destroy (b, 1f);
		/*
		if (State != States.Die) {
			GameObject o;
			for (int i = 0; i < SpawnCount; i++) {
				float r = Random.value * 2f;
				r *= Random.Range (-1, 1);
				Vector3 pos = Agent.transform.position;
				pos.x += r;
				pos.z += r;

				if (PhotonNetwork.connected) {
					o = PhotonNetwork.Instantiate ("Mob5", pos, Quaternion.identity, 0) as GameObject;
				} else
					o = Instantiate (Resources.Load ("Mob5"), pos, Quaternion.identity) as GameObject;
				o.transform.parent = Agent.transform.parent;
			}
		}
		*/
		base.Die ();
	}
}
