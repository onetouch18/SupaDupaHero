using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Granade : Photon.MonoBehaviour {

	public bool expl = false;
	public int dmg = 30;
	public float radius = 5f;
	public float quality = 1f;

	public GameObject effect;

	private SphereCollider body;
	private MeshRenderer rend;
	private LineRenderer line;
	private List<Vector3> vert;

	private Vector3 newPos;
	private PhotonView view;

	private GameObject eff;

    public Sound ExplosionSound;
    private AudioSource _source;
	private Player owner;

	void Start()
	{
	    _source = GetComponent<AudioSource>();
        view = GetComponent<PhotonView> ();
		body = GetComponent<SphereCollider> ();
		rend = GetComponent<MeshRenderer> ();
		line = GetComponent<LineRenderer> ();

		vert = new List<Vector3>();
		vert.Add (transform.position);
	}

	public void SetOwner(Player player){
		owner = player;
	}
		
	void Update () {
		drawLine ();
			
		if (!view.isMine && PhotonNetwork.connected) {
			transform.position = Vector3.Lerp (transform.position, newPos, Time.deltaTime * 5);
		}
	}

	void drawLine(){
		float lenght = Vector3.Distance (vert [vert.Count - 1], transform.position);

		if (lenght > quality && !body.isTrigger) {
			vert.Add (transform.position);
			line.SetVertexCount (vert.Count);
			line.SetPositions (vert.ToArray());
		}
	}

	void OnCollisionEnter(Collision collision) {
		if (collision.gameObject.tag != GameManager.localPlayer.tag) {
			//expl = true;
			//PhotonView.Destroy (transform.gameObject);
			body.isTrigger = true;

			if (!eff)
				eff = Instantiate (effect, transform.position, transform.rotation) as GameObject;
			eff.transform.localScale = eff.transform.localScale * radius / 20;
			eff.transform.parent = transform;
			Destroy (line);
			Destroy (GetComponent<Rigidbody> ());
			StartCoroutine ("explode");
			if (photonView.isMine || !PhotonNetwork.connected) {
				foreach (Collider c in Physics.OverlapSphere(transform.position, radius)) {
					HitZone zone = c.GetComponent<HitZone> ();
					if (zone != null) {

						if (zone.Controller.GetComponentInChildren<EnemyBase> ().Health - zone.CalculateDamage (dmg) <= 0f &&
						   zone.Controller.GetComponentInChildren<EnemyBase> ().State != States.Die) {
							zone.Controller.GetComponentInChildren<EnemyBase> ().Die ();
							if (GameManager.localPlayer.GetComponent<Player> () == owner) {
								GameManager.score++;
							}
						}
						zone.Hit (GetComponent<PhotonView> (), dmg);
					} else {
						if (c.GetComponent<IHitable> () != null) {
							c.GetComponent<IHitable> ().Hit (this, dmg);
						}
					}
				}
			}
		}
	}

	/*
	void OnTriggerEnter (Collider other){
		if (expl = true) {
			HitZone zone = other.GetComponent<HitZone> ();
			if (zone != null) {
				
				if (zone.Controller.GetComponentInChildren<EnemyBase>().Health - zone.CalculateDamage(dmg) <= 0f && 
					zone.Controller.GetComponentInChildren<EnemyBase>().State != States.Die ) {
					zone.Controller.GetComponentInChildren<EnemyBase> ().Die ();
					if (GameManager.localPlayer.GetComponent<Player>() == owner) {
						GameManager.score++;
					}
				}
				zone.Hit (GetComponent<PhotonView> (), dmg);
				if(zone.GetComponentInParent<BodyPhysicsController> ())
				zone.GetComponentInParent<BodyPhysicsController> ().Fall ();
				zone.GetComponent<Rigidbody> ().AddExplosionForce (700, transform.position, radius);

			}
            else
            {
                var c = other.GetComponent<IHitable>();
                if (c != null)
                {
                    c.Hit(this, dmg);
                }

            }
		}
	}
	*/
		
	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info){
		if (stream.isWriting) {
			stream.SendNext (transform.position);
		} else {
			newPos = (Vector3)stream.ReceiveNext ();
		}
	}

    private float PlaySoundOneShot(Sound sound)
    {
        var so = sound.Clip;
        if (so == null) return 0;
        _source.PlayOneShot(so);
        return so.length;
    }

	IEnumerator explode()
	{
		var duration = PlaySoundOneShot(ExplosionSound);

        Color col = rend.material.color;
		col.a = 0;
		rend.material.color = col;
		ParticleSystem s = eff.GetComponent<ParticleSystem> ();
		Vector3 scl = transform.localScale;

		while (s.time < s.duration) {
		//while(scl.x < radius){
			if (scl.x < radius) {
				scl.x += 1 * Time.deltaTime*20f;
				scl.y += 1 * Time.deltaTime*20f;
				scl.z += 1 * Time.deltaTime*20f;
				transform.localScale = scl;
			}
			yield return null;
		}
       
		Invoke("Destroy",duration);
	}

    private void Destroy()
	{	if (photonView.isMine || !PhotonNetwork.connected) {
			PhotonNetwork.Destroy (this.gameObject);
			Destroy (eff);
		}
    }
}
