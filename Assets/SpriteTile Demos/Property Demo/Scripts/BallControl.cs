using UnityEngine;
using SpriteTile;

public class BallControl : MonoBehaviour {

	public Transform cam;
	public float insetAmount = 8.0f;
	public float torqueSpeed = 5.0f;
	public float jumpSpeed = 300.0f;
	public float maxAngularVelocity = 450.0f;
	public float groundDrag = 1.0f;
	
	private Vector2 lowerLimit;
	private Vector2 upperLimit;
	
	void Start () {
		// Create bounds for camera movement, so it's clamped to the level size
		lowerLimit = new Vector2(insetAmount * Camera.main.aspect, insetAmount);
		upperLimit = (Tile.GetMapSize() - Int2.one).ToVector2();
		upperLimit.x -= insetAmount * Camera.main.aspect;
		upperLimit.y -= insetAmount;
	}
	
	void FixedUpdate () {
		// Move left and right
		if (Mathf.Abs (GetComponent<Rigidbody2D>().angularVelocity) < maxAngularVelocity) {
			GetComponent<Rigidbody2D>().AddTorque (-Input.GetAxis ("Horizontal") * torqueSpeed);
		}
	}
	
	void Update () {
		// Clamp camera position to size of level
		var position = cam.position;
		position.x = Mathf.Clamp (transform.position.x, lowerLimit.x, upperLimit.x);
		position.y = Mathf.Clamp (transform.position.y, lowerLimit.y, upperLimit.y);
		cam.position = position;
		
		// Jump
		if (Input.GetKeyDown (KeyCode.Space)) {
			GetComponent<Rigidbody2D>().AddForce (Vector3.up * jumpSpeed);
		}
	}
	
	void OnCollisionStay2D () {
		// Simulate additional friction when grounded
		GetComponent<Rigidbody2D>().drag = groundDrag;
	}
	
	void OnCollisionEnter2D (Collision2D other) {
		// Make child of lift when in contact, so riding down lifts isn't bumpy
		if (other.gameObject.name == "Lift(Clone)") {
			transform.parent = other.transform;
		}
	}
	
	void OnCollisionExit2D () {
		transform.parent = null;
	}
}