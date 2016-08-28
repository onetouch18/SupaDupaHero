using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour {

	public Image health;
	public Text full;
	public Text current;
	// Use this for initialization
	public void SetHP(float current, float full){
		health.fillAmount = current/full;
		this.full.text = full.ToString();
		this.current.text = (current).ToString();
	}
}
