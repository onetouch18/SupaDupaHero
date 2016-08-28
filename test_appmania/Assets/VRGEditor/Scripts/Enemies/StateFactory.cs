using System;
using UnityEngine;
using System.Collections;

public static class StateFactory
{
    public static IState<EnemyBase> GetState(States state)
    {
        switch (state)
        {
            case States.Attack:
                return new AttackState();
            case States.Walk:
                return new MovingState();
            case States.Die:
                return new DieState();
            case States.Run:
                return new RunState();
            case States.GlassBreak:
                return new GlassBreakState();
			case States.Crawling:
				return new CrawlingState ();
            case States.Animation:
                return new AnimationState();
            default:
                return new MovingState();///
        }
    }
}
