using UnityEngine;
using System.Collections;

public class IdleState : IState<EnemyBase>
{
    public States State { get {return States.Idle;} }

    public void Enter(EnemyBase entity)
    {
        throw new System.NotImplementedException();
    }

    public void Exit()
    {
        throw new System.NotImplementedException();
    }

    public void Behavior()
    {
       // throw new System.NotImplementedException();
    }
}
