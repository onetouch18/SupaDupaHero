// 
// SwipeDetector.cs 
// 
// Authors: 
//   Yurii Karpiuk (karpiuk@appmainia.com.ua) 
//

#define MOUSE
//define TOUCH

using System;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// The script captures the movement swipe.
/// </summary>
public class SwipeDetector : MonoBehaviour
{
    public class Swipe : UnityEvent<SwipeDirection>{}

    public enum SwipeDirection
    {
        Up,
        Down,
        Right,
        Left,
    }
    public static SwipeDetector Instance;

    [Header("Swipe distance in percentage from screen.")]
    [Range(0, 100)]
    public float minMovement = 5f;

    public Swipe OnSwipe;

    private Vector2 _startPos;
    private int _swipeId = -1;
    private float _mMinMovement;

    private void Update()
    {
#if MOUSE
        CheckMouse();
#else
        CheckTouch();
#endif
    }

    private void CheckMouse()
    {
		#if !UNITY_EDITOR

		if (Input.GetButtonDown ("Fire1"))
        {
            _startPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        }
		if (Input.GetButtonUp ("Fire1"))
        {
            var delta = new Vector2(Input.mousePosition.x, Input.mousePosition.y) - _startPos;

            if (!(delta.magnitude > _mMinMovement)) return;

            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
            {
                if (delta.x > 0)
                    OnSwipe.Invoke(SwipeDirection.Right);
                else if (delta.x < 0)
                    OnSwipe.Invoke(SwipeDirection.Left);
            }
            else
            {
                if (delta.y > 0)
                    OnSwipe.Invoke(SwipeDirection.Up);
                else if (delta.y < 0)
                    OnSwipe.Invoke(SwipeDirection.Down);
            }
        }
		#endif
    }

    private void CheckTouch()
    {
        foreach (var T in Input.touches)
        {
            var P = T.position;
            if (T.phase == TouchPhase.Began && _swipeId == -1)
            {
                _swipeId = T.fingerId;
                _startPos = P;
            }
            else if (T.fingerId == _swipeId)
            {
                var delta = P - _startPos;
                if (T.phase != TouchPhase.Moved || !(delta.magnitude > _mMinMovement)) continue;

                _swipeId = -1;
                if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
                {
                    if (delta.x > 0)
                        OnSwipe.Invoke(SwipeDirection.Right);
                    else if (delta.x < 0)
                        OnSwipe.Invoke(SwipeDirection.Left);
                }
                else
                {
                    if (delta.y > 0)
                        OnSwipe.Invoke(SwipeDirection.Up);
                    else if (delta.y < 0)
                        OnSwipe.Invoke(SwipeDirection.Down);
                }
            }
        }
    }

    private void Awake()
    {
        Instance = this;
        OnSwipe = new Swipe();
        _mMinMovement = (Screen.height * minMovement) / 100;
    }
}