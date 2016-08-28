using UnityEngine;
using System.Collections;

public enum DoorType {
	AUTOMATIC,
	//USABLE,

}

public class Door : Photon.MonoBehaviour {


	public float Speed, WaitTime, width = 5f;
	public DoorType Type = DoorType.AUTOMATIC;

	public Transform door;
	private float start;
    public Sound OpenSound;
    private AudioSource _source;
	private bool _opened = false;


	void Start()
	{
	    _source = GetComponent<AudioSource>();
		start = door.localPosition.x;
	}
	[PunRPC]
	public void Open(){
		if (!_opened) {
			_opened = true;
			StartCoroutine (openDoor (width));
			PlaySound (OpenSound);
			GetComponent<OcclusionPortal> ().open = true;
			if(PhotonNetwork.connected)
			photonView.RPC ("Open", PhotonTargets.OthersBuffered);
		}
	}

	void OnTriggerEnter(Collider other){
		if(other.tag != "Untagged" && Type == DoorType.AUTOMATIC)
			Invoke ("Open", WaitTime);
		//if (other.tag != "Untagged" && Type == DoorType.USABLE) {
			//transform.GetChild (0).gameObject.SetActive (true);
		//}
	}

	void OnTriggerExit(Collider other){
		//StartCoroutine ("closeDoor");
	}

	IEnumerator openDoor(float width){
		while (door.localPosition.y < start + width) {
			Vector3 pos = door.localPosition;
				pos.y += Time.fixedDeltaTime * Speed;
			door.localPosition = pos;
			yield return null;
		}
	}

	IEnumerator closeDoor(){
		while (door.localPosition.x > start) {
			Vector3 pos = door.localPosition;
			pos.x -= Time.fixedDeltaTime * Speed;
			door.localPosition = pos;
			yield return null;
		}
		yield return null;
	}

    public void PlaySound(Sound sound, bool isLoop = false)
    {
        if (_source.isPlaying) return;
        var so = sound.Clip;
        if (so == null) return;
        _source.loop = isLoop;
        _source.clip = so;
        _source.Play();
    }
}
