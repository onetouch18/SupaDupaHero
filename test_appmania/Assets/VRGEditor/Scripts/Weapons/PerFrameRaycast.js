#pragma strict

private var hitInfo : RaycastHit;
private var tr : Transform;

function Awake () {
	tr = transform;
}

function Update () {
	// Cast a ray to find out the end point of the laser
	hitInfo = RaycastHit ();
	Physics.Raycast (tr.GetChild(0).position, tr.GetChild(0).forward, hitInfo);
}

function GetHitInfo () : RaycastHit {
	return hitInfo;
}
