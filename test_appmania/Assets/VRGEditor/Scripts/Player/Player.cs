using System;
using UnityEngine;
using System.Collections;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using UnityEngine.Events;

public enum PlayerState
{
    IDLE,
    READY,
    ATTACKED,
    BITED,
    RELOAD,
	DIE,
	WAIT,
	FINISH
}

public class Player : Photon.MonoBehaviour
{

    public float hp;
	[HideInInspector]
    public float bombAngle = 2f;
	[HideInInspector]
    public PlayerState state = PlayerState.IDLE;
	public float TurnSpeed = 1f;
	[HideInInspector]
	public int GranadeReloadTime;

    [HideInInspector]
    public Vector3 checkPoint;

    private UnityAction Action = () => { };
    private NavMeshAgent _agent;
    private Animator _animator;
    private Quaternion rec;
    private GUIController gui;
    private Rifle _rifle;
    private EnemyBase enemy;
    private float _currentHP;
	private Vector3 direction;
	[HideInInspector]
    public int granades = 1;
	[HideInInspector]
	public int MaxGranades = 3;
	[HideInInspector]
    public Fader BloodScreen;

    #region Capture Fields
   // [Header("Capture fields")]
	[HideInInspector]
    public int CaptureWaveCount = 4;
	[HideInInspector]
    public float CaptureWaveAmplitude; //Now disabled
	[HideInInspector]
    public int CaptureStartWaveCount = 2;
	[HideInInspector]
    public float CaptureDecreaseTime = 1F;
	[HideInInspector]
    public float CaptureThrowDistance;
	[HideInInspector]
    public int CaptureCurrentWaveCount { get; private set; }
    public bool IsCaptured { get; private set; }
    private Coroutine _decreaser;
    [Header("")]
    #endregion

    #region Network Fields
    protected Vector3 CorrectPlayerPos = Vector3.zero;
    protected Quaternion CorrectPlayerRot = Quaternion.identity;
    private HealthBar health;
    #endregion
	[HideInInspector]
    public Sound GetDamageSound; //
	[HideInInspector]
    public Sound DieSound;  //
	[HideInInspector]
    public Sound WalkingSound;
	[HideInInspector]
    private AudioSource _source;
	[HideInInspector]
    public Transform Head;
	private Quaternion _look;
	private PathRenderer _pathRend;
	private int _dies;
	public event Action OnAction;
	//public int number;

    private void Start()
    {
        _source = GetComponent<AudioSource>();
        var cam = GameObject.FindGameObjectWithTag("vrCam");
        health = cam.GetComponentInChildren<HealthBar>();
        BloodScreen = cam.GetComponentInChildren<Fader>();
        gui = cam.GetComponentInChildren<GUIController>();
		_rifle = GetComponentInChildren<Rifle>();
		if (GameManager.IsTutor) {
			granades = 0;
			_rifle.canShoot = false;
			_rifle.gran.text = "G:N";
		} else {
			_rifle.gran.text = "G:" + granades.ToString();
		}
      
        _agent = GetComponent<NavMeshAgent>();
		_agent.angularSpeed /= TurnSpeed;
        _animator = GetComponent<Animator>();
        SwipeDetector.Instance.OnSwipe.AddListener(OnSwipe);
        _currentHP = hp;
        gui.SetHP(_currentHP, hp);
		IsCaptured = false;
		_pathRend = GetComponent<PathRenderer> ();
		//_agent.updateRotation = false;
		_agent.autoBraking = false;
		Welcome ();

    }
	/*
	void OnCollisionEnter(Collision collision){
		Item i = collision.transform.GetComponent<Item> ();
		if (i)
			_rifle.HitItem (i);
	}
	*/
	IEnumerator ReloadGranade(){
		int i = 0;
		while (i < GranadeReloadTime) {
			_rifle.gran.text = "G:R" + (GranadeReloadTime - i).ToString ();
			i++;
			yield return new WaitForSeconds (1f);
		}
		_rifle.gran.text = "G:" + granades.ToString() ;
		granades++;
	}

    private void Update()
    {	
		gui.setBullets (_rifle.bull, _rifle.bullets);
        Action();
		if (PhotonNetwork.connected)
			SyncUpdate ();
		if (photonView.isMine || !PhotonNetwork.connected) {
			if (IsCaptured) {	
				gui.UpdateSlider ((float)CaptureCurrentWaveCount / CaptureWaveCount);
				_rifle.canShoot = false;

				if (IsCaptured) {
					gui.UpdateSlider ((float)CaptureCurrentWaveCount / CaptureWaveCount);

					if (CaptureCurrentWaveCount <= 0) {
						CaptureCurrentWaveCount = 0;
						Die ();
						enemy.Reset ();
					} else if (CaptureCurrentWaveCount >= CaptureWaveCount || enemy.State == States.Die) {
						Dismiss ();
					}
				}
			}
			/*
			if (_rifle.canShoot && ((Input.GetKeyDown (KeyCode.Space) | VRController.Input.GetButtonDown (VRController.Input.Buttons.B)) && (granades > 0 && (photonView.isMine || !PhotonNetwork.connected)))) {
				ThrowGranade ();
			}
			VRController.Input.AllButtons [VRController.Input.Buttons.B].SetPreviousState (VRController.Input.AllButtons [VRController.Input.Buttons.B].State);
			*/
			if (Input.GetKeyDown (KeyCode.R))
				Reload ();
			else if (Input.GetKeyDown (KeyCode.LeftAlt))
				++CaptureCurrentWaveCount;
		}
    }


    public void PlayerAttacked(int damage)
    {
        //state = PlayerState.ATTACKED;
		if (!PhotonNetwork.connected || photonView.isMine) {
			OnDamage (damage);
			if(PhotonNetwork.connected)
			photonView.RPC ("OnDamage", PhotonTargets.OthersBuffered, damage);
		}
    }


    public void Capture()
    {
        CaptureCurrentWaveCount = CaptureStartWaveCount;
        IsCaptured = true;
		if (photonView.isMine || !PhotonNetwork.connected) {
			_decreaser = Timer.StartTimer (this, CaptureDecreaseTime, x => --x.CaptureCurrentWaveCount);
			gui.SetSlader((float)CaptureCurrentWaveCount / CaptureWaveCount);
		}
    }

   [PunRPC]
    private void Dismiss()
    {
        IsCaptured = false;
        _rifle.canShoot = true;
		StopCoroutine ("RotateTo");
		StartCoroutine ("RotateTo", direction);
		if (photonView.isMine || !PhotonNetwork.connected) {
			photonView.RPC ("Dismiss", PhotonTargets.OthersBuffered);
			enemy.Dismiss ();
			enemy.Reset ();
			gui.DeactivateSlider ();
			if (GameManager.IsTutor) {
				enemy.ClosenessBuildup = 100;
				enemy._closenessLevel = 0;
			}
			StopCoroutine (_decreaser);
		}
    }

    private void OnSwipe(SwipeDetector.SwipeDirection direction)
    {
        // ReSharper disable once SwitchStatementMissingSomeCases
        switch (direction)
        {

            case SwipeDetector.SwipeDirection.Up:
                ThrowGranade();
                break;
            case SwipeDetector.SwipeDirection.Down:
                Reload();
                break;
            case SwipeDetector.SwipeDirection.Right:

                if (IsCaptured)
                {

                    gui.UpdateSlider((float)CaptureCurrentWaveCount / CaptureWaveCount);

                }
                break;
            case SwipeDetector.SwipeDirection.Left:
                if (IsCaptured)
                {
                    ++CaptureCurrentWaveCount;

                }
                break;
        }

    }

    void ThrowGranade()
    {
            if (granades > 0)
            {
				GameObject granade;
                if (PhotonNetwork.connected)
					granade = PhotonNetwork.Instantiate("Granade", _rifle.spawnPoint.position, Quaternion.identity, 0);
                else
					granade = Instantiate(Resources.Load("Granade"),  _rifle.spawnPoint.position, Quaternion.identity) as GameObject;
               // Vector3 vel = GameObject.FindGameObjectWithTag("vrCam").transform.GetChild(0).forward;
				Vector3 vel = _rifle.transform.forward;
                vel.y += bombAngle;
                vel.Normalize();
                vel.Scale(new Vector3(10, 10, 10));
                granade.GetComponent<Rigidbody>().velocity = vel;
				granade.GetComponent<Granade> ().SetOwner (this);
				if (!GameManager.God) {
					granades--;
					_rifle.gran.text = "G:" + granades.ToString() ;
					//if(granades < 1)
					//StartCoroutine ("ReloadGranade");
				}
           // }
        }
    }

    void Reload()
    {	
		if (!IsCaptured) {
			_rifle.canShoot = false;
			_rifle.Invoke ("Reload", _rifle.ReloadTime);
		}
    }

	[PunRPC]
    public void OnDamage(int damage)
    {	
		if (!GameManager.IsTutor && !GameManager.God)
        {
            _currentHP -= damage;

			if (_currentHP <= 0 && state != PlayerState.DIE)
            {
                Die();
				if(PhotonNetwork.connected)
				photonView.RPC ("Die", PhotonTargets.OthersBuffered);
            }
            else
            {
                PlaySoundOneShot(GetDamageSound);
            }
        }
		if (photonView.isMine | !PhotonNetwork.connected)
		{
			//BloodScreen.FadeIn(0.5F);
			gui.UpdateHP(_currentHP,hp);
		}
    }

    [PunRPC]
    void Die()
	{	
		
		//if (granades < 1) {
			//granades = 1;
			//_rifle.gran.text = "G:1";
		//}
		state = PlayerState.DIE;
		_rifle.bull = _rifle.bullets;
		if (!GameManager.IsTutor) {
            PlaySoundOneShot(DieSound);
			_currentHP = hp;
			if (photonView.isMine || !PhotonNetwork.connected) {
				gui.health.gameObject.SetActive (false);
				gui.DeactivateSlider ();
				StopAllCoroutines ();
			}
			IsCaptured = false;
			_rifle.canShoot = false;
			//_agent.ResetPath ();
			//_agent.enabled = false;
			_dies++;
			//if (photonView.isMine) {
				//photonView.RPC ("Die", PhotonTargets.OthersBuffered);
			//}
		}
    }

	private bool wait = false;

	public void CheckDie(bool isAll, float time){
		
		if (isAll && !wait) {
			wait = true;
			StartCoroutine (FadeIn(time));
			StartCoroutine (DieTime (time));
		} else if (state != PlayerState.WAIT && !isAll) {
			StartCoroutine (FadeIn(time));
			GUIController.ShowLog ("Wait for other players.");
			state = PlayerState.WAIT;
		}
	}

	private Vector3 curve;

	IEnumerator DieTime(float time){
		float t = time;
		//state = PlayerState.WAIT;
		while (t > 0) {
			GUIController.ShowLog ("You are die!" + "\n" + "Wait for" + " " + t + " " + "seconds");
			t--;
			yield return new WaitForSeconds (1f);
		}
		//transform.position = checkPoint;
		wait = false;
		_agent.Warp (checkPoint);
		if (curve != Vector3.zero)
			transform.forward = 2 * transform.position - new Vector3 (curve.x, transform.forward.y, curve.z);
		else if(direction != Vector3.zero)
			transform.LookAt (new Vector3 (direction.x, transform.position.y, direction.z));
		OnAction ();
		GUIController.ShowLog("");
		_currentHP = hp;
		state = PlayerState.READY;
		gui.health.gameObject.SetActive (true);
		gui.UpdateHP(_currentHP, hp ,1.5F);
		_rifle.canShoot = true;
		StartCoroutine (FadeOut(time/2f));
	}

	public void Resp(){
		GUIController.ShowLog("");
		_currentHP = hp;
		gui.health.gameObject.SetActive (true);
		gui.UpdateHP(_currentHP, hp ,1.5F);
		_rifle.canShoot = true;
		state = PlayerState.READY;
		StartCoroutine (FadeOut(1f));
	}

	IEnumerator FadeIn(float time){
		UnityStandardAssets.ImageEffects.Grayscale g = gui.GetComponentInParent<UnityStandardAssets.ImageEffects.Grayscale> ();
		g.enabled = true;
		g.rampOffset = 0;
		while (g.rampOffset > -0.5f) {
			g.rampOffset = Mathf.Lerp (g.rampOffset, -0.75f, Time.deltaTime/time);
			yield return null;
		}
	}

	IEnumerator FadeOut(float time){
		UnityStandardAssets.ImageEffects.Grayscale g = gui.GetComponentInParent<UnityStandardAssets.ImageEffects.Grayscale> ();
		g.enabled = true;
		while (g.rampOffset < 0f) {
			g.rampOffset = Mathf.Lerp (g.rampOffset, 0.5f, Time.deltaTime/time);
			yield return null;
		}
		g.enabled = false;
		GUIController.ShowLog("");
	}

    [PunRPC]
    public void MoveTo(Vector3 point)
	{	
		_agent.enabled = true;
		_agent.ResetPath ();
        _agent.SetDestination(point);
        if (photonView.isMine)
        {
            photonView.RPC("MoveTo", PhotonTargets.OthersBuffered, point);
        }
       // PlaySound(WalkingSound, true);
        Action = () =>
        {
            var speed = Vector3.Project(_agent.desiredVelocity, transform.forward).magnitude;
            speed = speed < 1 ? 0 : 1;
           // _animator.SetFloat("Speed", speed);
            if (transform.position.AlmostEquals(point, 0f))
            {
                Reset();
            }
        };
    }

	float distance = 1f;

	public IEnumerator MoveByCurve(MGCurve curve){
		//distance -= Time.deltaTime;
		//_agent.SetDestination (curve.GetPoint (distance));
		//_agent.updateRotation = false;
		this.curve = curve.GetDirection(1f);
		_agent.enabled = true;
		_agent.ResetPath ();
		distance = 1f;
		_pathRend.RenderPath(curve);
		_agent.SetDestination (curve.GetPoint (distance));
		while(distance > 0){
			if (transform.position.AlmostEquals (curve.GetPoint (distance), 0.5f)) {
				distance -= Time.deltaTime/_agent.speed;
				_agent.SetDestination (curve.GetPoint (distance));
				//Vector3 dir =  curve.GetDirection (distance) - transform.position;
				//transform.rotation = Quaternion.Lerp (transform.rotation, Quaternion.LookRotation (dir), Time.deltaTime);

				//transform.LookAt (dir);
			}
			yield return null;

		}
		Reset ();
		distance = 1f;
	}

    [PunRPC]
    public void Reset()
    {
        //CaptureCurrentWaveCount = CaptureStartWaveCount;
        // IsCaptured = false;
		//_agent.Stop();
		curve = Vector3.zero;
		direction = Vector3.zero;
        StopSound();
        //gui.SetHP(_currentHP, hp);
        //_animator.SetFloat("Speed", 0);
        state = PlayerState.IDLE;
        Action = () => { };
        if (photonView.isMine) photonView.RPC("Reset", PhotonTargets.OthersBuffered);
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(state);
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            state = (PlayerState)stream.ReceiveNext();
            CorrectPlayerPos = (Vector3)stream.ReceiveNext();
            CorrectPlayerRot = (Quaternion)stream.ReceiveNext();
        }
    }

    public void SpecialAbility(Ability ability, EnemyBase enemy)
    {
        this.enemy = enemy;
		switch (ability.Type)
        {
            case SpecialAbilities.BITE:
                Bited();
                break;
		case SpecialAbilities.ERUCTATE:
			OnDamage (ability.Damage);
			break;
        }
    }

    [PunRPC]
    void Bited()
    {	
		if (!GameManager.God) {
			StopCoroutine ("RotateTo");
			StartCoroutine ("RotateTo", enemy.transform.position);
			_rifle.canShoot = false;
			Capture ();
			if (photonView.isMine) {	
				photonView.RPC ("Bited", PhotonTargets.OthersBuffered);
			}
		}
    }
	/*
	public void UseItem(ItemType type, int value){
		switch (type) {
		case ItemType.GRANADE:
			if (granades < MaxGranades) {
				granades += value;
				_rifle.gran.text = "G:" + granades.ToString ();
			}
			break;
		case ItemType.HEALTH:
			if (_currentHP + value >= hp) {
				_currentHP = hp;
			}else
			_currentHP += value;
			gui.UpdateHP(_currentHP,hp);
			break;
		}
			
	}
	*/
    void SyncUpdate()
    {
        if (!photonView.isMine)
        {
            if (Vector3.Distance(CorrectPlayerPos, transform.position) < 4)
            {
                transform.position = Vector3.Lerp(transform.position, CorrectPlayerPos, Time.deltaTime * 5);
                transform.rotation = Quaternion.Lerp(transform.rotation, CorrectPlayerRot, Time.deltaTime * 5);
            }
            else
            {
                transform.position = CorrectPlayerPos;
                transform.rotation = CorrectPlayerRot;
            }
        }
    }

	public void SetDirection(Vector3 direction){
		StartCoroutine ("RotateTo", direction);
		this.direction = direction;

	}

	public IEnumerator RotateTo(Vector3 direction){
		Vector3 vec =  transform.position;
		direction.y = transform.position.y;
		Quaternion r = Quaternion.LookRotation (direction - vec);
		this.direction = direction;

		while (!transform.rotation.AlmostEquals(r, 5)) {
			transform.rotation = Quaternion.Lerp(transform.rotation, r, Time.deltaTime * TurnSpeed);
			yield return null;
		}

	}

    public void PlaySoundOneShot(Sound sound)
    {
       // if (_source.isPlaying) return;
       // var so = sound.Clip;
       // if (so == null) return;
       // _source.PlayOneShot(so);
    }
	/*
    public void PlaySound(Sound sound, bool isLoop = false)
    {
        if (_source.isPlaying) return;
        var so = sound.Clip;
        if (so == null) return;
        _source.loop = isLoop;
        _source.clip = so;
        _source.Play();
    }
	*/
    public void StopSound()
    {
        //if (!_source.isPlaying) return;
        //_source.Stop();
    }

	void Welcome(){
		if (!PhotonNetwork.connected || photonView.isMine) {
			UnityStandardAssets.ImageEffects.Grayscale g = gui.GetComponentInParent<UnityStandardAssets.ImageEffects.Grayscale> ();
			g.enabled = true;
			g.rampOffset = -0.75f;
			GUIController.ShowLog ("Welcome to chapter "+ GameManager.Chapter);
			StartCoroutine (FadeOut (4f));
		}
	}

	public void Finish(){
		GUIController.ShowLog ("Congratulations, you finish" + "\n" + 
			"Chapter " + GameManager.Chapter + "\n" + 
			"Killed mobs:" + GameManager.score + "\n" + 
			"Death:" + _dies);
		StartCoroutine (FadeIn (4f));
	}
}
