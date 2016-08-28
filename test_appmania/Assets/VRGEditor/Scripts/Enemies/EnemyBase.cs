
using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using Random = UnityEngine.Random;


public class EnemyBase : Photon.MonoBehaviour
{
    public float Health;
    public float Speed;
    public float RunSpeed = 1.5f;
    public int AttackDmg;
    public float AttackSpeed;
    public float AttackRadius;
    public float AttackDistance;
    public AttackType attackType;

    public float DieRigTime = 2f;
    public float ParticleTime = 2f;

    public Ability[] Abilities;

    public States State { get { return StateMachine.CurrentState.State; } }

    protected HitController _hitController;
    public NavMeshAgent Agent;
    public EnemyAnimation AnimationCompnent;

    #region Network Fields
    protected Vector3 CorrectPlayerPos = Vector3.zero;
    protected Quaternion CorrectPlayerRot = Quaternion.identity;
    #endregion

    public Player Target;

    public float _closenessLevel = 0;
    public bool _isHaveTarget;
    public float ClosenessBuildup = 4;
    public float ClosenesDistance = 2;
   // public DeathParticle _deathParticle;

    public StateMachine StateMachine;

    public NavMeshObstacle obstacle;

    public Sound GetDamageSound; //
    public Sound DieSound;  //
    public Sound AttackSound; //
    public Sound AbilitySound; //
    public Sound WalkingSound;
    private AudioSource _source;

    public Transform Head;
    public float maxAngle = 60;
    private float _rotatioSpeed = 1;

   // private Shadow _shadow;
    [HideInInspector]
    public UnityEvent AttackEvent;

    [HideInInspector]
    //public HittableGlass Glass;
    public bool granaded = false;

    /// <summary>
    /// For animation state.
    /// </summary>
    [HideInInspector] public bool IsMirror;
    [HideInInspector] public Animations Animation;

    /// <summary>
    /// Crawling
    /// </summary>
    [HideInInspector] public float StateExitTime;
    public void Reset()
    {
        //if (State != States.Die)
        //State = States.Idle;
        _isHaveTarget = false;
        if (Agent.enabled)
            Agent.ResetPath();
    }

    void Start()
    {
        Health += Health * 0.5f * (GameManager.players.Count - 1);
        AttackDmg += (int)(AttackDmg * 0.05f * (GameManager.players.Count - 1));

    }

    void Awake()
    {
		//Init (States.Idle);
    }


    [PunRPC]
    public virtual void Init(States StartState)
    {
        _source = GetComponentInParent<AudioSource>();

        Agent = transform.parent.GetComponent<NavMeshAgent>();
       // _shadow = GetComponentInParent<Shadow>();
        Agent.speed = Speed;
        Agent.stoppingDistance = AttackDistance;
        GetComponent<SphereCollider>().radius = AttackRadius;
        AnimationCompnent = GetComponent<EnemyAnimation>();
        _hitController = GetComponentInParent<HitController>();
       	_hitController.OnDamage += OnDamage;
        obstacle = GetComponentInParent<NavMeshObstacle>();
        //State = States.Idle;
        if (StateMachine.CurrentState == null)
            StateMachine.Configure(this, StateFactory.GetState(StartState));
		if (photonView.isMine) 
		{	
			//_hitController.OnDamage += OnDamage;
            photonView.RPC("Init", PhotonTargets.OthersBuffered, StartState);
        }
    }

    protected void OnDestroy()
    {
        if (_hitController != null)
        {
            _hitController.OnDamage -= OnDamage;
        }

    }
	[PunRPC]
    public void OnDamage(float damage)
    {
        Health -= damage;
        if (Health <= 0)
        {
            Die();
        }
        else
        {
            PlaySoundOneShot(GetDamageSound);

            // _animationCompnent.TakeDmg();
        }
    }

    protected virtual void OnDamage(object sender, DamageEventArgs damageEventArgs)
    {
        // Debug.Log(sender + " " + damageEventArgs);
      	 OnDamage(damageEventArgs.Damage);
		if (PhotonNetwork.connected)
        {
		photonView.RPC("OnDamage", PhotonTargets.OthersBuffered, damageEventArgs.Damage);
        }
        
    }

    protected virtual void SetStateIdle()
    {
        //State = States.Idle;
        _isHaveTarget = false;
        AnimationCompnent.Ide();
        //Behavior = () => { };
    }

    //Call in animation
    protected virtual void ExitFromState()
    {
        StateMachine.CurrentState.Exit();
    }

    [PunRPC]
    public virtual void UseAbility(Ability a)
    {
        switch (a.Type)
        {
            case SpecialAbilities.BITE:
                Bite();
                Target.SpecialAbility(a, this);
                break;
        }

        if (photonView.isMine && PhotonNetwork.connected)
        {
            photonView.RPC("UseAbility", PhotonTargets.OthersBuffered, a);
        }
    }

    protected virtual void Bite()
    {
        PlaySoundOneShot(AbilitySound);
        AnimationCompnent.UseAbility();
    }

    protected virtual void OnTriggerStay(Collider collider)
    {
		if (!_isHaveTarget && State != States.Die) {
			if (collider.tag != "Player")
				return;
			_isHaveTarget = true;
			if (photonView.isMine) {
				if (attackType == AttackType.MELEE) {
					photonView.RPC("SetTarget", PhotonTargets.AllBuffered, GetClosestTarget().photonView.viewID);
				} else
					photonView.RPC("SetTarget", PhotonTargets.AllBuffered, GetHPTarget().photonView.viewID);
			} else if (!PhotonNetwork.connected) {
				if (attackType == AttackType.MELEE) {
				Target = GetClosestTarget ();
				} else
				Target = GetHPTarget ();
			}
		}
        //}
    }

    [PunRPC]
    public Player GetHPTarget()
    {
        Player maxHP = null;

        float hp = 0;
        foreach (Transform tra in GameManager.players)
        {
            if (tra == null)
                continue;
            Player traHP = tra.GetComponent<Player>();
            if (traHP.state != PlayerState.DIE && traHP.state != PlayerState.WAIT)
            {
                if (traHP.hp > hp /*&& traHP.state != PlayerState.ATTACKED*/)
                {
                    hp = traHP.hp;
                    maxHP = traHP;
                }
            }
        }
        return maxHP;
    }

    [PunRPC]
	private void SetTarget(int id)
    {
		Target = PhotonView.Find (id).GetComponent<Player> ();
    }

    public Player GetClosestTarget()
    {
        //Vector3 k = new Vector3();
        Transform close = null;
        float dist = -1;
        foreach (Transform tra in GameManager.players)
        {
            //Debug.Log (tra.GetComponent<Player> ().state);
            if (tra == null)
            {
                continue;
            }
            float thisD = Vector3.Distance(tra.position, transform.position);
            if (dist == -1 || thisD < dist)
            {
                if (tra.GetComponent<Player>().state != PlayerState.ATTACKED && tra.GetComponent<Player>().state != PlayerState.WAIT &&
                    tra.GetComponent<Player>().state != PlayerState.DIE)
                {
                    dist = thisD;
                    close = tra;
                }
            }
        }
        return close.GetComponent<Player>();
    }

    [PunRPC]
    public virtual void Attack()
    {
        AnimationCompnent.Attack();
		AttackDelay ();
        PlaySoundOneShot(AttackSound);
        //Attack player call in Animation event
        if (photonView.isMine)
        {
            photonView.RPC("Attack", PhotonTargets.OthersBuffered);
        }
    }
    [HideInInspector]
    public bool broked = false;
    //Call in animation
    protected void AttackDelay()
    {	
		//if (Glass == null) {
		//	broked = true;
		//}
        if (State == States.Die) return;
		//if (!broked)
			//AttackEvent.Invoke ();
    	// else
			Target.PlayerAttacked(AttackDmg);

    }


    protected void Jump()
    {
        //animation.Jump ();
    }

    [PunRPC]
    public virtual void Die()
    {
        if (State != States.Die)
        {
            StateMachine.ChangeState(new DieState());
            PlaySoundOneShot(DieSound);
            Agent.transform.parent = null;
			StartCoroutine (Fade ());
            //_shadow.Enabled(false);
            //Reset();
            //GetComponentInParent<BodyPhysicsController>().Fall();
            ////State = States.Die;
            //Invoke("DieParticle", 1.2f);
            //Invoke("Destroy", 3f);
            // _animationCompnent.Die();
        }
        if (photonView.isMine)
        {
            photonView.RPC("Die", PhotonTargets.OthersBuffered);
        }
    }

	IEnumerator Fade(){
		Color c = transform.GetChild (0).GetComponent<MeshRenderer> ().material.color;
		while (c.a > 0) {
			c.a -= Time.deltaTime;
			transform.GetChild (0).GetComponent<MeshRenderer> ().material.color = c;
			yield return null;
		}
	}
  //  public void DieParticle()
   // {
        //if (_deathParticle)
         //   _deathParticle.Play();
   // }

    [PunRPC]
    public void Dismiss()
    {
        AnimationCompnent.TakeDmg();
        if (photonView.isMine)
        {
            photonView.RPC("Dismiss", PhotonTargets.OthersBuffered);
        }
    }

    protected void Destroy()
    {
		if (PhotonNetwork.connected && PhotonNetwork.isMasterClient)
        {
            PhotonNetwork.Destroy(transform.parent.gameObject);
        }
        else Destroy(transform.parent.gameObject);
    }

    protected virtual void SyncUpdate()
    {
        // if (!photonView.isMine)
        // {
        if (CorrectPlayerPos == Vector3.zero)
            return;

        //Update remote player (smooth this, this looks good, at the cost of some accuracy)
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
        // }
    }

    protected virtual void Update()
    {
        SyncUpdate();
        StateMachine.Behavior();
        HeadTrack();
        if (_isHaveTarget)
            RotateTowards(Target.transform);

        if (State == States.Walk && !_isWalkingSound && !_source.isPlaying)
        {
            Invoke("PlayWalkingSound", Random.Range(_walkSoundRange.x, _walkSoundRange.y));
        }
        ///////// Behavior();
    }


    //void OnAnimatorIK()
    //{
    //    if (State != States.Walk || !AnimationCompnent.AnimatorComponent || !Target || !Target.Head) return;

    //    AnimationCompnent.AnimatorComponent.SetLookAtWeight(1);
    //    AnimationCompnent.AnimatorComponent.SetLookAtPosition(Target.Head.transform.position);

    //}

    private void HeadTrack()
    {

        if (State != States.Walk || Target == null || Head == null) return;

        Vector3 look = Target.Head.transform.position - Head.transform.position;

        Quaternion q = Quaternion.LookRotation(look);

        //Debug.LogError(Quaternion.Angle(q, transform.rotation));
        if (Quaternion.Angle(q, transform.rotation) > maxAngle)
            return;
        //targetRotation = q;

        Head.transform.rotation = q; // Quaternion.Slerp(Head.transform.rotation, q, Time.deltaTime * 5);


        //Head.transform.LookAt(Target.Head.transform.position);// = Quaternion.LookRotation();
    }



    private void RotateTowards(Transform target)
    {
        if (State != States.Attack)
        {
            return;
        }

        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        lookRotation.x = 0F;
        lookRotation.z = 0F;
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * _rotatioSpeed);
    }

    protected virtual void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            //We own this player: send the others our data
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            //stream.SendNext(Health);
            //stream.SendNext (Target.name);
            //stream.SendNext(State);
        }
        else
        {
            //Network player, receive data	
            CorrectPlayerPos = (Vector3)stream.ReceiveNext();
            CorrectPlayerRot = (Quaternion)stream.ReceiveNext();
           // Health = (float)stream.ReceiveNext();
            //S
            //Target = GameManager.players.(Player) stream.ReceiveNext();
        }
    }
    public void PlaySoundOneShot(Sound sound)
    {
        if (_source.isPlaying && !_isWalkingSound) return;
        if (_source.isPlaying) StopSound();
        var so = sound.Clip;
        if (so == null) return;
        _source.PlayOneShot(so);
        _isWalkingSound = false;
        CancelInvoke("PlayWalkingSound");
    }

    private bool _isWalkingSound;
    [SerializeField]
    private Vector2 _walkSoundRange;
    [SerializeField]
    [Range(0, 100)]
    private float _RandomPlayValue = 40;
    internal float Delay;

    private void PlayWalkingSound()
    {
        PlaySound(WalkingSound);
    }

    private void WalkingSoundStop()
    {
        _isWalkingSound = false;
    }

    public void PlaySound(Sound sound, bool isLoop = false)
    {
        if (_source.isPlaying) return;
        var so = sound.Clip;
        if (so == null) return;
        _source.loop = isLoop;
        _source.clip = so;
        _isWalkingSound = true;
        CancelInvoke("PlayWalkingSound");
        Invoke("WalkingSoundStop", _source.clip.length);

        if (Random.value <= _RandomPlayValue / 100)
        {/*Debug.LogError(1);*/ _source.Play(); }
    }
    public void StopSound()
    {
        if (!_source.isPlaying) return;
        _source.Stop();
        _isWalkingSound = false;
    }
}

