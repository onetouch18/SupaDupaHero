using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

// ReSharper disable once CheckNamespace
public class Timer : MonoBehaviour
{
    public static Timer Instance;

    public static Coroutine StartTimer(float time, UnityAction<float> callEveryTip)
    {
        return Instance.StartCoroutine(TimerCor(time, callEveryTip));
    }

    public static Coroutine StartTimer<T>(T obj, float delay, UnityAction<T> action, Predicate<T> exitPredicate)
    {
        return Instance.StartCoroutine(TimerCor(exitPredicate, delay, action, obj));
    }

    public static Coroutine StartTimer<T>(T obj, float delay, UnityAction<T> action)
    {
        return Instance.StartCoroutine(TimerCor(delay, action, obj));
    }

    public static Coroutine CallAfter<T>(T obj, float delay, UnityAction<T> action)
    {
        return Instance.StartCoroutine(CallAfter(delay, action, obj));
    }

    private static IEnumerator CallAfter<T>(float delay, UnityAction<T> action, T obj)
    {
        yield return new WaitForSeconds(delay);

        action.Invoke(obj);
    }

    private static IEnumerator TimerCor<T>(float delay, UnityAction<T> action, T obj)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);

            action.Invoke(obj);
        }
    }

    private static IEnumerator TimerCor<T>(Predicate<T> exitPredicate, float delay, UnityAction<T> action, T obj)
    {
        while (true)
        {
            if (exitPredicate.Invoke(obj))
                yield break;

            yield return new WaitForSeconds(delay);

            action.Invoke(obj);
        }
    }

    private static IEnumerator TimerCor(float time, UnityAction<float> callEveryTip)
    {
        var timer = time;
        while (timer > 0)
        {
            callEveryTip.Invoke(timer);
            timer -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        callEveryTip.Invoke(0);
    }


    private void Awake()
    {
        Instance = this;
    }
}
