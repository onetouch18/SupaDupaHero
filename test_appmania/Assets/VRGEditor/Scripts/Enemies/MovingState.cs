using UnityEngine;
using System.Collections;

public class MovingState : IState<EnemyBase>
{
    public States State { get { return States.Walk; } }
    private EnemyBase _owner;
	private Ability[] abilityes;
	private float _buildUpTime;
	private Quaternion _look;
	//private Vector3 prevPos;

    public void Enter(EnemyBase entity)
    {
        _owner = entity;

		ArrayList abil = new ArrayList ();
		foreach (Ability a in _owner.Abilities) {
			if (a.AttackType == AttackType.RANGED) {
				abil.Add (a);
			}
		}
		abilityes = abil.ToArray (typeof(Ability))as Ability[];

        _owner.obstacle.enabled = false;
        _owner.Agent.enabled = true;

        _owner.StateMachine.Behavior = Behavior;
		_owner.AnimationCompnent.AnimatorComponent.applyRootMotion = true;
		//_owner.Agent.updatePosition = false;
       // _owner.Agent.updateRotation = false;

        _owner.transform.parent.position = new Vector3(_owner.transform.parent.position.x, _owner.Agent.nextPosition.y, _owner.transform.parent.position.z);
        //_owner.Agent.updateRotation = true;
        //_owner.PlaySound(_owner.WalkingSound, true);
    }


    public void Exit()
    {
        _owner.StopSound();
		if(_owner.attackType == AttackType.MELEE)
        _owner.StateMachine.ChangeState(new AttackState());
		else 
			_owner.StateMachine.ChangeState(new RangedAttackState());
        //_owner.SetStateAttack();
    }

    public void Behavior()
    {	
		//Move ();
        //Debug.LogError(_owner._isHaveTarget + " " + _owner.Agent.isOnNavMesh);
        if (_owner._isHaveTarget && _owner.Agent.isOnNavMesh)
            _owner.Agent.SetDestination(_owner.Target.transform.position);
        else
            return;
		//_owner.transform.position = _owner.Agent.transform.position;

		_buildUpTime += Time.deltaTime * _owner.Speed;
		_buildUpTime = Mathf.Clamp01(_buildUpTime);

		if (_buildUpTime == 1f) {
			RaycastHit hitInfo;
			if (Physics.Raycast (_owner.Agent.transform.position, _owner.Agent.transform.forward, out hitInfo)) {
				if (hitInfo.transform.tag == "Player") {
					foreach (Ability a in abilityes) {
						if (Vector3.Distance (_owner.transform.position, _owner.Target.transform.position) < a.Range && a.IsUse ()) {
							_owner.UseAbility (a);
						} else
							break;	
					}
				}
			}
			_buildUpTime = 0f;
		}


        if (Vector3.Distance(_owner.transform.position, _owner.Target.transform.position) < _owner.AttackDistance)
        {
            Exit();
            _owner.Agent.ResetPath();
			_owner.Agent.enabled = false;
			_owner.obstacle.enabled = true;
        }
    }

	void Move(){
        //prevPos = _owner.AnimationCompnent.AnimatorComponent.bodyPosition;

        //Vector3 position = _owner.AnimationCompnent.AnimatorComponent.bodyPosition;
        //position.y = _owner.Agent.nextPosition.y;

        Vector3 direction =_owner.Agent.steeringTarget - _owner.transform.position;

		direction.y = 0.0f;
		if (direction.magnitude > 0.1f)
		{
			Quaternion qLook = Quaternion.LookRotation(direction, Vector3.up);
			_look = qLook;
		}

		_owner.transform.rotation = Quaternion.Slerp(_owner.transform.rotation, _look, Time.deltaTime );

		_owner.Agent.Warp(_owner.AnimationCompnent.AnimatorComponent.rootPosition);

		_owner.transform.localPosition = Vector3.zero;
	}
}
