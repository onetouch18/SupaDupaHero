using UnityEngine;
using System.Collections;

public class PlayerAnimation : Photon.MonoBehaviour {

	public Transform right,left;

	//public Transform rootBone;
	//public Transform upperBodyBone;

	//demo
	public Transform spine, weapon;

	private Animator animator;

	//private Vector3 spin, oldSpin;
	//private Vector3 upper, oldUp;


	void Start(){
		animator = GetComponent<Animator> ();
		//cam = GameObject.FindGameObjectWithTag("vrCam").transform.GetChild (0);

		//Vector3 rot = transform.eulerAngles;
		//rot.y = cam.eulerAngles.y;
		//upperBodyBone.eulerAngles = rot;

	}


	void OnAnimatorIK(){
		//if(!photonView.isMine){
		//animator.SetLookAtWeight (1f,1f,0f,0f,1f);
		//animator.SetLookAtPosition (weapon.GetChild(0).forward*100f);
		Vector3 rot = spine.eulerAngles;
		rot.y = weapon.eulerAngles.y + 180;
		animator.SetBoneLocalRotation(HumanBodyBones.Spine, Quaternion.Euler(rot));
		//Vector3 spineTransform = spine.eulerAngles;
		//spineTransform.y = weapon.eulerAngles.y;
		//spine.forward = spineTransform;
		
		animator.SetIKPositionWeight(AvatarIKGoal.RightHand,1f);
		//animator.SetIKRotationWeight(AvatarIKGoal.RightHand,1);  
		animator.SetIKPosition(AvatarIKGoal.RightHand,right.position);
		//animator.SetIKRotation(AvatarIKGoal.RightHand,right.rotation);

		animator.SetIKPositionWeight(AvatarIKGoal.LeftHand,1f);
		//animator.SetIKRotationWeight(AvatarIKGoal.LeftHand,1);  
		animator.SetIKPosition(AvatarIKGoal.LeftHand,left.position);
		//animator.SetIKRotation(AvatarIKGoal.LeftHand,left.rotation);
		//}

	}


	void LateUpdate (){
		//
	}
	/*
	private void Rot(){
			Vector3 rot = transform.eulerAngles;
			rot.y = cam.eulerAngles.y + 55;
			upperBodyBone.eulerAngles = rot;
			oldUp = rot;
	}


	void  LateUpdate (){
		if (photonView.isMine || !PhotonNetwork.connected) {
			Rot ();
			spine.Rotate (Vector3.right, cam.rotation.eulerAngles.x, Space.Self);
			oldSpin = spine.eulerAngles;
		} else {
			upperBodyBone.eulerAngles = Vector3.Lerp(oldUp, upper, Time.deltaTime);
			spine.eulerAngles = Vector3.Lerp(oldSpin, spin, Time.deltaTime);

		}

	}

	static float HorizontalAngle ( Vector3 direction  ){
		return Mathf.Atan2 (direction.x, direction.z) * Mathf.Rad2Deg;
	}

	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info){
		if (stream.isWriting) {
			stream.SendNext (upperBodyBone.eulerAngles);
			stream.SendNext (spine.eulerAngles);
		} else {
			upper = (Vector3) stream.ReceiveNext ();
			spin = (Vector3) stream.ReceiveNext ();
		}
	}
	*/
}
