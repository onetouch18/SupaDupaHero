using UnityEngine;
using System.Collections;

public class DieState : IState<EnemyBase>
{
    private EnemyBase _owner;

    public States State { get { return States.Die; } }

    public void Enter(EnemyBase entity)
    {	
		
        _owner = entity;
        _owner.Reset();
		//if(_owner.GetComponentInParent<BodyPhysicsController>())
       // _owner.GetComponentInParent<BodyPhysicsController>().Fall();
        //State = States.Die;
		foreach (Collider c in _owner.Agent.GetComponentsInChildren<Collider>()) {
			c.enabled = false;
		}
		_owner.obstacle.enabled = false;
        _owner.AnimationCompnent.Die();
		_owner.Invoke("DieParticle", _owner.DieRigTime);
		_owner.Invoke("Destroy", _owner.DieRigTime + _owner.ParticleTime);
        _owner.StateMachine.Behavior = Behavior;
    }

    public void Exit()
    {

    }

    public void Behavior()
    {
    }
}
