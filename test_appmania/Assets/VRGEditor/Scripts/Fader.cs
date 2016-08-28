using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Fader : MonoBehaviour
{
    private Image _image;
    private bool _isFading;
    public float FadeTime = 0.5F;


    private void OnEnable()
    {
        _image = GetComponent<Image>();
        _image.GetComponent<CanvasRenderer>().SetAlpha(0);
    }

    public void FadeOut()
    {
        if (_isFading) return;
        _isFading = true;
        _image = GetComponent<Image>();
        _image.CrossFadeAlpha(0.0f, FadeTime, false);
        _isFading = false;
    }
    [ContextMenu("s")]
    public void A()
    {
        FadeIn(0.5F);
    }

    public void FadeIn(float outAfter)
    {
        FadeIn();
        Invoke("FadeOut", outAfter);
    }

    public void FadeIn()
    {
        if (_isFading) return;
        _isFading = true;
        _image = GetComponent<Image>();
        _image.CrossFadeAlpha(0.7f, FadeTime, false);
        _isFading = false;
    }
}
