using UnityEngine;

public class EnemyAnimation : MonoBehaviour
{
    public string[] AtackAnimations;

    public Animator AnimatorComponent { get { return _animator; } }
    private Animator _animator;
    private float _blendAnim;
    private NavMeshAgent _navMeshAg;
    private float _rnd;
    private AnimationClip clip;
    private const float AnimatorMaxBlenValue = 2.5F;
    private Quaternion _look;


    Vector2 smoothDeltaPosition = Vector2.zero;
    Vector2 velocity = Vector2.zero;
    protected virtual void Update()
    {
        Move();
        //Vector3 worldDeltaPosition = _navMeshAg.nextPosition - transform.position;
        //// Map 'worldDeltaPosition' to local space
        //float dx = Vector3.Dot(transform.right, worldDeltaPosition);
        //float dy = Vector3.Dot(transform.forward, worldDeltaPosition);
        //Vector2 deltaPosition = new Vector2(dx, dy);

        //// Low-pass filter the deltaMove
        //float smooth = Mathf.Min(1.0f, Time.deltaTime / 0.15f);
        //smoothDeltaPosition = Vector2.Lerp(smoothDeltaPosition, deltaPosition, smooth);

        //// Update velocity if time advances
        //if (Time.deltaTime > 1e-5f)
        //    velocity = smoothDeltaPosition / Time.deltaTime;

        ////_animator.SetFloat("MovSpeed", velocity.y);
        //bool shouldMove = velocity.magnitude > 0.5f && _navMeshAg.remainingDistance > _navMeshAg.radius;
        //GetComponent<LookAt>().lookAtTargetPosition = _navMeshAg.steeringTarget + transform.forward;
    }
	/*
    private void LateUpdate()
    {

        ////// _navMeshAg.updateRotation = false;
        ////// Vector3 velocity = Quaternion.Inverse(transform.rotation) * _navMeshAg.desiredVelocity;
        //////var rot =  Quaternion.LookRotation(_navMeshAg.desiredVelocity);
        ////// float angle = Mathf.Atan2(velocity.x, velocity.z) * 180.0f / 3.14159f;
        ////// Debug.LogError(velocity + " " + angle);

        ////// transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime);
        //    Vector3 direction = (_navMeshAg.nextPosition - _navMeshAg.transform.position).normalized;
        //    Debug.LogError(direction);
        //    Quaternion lookRotation = Quaternion.LookRotation(direction, Vector3.up);
        //    Quaternion targetRotation = Quaternion.Slerp(_navMeshAg.transform.rotation, lookRotation, 10);
        //    targetRotation.x = 0;
        //    targetRotation.z = 0;
        //    _navMeshAg.transform.rotation = targetRotation;
		///*

		_navMeshAg.updateRotation = true;
		//_owner.Agent.updatePosition = true;
		//prevPos = _owner.AnimationCompnent.AnimatorComponent.bodyPosition;
		//_owner.Agent.transform.position = _owner.AnimationCompnent.AnimatorComponent.bodyPosition;
		//velocity = AnimatorComponent.deltaPosition / Time.deltaTime;
		Vector3 position = AnimatorComponent.bodyPosition;
		position.y = _navMeshAg.nextPosition.y;
		//_owner.transform.position = position;
		Vector3 direction =_navMeshAg.steeringTarget - transform.position;
		// Debug.LogError(direction);
		direction.y = 0.0f;
		if (direction.magnitude > 0.1f)
		{
			Quaternion qLook = Quaternion.LookRotation(direction, Vector3.up);
			_look = qLook;
		}
		//transform.parent.position += AnimatorComponent.deltaPosition;
		transform.rotation = Quaternion.Slerp(transform.rotation, _look, Time.deltaTime / 0.2f);
		//_owner.Agent.transform.position = prevPos;
		_navMeshAg.Warp(Vector3.zero);
		//_owner.transform.position = _owner.Agent.transform.position;
		//_owner.transform.localPosition = Vector3.zero;
		Debug.Log (AnimatorComponent.bodyPosition);
		/*
		_navMeshAg.velocity = AnimatorComponent.deltaPosition / Time.deltaTime;

		Vector3 direction = _navMeshAg.steeringTarget - transform.position;
		direction.y = 0.0f;
		if (direction.magnitude > 0.1f)
		{
			Quaternion qLook = Quaternion.LookRotation(direction, Vector3.up);
			_look = qLook;
		}
		transform.rotation = Quaternion.Slerp(transform.rotation, _look, Time.deltaTime / 0.2f);
		*/
  //  }

    void Awake()
    {
        _animator = GetComponent<Animator>();
        _navMeshAg = GetComponentInParent<NavMeshAgent>();
       // _navMeshAg.updatePosition = false;
        _animator.runtimeAnimatorController = Resources.Load("Controllers/Controller" + Mathf.FloorToInt(Random.Range(0, 2))) as RuntimeAnimatorController;
        _rnd = Random.Range(8f, 10f) / 10f;
    }
	public void StartCrawling(){
		//_animator.SetTrigger ("Crawl");
	}

	public void StandUp(){
		//_animator.SetTrigger ("StandUp");
	}
    public void StartRun()
    {
       // _animator.SetTrigger("StartRunning");
    }
    public void EndRun()
    {
       // _animator.SetTrigger("EndRunning");
    }

    public void Play(Animations anim, bool isMirror = false)
    {
       // _animator.SetTrigger(anim.ToString());
       // _animator.SetBool("Mirror", isMirror);
    }

    public virtual void Attack()
    {
       // _animator.SetTrigger(AtackAnimations[0]);
      //  _animator.SetFloat("MovSpeed", 0.0f);
    }

    public virtual void UseAbility()
    {
       // _animator.SetTrigger(AtackAnimations[1]);
    }

    public void Ide()
    {
       // _animator.SetTrigger("Ide");
    }

    public virtual void Move()
    {	
        float speed = _navMeshAg.desiredVelocity.magnitude;
        //var speed = Vector3.Project(_navMeshAg.desiredVelocity, transform.forward).magnitude * _navMeshAg.speed * 4;
        //Debug.LogError(speed);

        speed = AnimatorMaxBlenValue / (_navMeshAg.speed / speed);

        _blendAnim = Mathf.Lerp(_blendAnim, speed, Time.deltaTime);
       // _animator.SetFloat("MovSpeed", _blendAnim);
    }

    public virtual void Die()
    {
      //  _animator.SetTrigger("Die");
    }

    public virtual void TakeDmg()
    {
       // _animator.SetTrigger("GetDamage");
    }
    /// <summary>
    /// Call at the end of the GetDamage animation.
    /// </summary>
    private void ResetTakeDmg()
    {
       // _animator.ResetTrigger("GetDamage");
    }
}
