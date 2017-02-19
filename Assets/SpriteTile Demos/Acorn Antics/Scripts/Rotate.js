#pragma strict

var speed = 1.0;
var angle = 5.0;

function Update () {
	transform.eulerAngles = Vector3(0, 0, Mathf.Sin (Time.time * speed) * angle);
}