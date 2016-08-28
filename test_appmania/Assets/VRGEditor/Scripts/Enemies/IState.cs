// Authors: 
//   Yurii Karpiuk (karpiuk@appmainia.com.ua) 
//

using UnityEngine;

//public abstract class StateGeneric<T> : MonoBehaviour
//{
//    public States State { get { return _state; } }
//    [SerializeField] protected States _state;

//    public abstract void Enter(T entity);
//    public abstract void Exit(T entity);
//    protected abstract void Behavior();
//}

public interface IState<T>
{
    States State { get; }

    void Enter(T entity);
    void Exit();
    void Behavior();
}

