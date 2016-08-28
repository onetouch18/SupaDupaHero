using UnityEngine;
using System.Collections;

public class AnimationState : IState<EnemyBase>
{
    private EnemyBase _owner;

    public States State { get { return States.Animation; } }
    public Animations Animation;
    public bool IsMirror;

    public void Enter(EnemyBase entity)
    {
        _owner = entity;
        Animation = _owner.Animation;
        IsMirror = _owner.IsMirror;
        _owner.Agent.enabled = false;
        _owner.AnimationCompnent.AnimatorComponent.applyRootMotion = true;
        _owner.StateMachine.Behavior = Behavior;

        _owner.AnimationCompnent.Play(Animation, IsMirror);
    }
    //call in animation
    public void Exit()
    {
        _owner.AnimationCompnent.AnimatorComponent.applyRootMotion = false;
        _owner.Agent.transform.position = _owner.transform.position;
        _owner.transform.localPosition = Vector3.zero;
        _owner.Agent.enabled = true;
        _owner.StateMachine.ChangeState(new MovingState());
    }

    public void Behavior()
    {
        //_owner.transform.parent.position += _owner.AnimationCompnent.AnimatorComponent.deltaPosition;
        //_owner.transform.parent.rotation = _owner.AnimationCompnent.AnimatorComponent.rootRotation;
    }
}