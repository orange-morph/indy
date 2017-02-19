#pragma strict

var cam : Transform;
var insetAmount = 8.0;
var torqueSpeed = 5.0;
var jumpSpeed = 300.0;
var maxAngularVelocity = 450.0;
var groundDrag = 1.0;

private var lowerLimit : Vector2;
private var upperLimit : Vector2;

function Start () {
	// Create bounds for camera movement, so it's clamped to the level size
	lowerLimit = Vector2(insetAmount * Camera.main.aspect, insetAmount);
	upperLimit = (Tile.GetMapSize() - Int2.one).ToVector2();
	upperLimit.x -= insetAmount * Camera.main.aspect;
	upperLimit.y -= insetAmount;
}

function FixedUpdate () {
	// Move left and right
	if (Mathf.Abs (GetComponent (Rigidbody2D).angularVelocity) < maxAngularVelocity) {
		GetComponent (Rigidbody2D).AddTorque (-Input.GetAxis ("Horizontal") * torqueSpeed);
	}
}

function Update () {
	// Clamp camera position to size of level
	cam.position.x = Mathf.Clamp (transform.position.x, lowerLimit.x, upperLimit.x);
	cam.position.y = Mathf.Clamp (transform.position.y, lowerLimit.y, upperLimit.y);
	
	// Jump
	if (Input.GetKeyDown (KeyCode.Space)) {
		GetComponent (Rigidbody2D).AddForce (Vector3.up * jumpSpeed);
	}
}

function OnCollisionStay2D () {
	// Simulate additional friction when grounded
	GetComponent (Rigidbody2D).drag = groundDrag;
}

function OnCollisionEnter2D (other : Collision2D) {
	// Make child of lift when in contact, so riding down lifts isn't bumpy
	if (other.gameObject.name == "Lift(Clone)") {
		transform.parent = other.transform;
	}
}

function OnCollisionExit2D () {
	transform.parent = null;
}