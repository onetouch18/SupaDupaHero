using UnityEngine;
using System.Collections;

public class RangedAttackState : IState<EnemyBase> {

	public States State { get { return States.Walk; } }
	private EnemyBase _owner;
	private Ability[] abilityes;
	private float _buildUpTime;


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

		_owner.StateMachine.Behavior = Behavior;
	}

	public void Exit()
	{
		_owner.StateMachine.ChangeState(new MovingState());
		//_owner.SetStateMoving();
	}

	public void Behavior()
	{
		if (!_owner._isHaveTarget) return;

		var playerDirection = (_owner.Target.transform.position - _owner.transform.position);
		playerDirection.y = 0;
		var playerDist = playerDirection.magnitude;

		if (playerDist < _owner.AttackDistance)
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
				if (a.IsUse ()) {
					i++;
					_owner.UseAbility (a);
				}
				else
					break;	
			}
			if (State != States.Idle && i == 0)
				_owner.Attack();
			_owner._closenessLevel = 0;
		}	
	}
}
