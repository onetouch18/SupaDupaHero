// Authors: 
//   Yurii Karpiuk (karpiuk@appmainia.com.ua) 
//

using UnityEngine;
using UnityEngine.Events;

public class StateMachine : Photon.MonoBehaviour
{
    public UnityAction Behavior = () => { };

    public IState<EnemyBase> CurrentState;
    private EnemyBase _owner;


    public void Configure(EnemyBase owner, IState<EnemyBase> initialState)
    {	
        _owner = owner;
        ChangeState(initialState);
    }

	/*
	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info){
		if (stream.isWriting) {
			stream.SendNext (CurrentState.State);
		} else {

		}
	}
	*/
    public void ChangeState(IState<EnemyBase> newState)
    {	
        CurrentState = newState;
        if (CurrentState != null)
            CurrentState.Enter(_owner);
    }
}
