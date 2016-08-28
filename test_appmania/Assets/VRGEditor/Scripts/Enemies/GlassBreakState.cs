using UnityEngine;
using System.Collections;

public class GlassBreakState : IState<EnemyBase>
{
    public States State { get { return States.GlassBreak; } }
    private EnemyBase _owner;
  //  private HittableGlass _glass;
    private bool _isAttack;

    public void Enter(EnemyBase entity)
    {
        _owner = entity;

      //  _glass = _owner.Glass;

        //_owner.AnimationCompnent.Ide();
		//_owner.AttackEvent.AddListener(() => {_owner.broked = true;_glass.Break();});
        _owner.obstacle.enabled = false;
        _owner.Agent.enabled = true;
        _owner.StateMachine.Behavior = Behavior;
    }

    public void Exit()
    {
        //Debug.LogError(1);
		//_owner.AttackEvent.RemoveAllListeners();
        _owner.StateMachine.ChangeState(new MovingState());
		_owner.broked = true;
        //_owner.SetStateMoving();
    }

    public void Behavior()
    {
        //Debug.LogError(_glass);
        if (_owner.Delay > 0)
        {
            _owner.Delay -= Time.deltaTime;
            return;
        }


        //if (_glass == null || _glass.IsBreaked)
            Exit();

        //if (_isAttack) return;
        //if (!_isReached)
         //   _owner.Agent.SetDestination(_glass.transform.position);

        //if (Vector3.Distance(_owner.transform.position, _glass.transform.position) < _owner.AttackDistance)
        //{
        //    Debug.LogError("reach");
        //    _isReached = true;
        //    _owner.Agent.ResetPath();
        //    _owner.Agent.enabled = false;
        //    _owner.obstacle.enabled = true;
        //    //_owner.AnimationCompnent.Ide();
        //}
		//if (!_glass.IsBreaked) {
			//var playerDirection = (_glass.transform.position - _owner.transform.position);
			//playerDirection.y = 0;
			//var playerDist = playerDirection.magnitude;

			//if (playerDist < _owner.ClosenesDistance) {
			//	_isAttack = true;
			//	_owner.Attack ();
			//}
		//}
    }
}
