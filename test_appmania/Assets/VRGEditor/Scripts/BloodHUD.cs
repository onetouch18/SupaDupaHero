using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class BloodHUD : MonoBehaviour
{
    [SerializeField]
    private Image _image;
    private Color _needColor;
    private Color _alphaZero;
    private const float HideTime = 5f;
    //private Coroutine _fadeAfterCoroutine;

    private void Awake()
    {
        _image.GetComponent<Image>();
        _alphaZero = _needColor = _image.color;
        _alphaZero.a = 0;
    }

    internal void SetHP(float current, float full)
    {
        //Debug.LogError("SEEEET");
        StopAllCoroutines();
        _needColor.a = 1 - Mathf.Clamp(current / full, 0, 1);
        _image.color = _needColor;
    }
    
    internal void UpdateHP(float current, float full, float speed = 1F)
    {
		
        _needColor.a = 1 - Mathf.Clamp(current / full, 0, 1);
        StopAllCoroutines();
        StartCoroutine(Fade(_needColor, speed));
    }

	IEnumerator Fade(Color color, float speed){

		_image.color = _needColor;

		while (_image.color.a > 0) {
			_image.color = Color.Lerp (_image.color, _alphaZero, Time.deltaTime);

			yield return new WaitForEndOfFrame();
		}

	}


	/*
    private IEnumerator FadeAfter(Color color, float speed = 1f)
    {
        var timer = HideTime;
        //while (timer > 0)
        //{
        //    yield return new WaitForEndOfFrame();
        //    timer -= Time.deltaTime;
        ///}
        //Debug.LogError("start FadeAfter");

        yield return new WaitForSeconds(HideTime);
       // Debug.LogError("go FadeAfter");

        while (true)
        {
            if (Math.Abs(_image.color.a - color.a) < 0.02)
            {
                _image.color = color;
                //Debug.LogError("break FadeAfter");
                yield break;
            }
            //Debug.LogError("lerp FadeAfter");
            _image.color = Color.Lerp(_image.color, color, Time.deltaTime * speed);
            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator Fade(Color color, float speed)
    {
        while (true)
        {
            if (Math.Abs(_image.color.a - color.a) < 0.02)
            {
                if (_needColor.a < 1)
                {
                    /*_fadeAfterCoroutine = StartCoroutine(FadeAfter(_alphaZero));
                }
                _image.color = color;
				StartCoroutine(FadeAfter(_alphaZero));
               // Debug.LogError("break");
               // yield break;
            }
         //   Debug.LogError("lerp");
            //_image.color = Color.Lerp(_image.color, color, Time.deltaTime * speed);
            yield return new WaitForEndOfFrame();
        }
    }
	*/
}
