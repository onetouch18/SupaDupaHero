using UnityEngine;
using System.Collections;

public class PathRenderer : MonoBehaviour {

	public int Quality = 100;
	private MGCurve _curve;
	private LineRenderer _renderer;

	void Start(){
		_renderer = GetComponent<LineRenderer> ();
		_renderer.SetVertexCount (Quality);
		_renderer.enabled = false;
	}


	public void RenderPath(MGCurve curve){
		this._curve = curve;
		_renderer.enabled = true;
		float lenght = 0f;
		for (int i = 0; i < Quality; i++) {
			_renderer.SetPosition (i, curve.GetPoint ((float)(1 * i) / Quality));
			lenght += Vector3.Distance (curve.GetPoint ((float)(1 * i - 1) / Quality), curve.GetPoint ((float)(1 * i) / Quality));
		}
		_renderer.material.mainTextureScale = new Vector2(lenght, 1f);
	}
	

	
	// Update is called once per frame
	void Update () {
		
	}
}
