using UnityEngine;
using System.Collections;

public class AttackState : IState<EnemyBase>
{
    public States State { get {return States.Attack;} }
    private EnemyBase _owner;
	private Ability[] abilityes;

    public void Enter(EnemyBase entity)
    {	
        _owner = entity;

		ArrayList abil = new ArrayList ();
		foreach (Ability a in _owner.Abilities) {
			if (a.AttackType == AttackType.MELEE) {
				abil.Add (a);
			}
		}
		abilityes = abil.ToArray (typeof(Ability))as Ability[];
		_owner.AnimationCompnent.Ide ();
        _owner.StateMachine.Behavior = Behavior;
    }

    public void Exit()
    {
        _owner.StateMachine.ChangeState(new MovingState());
        //_owner.SetStateMoving();
    }

    public void Behavior()
    {
        //_owner.transform.parent.position += _owner.AnimationCompnent.AnimatorComponent.deltaPosition;/// Time.deltaTime;

        if (_owner.Target.state == PlayerState.DIE || _owner.Target.state == PlayerState.WAIT)
			Exit ();
		if (!_owner._isHaveTarget) Exit();

        var playerDirection = (_owner.Target.transform.position - _owner.transform.position);
        playerDirection.y = 0;
        var playerDist = playerDirection.magnitude;

		if (playerDist < _owner.ClosenesDistance)
            _owner._closenessLevel += Time.deltaTime / _owner.ClosenessBuildup;

        else
        {
            _owner._closenessLevel -= Time.deltaTime / _owner.ClosenessBuildup;
            Exit();
        }
        _owner._closenessLevel = Mathf.Clamp01(_owner._closenessLevel);

        if (_owner._closenessLevel == 1 && State != States.UseAbility)
        {	
			int i = 0;
			foreach (Ability a in abilityes) {
				if (a.IsUse () && !_owner.Target.IsCaptured) {
					i++;
					_owner.UseAbility (a);
				}
				else
					break;	
			}
			if (State != States.Idle && i == 0) {
				_owner.Attack ();
				Debug.Log ("Attack");
			}
            	_owner._closenessLevel = 0;
        }
			
    }
}
