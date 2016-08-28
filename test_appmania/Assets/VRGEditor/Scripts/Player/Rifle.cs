using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Rifle : Photon.MonoBehaviour
{

    public float frequency = 10;
    public int bullets = 20;
    public int damage = 30;
	[HideInInspector]
	public int ForceMultiplier = 10;
	//public int SprayLimit = 3;
	public float ReloadTime;
    //float forcePerSecond = 10.0f;
    //float hitSoundVolume = 0.5f;
	[HideInInspector]
    public bool canShoot = true;

    public GameObject muzzleFlashFront;
    public GameObject bulletPrefab;
    public Transform spawnPoint;
    public GameObject blood, sparks, bullet;
	public Text gran, bulletText;


    public int bull;
    private float coneAngle = 1.5f;
    private bool firing = false;
    private float lastFireTime = -1;
    private Quaternion _startRot;
	private Quaternion _calibration;
	private int spray;
	private Animator _anim;
	private bool isCalibrated;
	private bool isReload = false;

    private AudioSource _source;
    public Sound FireSound;
    public Sound ReloadSound;

	protected Vector3 CorrectPos = Vector3.zero;
	protected Quaternion CorrectRot = Quaternion.identity;

    void Awake()
    {	
        _source = GetComponent<AudioSource>();
        _anim = GetComponentInChildren<Animator> ();
        muzzleFlashFront.SetActive(false);
        bull = bullets;
		bulletText.text = bull.ToString() + "/" + bullets.ToString();
        canShoot = true;
        if (spawnPoint == null)
            spawnPoint = transform;
        _startRot = transform.rotation;
        //VRController.Input.Recenter(transform.rotation);
		//transform.parent = GameObject.FindGameObjectWithTag ("RightHand").transform;
    }



    void Update()
	{	
		if (!PhotonNetwork.connected || photonView.isMine) {
			/*
			if (AndroidBT_BLE.IsConnected ()) {
				if (transform.parent != transform.root) {
					transform.parent = transform.root;
				}
				if (!isCalibrated) {
					Invoke ("Calibrate", 2f);
					isCalibrated = true;
					GUIController.ShowLog ("Wait for calibration");
					//transform.rotation = Quaternion.Lerp (transform.rotation, transform.rotation * VRController.Input.Orientation, Time.deltaTime * 9f);
				} else if (_calibration != null) {
					_startRot = transform.root.rotation;
					transform.rotation = _startRot * Quaternion.Inverse (_calibration) * VRController.Input.Orientation; // debug 
					//GUIController.ShowLog (AndroidBT.Acceleration.ToString ());
				}
				if (transform.eulerAngles.x > 30 && transform.eulerAngles.x < 270 && !isReload) {
					isReload = true;
					Invoke ("Reload", ReloadTime);
				}
			} else {
				*/
				transform.forward = Camera.main.transform.forward;
				checkForFire ();
			}

			
		Shoot ();
		SyncUpdate ();
    }


	void Shoot(){
		if (firing && canShoot)
		{
			if (bull > 0 )
			{	
				if (Time.time  > lastFireTime + 1 / frequency /*&& (spray < SprayLimit)*/) {

					GameObject o = Instantiate(bulletPrefab, spawnPoint.position, spawnPoint.rotation) as GameObject;
					o.transform.localScale *= 0.2f;
					Destroy(o, 2f);

					spray++;
					Quaternion coneRandomRotation = Quaternion.Euler (Random.Range (-coneAngle, coneAngle), Random.Range (-coneAngle, coneAngle), 0);


					//_anim.SetTrigger ("Shoot");
					muzzleFlashFront.SetActive(true);
					lastFireTime = Time.time;
					bull--;
					bulletText.text = bull.ToString() + "/" + bullets.ToString();
					PlaySoundOneShot(FireSound);
					RaycastHit hitInfo;

					if (Physics.Raycast(transform.GetChild(0).position, transform.GetChild(0).forward, out hitInfo))
					{   
						GameObject s = Instantiate(sparks, hitInfo.point, Quaternion.identity) as GameObject;
						Destroy(s, 3f);
						s.transform.parent = hitInfo.transform;
						IHitable hitPoint = hitInfo.transform.GetComponent<IHitable>();

						if (hitPoint != null /*&& (photonView.isMine || !PhotonNetwork.connected)*/)
						{	

							if (hitPoint as HitZone) {
								HitEnemy (hitPoint as HitZone, hitInfo);
							} else {
								if(photonView.isMine || !PhotonNetwork.connected)
								hitPoint.Hit (transform.gameObject, damage);
								//GameObject s = Instantiate(sparks, hitInfo.point, Quaternion.LookRotation(hitInfo.normal)) as GameObject;
								//Destroy(s, 1f);
							}
							//GameObject s = Instantiate(sparks, hitInfo.point, Quaternion.identity) as GameObject;
							//Destroy(s, 1f);
						}
						else 
						{
							//GameObject s = Instantiate(sparks, hitInfo.point, Quaternion.LookRotation(hitInfo.normal)) as GameObject;
							//Destroy(s, 1f);
						}
					}
				}
				else muzzleFlashFront.SetActive(false);
			}
			else
			{	
				GUIController.ShowLog ("Reload");
				OnStopFire();
				//if (!AndroidBT_BLE.IsConnected () && !isReload) {
					isReload = true;
					Invoke ("Reload", ReloadTime);
				//}
				/*
				canShot = false;
				PlaySoundOneShot(ReloadSound);
				Invoke("Reload", 1f);
				*/
			}
		}
	}

	void HitEnemy(HitZone hitPoint, RaycastHit hitInfo){
		
		/*
		if (hitPoint.ModifierValue > 0) {
			GameObject b = Instantiate (blood, hitInfo.point, Quaternion.LookRotation (hitInfo.normal)) as GameObject;
			Vector3 ang = b.transform.eulerAngles;
			ang.y -= 90;
			b.transform.eulerAngles = ang;
			Destroy(b, 1f);
		}
		else{  GameObject s = Instantiate(sparks, hitInfo.point, Quaternion.LookRotation(hitInfo.normal)) as GameObject;
			Destroy(s, 1f);
		}
		*/
		if (photonView.isMine || !PhotonNetwork.connected) {
			if (hitPoint.Controller.GetComponentInChildren<EnemyBase>().Health - hitPoint.CalculateDamage(damage) <= 0f && 
				hitPoint.Controller.GetComponentInChildren<EnemyBase>().State != States.Die ) {
				GameManager.score++;
			}
			hitPoint.Hit (GetComponent<PhotonView> (), damage);
		}

		/*
		Vector3 force = hitInfo.transform.position - transform.position;
		force = force.normalized * ForceMultiplier;
		if (hitInfo.transform.GetComponent<PhysicsBodyPart> ())
			hitInfo.transform.GetComponent<PhysicsBodyPart>().ApplyForce(force, hitInfo.point);
		if(hitInfo.transform.GetComponent<Rigidbody>())
		hitInfo.transform.GetComponent<Rigidbody>().AddForceAtPosition(force, hitInfo.point, ForceMode.Impulse);
		*/

	}
	/*
	 void Calibrate(){
			_calibration = VRController.Input.Orientation;
			GUIController.ShowLog ("");
	}
	*/
    void checkForFire()
    {	
		
        if (photonView.isMine || !PhotonNetwork.connected)
        {
			if (Input.GetButton ("Fire1") /*|| VRController.Input.GetButtonDown (VRController.Input.Buttons.A)*/) {
				OnStartFire ();
				if (!GameManager.ready)
					GameManager.localPlayer.GetComponent<Player> ().state = PlayerState.READY;
			}
			else if (!Input.GetButton("Fire1") /*|| VRController.Input.GetButtonUp(VRController.Input.Buttons.A)*/)
                OnStopFire();
			//if(VRController.Input.GetButtonUp(VRController.Input.Buttons.C))
			//	Calibrate();
			
			//VRController.Input.AllButtons [VRController.Input.Buttons.A].SetPreviousState (VRController.Input.AllButtons [VRController.Input.Buttons.A].State);
			//VRController.Input.AllButtons [VRController.Input.Buttons.C].SetPreviousState (VRController.Input.AllButtons [VRController.Input.Buttons.C].State);
        }
    }

    void Reload()
    {	
		PlaySoundOneShot(ReloadSound);
        bull = bullets;
        canShoot = true;
		bulletText.text = bull.ToString() + "/" + bullets.ToString();
		GUIController.ShowLog ("");
		isReload = false;
    }

    [PunRPC]
    void OnStartFire()
	{	
		spray = 0;
        firing = true;
        if (photonView.isMine)
        {
			photonView.RPC("OnStartFire", PhotonTargets.OthersBuffered);
        }
        /*
		if (GetComponent.<AudioSource>())
			GetComponent.<AudioSource>().Play ();
			*/
    }
    [PunRPC]
    void OnStopFire()
    {	
		spray = 0;
        firing = false;
		muzzleFlashFront.SetActive(false);
        if (photonView.isMine)
        {
            photonView.RPC("OnStopFire", PhotonTargets.OthersBuffered);
        }
        /*
		if (GetComponent.<AudioSource>())
			GetComponent.<AudioSource>().Stop ();
			*/
    }

    public void PlaySoundOneShot(Sound sound)
    {
        //if (_source.isPlaying) return;
        var so = sound.Clip;
        if (so == null) return;
        _source.PlayOneShot(so);
    }

	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			//stream.SendNext(state);
			stream.SendNext(transform.position);
			stream.SendNext(transform.rotation);
		}
		else
		{
			//state = (PlayerState)stream.ReceiveNext();
			CorrectPos = (Vector3)stream.ReceiveNext();
			CorrectRot = (Quaternion)stream.ReceiveNext();
		}
	}

	void SyncUpdate()
	{	
		if (PhotonNetwork.connected) {
			if (!photonView.isMine) {
				if (Vector3.Distance (CorrectPos, transform.position) < 4) {
					transform.position = Vector3.Lerp (transform.position, CorrectPos, Time.deltaTime * 10);
					transform.rotation = Quaternion.Lerp (transform.rotation, CorrectRot, Time.deltaTime * 10);
				} else {
					transform.position = CorrectPos;
					transform.rotation = CorrectRot;
				}
			}
		}
	}
}
