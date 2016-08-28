using UnityEngine;
using System.Collections;

public class CrawlingState : IState<EnemyBase> {

	private EnemyBase _owner;


	public States State { get { return States.Crawling; } }

	public void Enter(EnemyBase entity)
	{
		_owner = entity;
		_owner.Agent.enabled = false;
		_owner.AnimationCompnent.AnimatorComponent.applyRootMotion = true;
		_owner.StateMachine.Behavior = Behavior;
		_owner.AnimationCompnent.StartCrawling();
		_owner.Invoke("ExitFromState", _owner.StateExitTime);
		//_owner.Invoke("ExitFromState", 2F);
	}

	public void Exit()
	{	
		_owner.AnimationCompnent.StandUp ();
		_owner.AnimationCompnent.AnimatorComponent.applyRootMotion = false;
		_owner.Agent.transform.position = _owner.transform.position;
		_owner.transform.localPosition = Vector3.zero;
		_owner.Agent.enabled = true;
		_owner.StateMachine.ChangeState(new MovingState());

	}

	public void Behavior()
	{	
		
	}
}
