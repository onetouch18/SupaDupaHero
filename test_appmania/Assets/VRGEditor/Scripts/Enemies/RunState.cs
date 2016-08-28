using UnityEngine;
using System.Collections;

public class RunState : IState<EnemyBase>
{
    public States State { get { return States.Run; } }
    private EnemyBase _owner;

    public void Enter(EnemyBase entity)
    {
        Debug.LogError(1);
        _owner = entity;
        _owner.Agent.enabled = true;
        _owner.Agent.speed = _owner.RunSpeed;
        _owner.StateMachine.Behavior = Behavior;
		//_owner.Agent.updateRotation = true;
        _owner.AnimationCompnent.StartRun();

        //_owner.PlaySound(_owner.WalkingSound, true);
    }


    public void Exit()
    {
        _owner.Agent.speed = _owner.Speed;
        _owner.StopSound();
        _owner.AnimationCompnent.EndRun();
        _owner.StateMachine.ChangeState(new AttackState());
        //_owner.SetStateAttack();
    }

    public void Behavior()
    {
        if (_owner._isHaveTarget && _owner.Agent.isOnNavMesh)
            _owner.Agent.SetDestination(_owner.Target.transform.position);
        else
            return;
		_owner.transform.position = _owner.Agent.transform.position;
        if (Vector3.Distance(_owner.transform.position, _owner.Target.transform.position) < _owner.AttackDistance)
        {
            Exit();
            _owner.Agent.ResetPath();
			_owner.Agent.enabled = false;
			//_owner.obstacle.enabled = true;
        }
    }
}
