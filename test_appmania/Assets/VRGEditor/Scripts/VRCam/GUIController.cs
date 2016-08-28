using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GUIController : MonoBehaviour {

	public HealthBar health;
	public Slider slider;
	public Text curB, fullB, GuiLogText;
    [SerializeField] private BloodHUD _bloodHud;
	// Use this for initialization


	static GUIController _this;

	public void UpdateHP(float current, float full, float speed = 1){
        health.SetHP (current, full);
	    //_bloodHud.UpdateHP(current, full, speed);
	}

    public void SetHP(float current, float full)
    {
        health.SetHP (current, full);
       //_bloodHud.SetHP(current, full);
    }

    void Awake()
	{
		_this = this;
	}
	public void SetSlader(float procentage){
		slider.gameObject.SetActive (true);
		slider.value = procentage;
	}

	public void UpdateSlider(float procentage){
		slider.value = procentage;
	}

	public void DeactivateSlider(){
		slider.gameObject.SetActive (false);
	}

	public void setBullets(int current, int full){
		curB.text = current.ToString ();
		fullB.text = full.ToString ();
	}
	public static void ShowLog(string Logstring)
	{
		_this.GuiLogText.text = Logstring;
	}
}
