#pragma strict
import SpriteTile;

var target : Transform;

// Copy the x/y coords of the target, but keep the z position as is
function LateUpdate () {
	transform.position = Vector3(target.position.x, target.position.y, transform.position.z);
}