#pragma strict

var startPos = Vector3(5.5, 13.5, 0);
var torqueSpeed = 5.0;
var maxAngularVelocity = 1200.0;
var jumpSpeed = 320.0;
var acornFrames : Sprite[];
var groundDrag = 0.4;
var fallSound : AudioClip;
var landSound : AudioClip;
var jumpSound : AudioClip;
var dieSound : AudioClip;
var gateSound : AudioClip;
private var audioSources : AudioSource[];
private var isGrounded = false;
private var groundedCount = 0;
private var canMoveLeft : boolean;
private var manager : Manager;
private var setup = false;
@HideInInspector
var canControl = false;
@HideInInspector
var dying = false;
@HideInInspector
var levelBound : float;
@HideInInspector
var landed = false;

private var spikeMin = 21;
private var spikeMax = 24;
private var gate = 26;
private var noLeftControl = 1;
private var restoreLeftControl = 2;
private var canJump = true;

function Awake () {
#if !UNITY_4_3
	GetComponent (Rigidbody2D).inertia = .15;
#endif
}

function Initialize () {
	if (!setup) {
		manager = FindObjectOfType (Manager);
		audioSources = GetComponents.<AudioSource>();
		audioSources[1].clip = jumpSound;
		setup = true;
	}
	transform.position = startPos;
	transform.rotation = Quaternion.identity;
	GetComponent (Rigidbody2D).drag = 0.1;
	GetComponent (Rigidbody2D).velocity = Vector2.zero;
	GetComponent (Rigidbody2D).angularVelocity = 0.0;
	GetComponent (Rigidbody2D).isKinematic = false;
	GetComponent (SpriteRenderer).enabled = true;
	GetComponent (SpriteRenderer).color.a = 1.0;
	GetComponent (SpriteRenderer).sortingLayerName = Tile.GetSortingLayerName (2);
	canMoveLeft = true;
	landed = false;
	isGrounded = false;
	audioSources[0].clip = fallSound;
	audioSources[0].Play();
}

function FixedUpdate () {
	if (!isGrounded) {
		groundedCount--;
	}
	if (!canControl) return;
	
	// Allow torque to be added if the acorn isn't spinning too fast and is grounded (or nearly so)
	if (Mathf.Abs (GetComponent (Rigidbody2D).angularVelocity) < maxAngularVelocity && groundedCount > 0) {
#if (UNITY_IPHONE || UNITY_ANDROID) && !UNITY_EDITOR
		// Cheesy (and invisible) touch-screen controls...bottom left corner moves left, bottom right corner moves right
		for (var i = 0; i < Input.touchCount; i++) {
			var touch = Input.GetTouch (i);
			if (Rect(0, 0, Screen.width/4, Screen.height/4).Contains (touch.position)) {
				var input = -1.0;
			}
			else if (Rect(Screen.width/4 * 3, 0, Screen.width/4, Screen.height/4).Contains (touch.position)) {
				input = 1.0;
			}
		}
#else
		var input = Input.GetAxis ("Horizontal");
#endif
		if (!canMoveLeft && input < 0.0) {
			input = 0.0;
		}
		GetComponent (Rigidbody2D).AddTorque (-input * torqueSpeed);
	}

}

function Update () {
	if (!canControl) return;
	
#if (UNITY_IPHONE || UNITY_ANDROID) && !UNITY_EDITOR
	// More cheesy touch-screen controls...bottom center jumps
	var jump = false;
	for (var i = 0; i < Input.touchCount; i++) {
		var touch = Input.GetTouch (i);
		if (Rect(Screen.width/4, 0, Screen.width/4 * 2, Screen.height/4).Contains (touch.position) && touch.phase == TouchPhase.Began) {
			jump = true;
		}
	}
#else
	var jump = Input.GetKeyDown (KeyCode.Space);
#endif
	if (jump && canJump && groundedCount > 0) {
		Jump();
	}
	
	if (transform.position.y < levelBound) {	// Make sure calls to GetTile etc. are within level bounds
		var tileInfo = Tile.GetTile (transform.position, 2);
		// If we hit a spike
		if (isGrounded && tileInfo.tile >= spikeMin && tileInfo.tile <= spikeMax) {
			PlayerDie();
		}
		
		// If we entered a gate
		if (tileInfo.tile == gate) {
			LevelComplete();
		}
		
		// If we entered a trigger
		var trigger = Tile.GetTrigger (transform.position, 2);
		if (trigger == noLeftControl) {
			canMoveLeft = false;
		}
		else if (trigger == restoreLeftControl) {
			canMoveLeft = true;
		}
	}
}

function Jump () : IEnumerator {
	canJump = false;
	GetComponent (Rigidbody2D).AddForce (Vector2.up * jumpSpeed);
	audioSources[1].Play();
	groundedCount = 0;
	// Prevent double-jumps
	yield WaitForSeconds (0.25);
	canJump = true;
}

function PlayerDie () : IEnumerator {	
	canControl = false;
	dying = true;
	audioSources[0].clip = dieSound;
	audioSources[0].Play();
	manager.LoseLife();
	for (var i = 1; i < acornFrames.Length; i++) {
		GetComponent (SpriteRenderer).sprite = acornFrames[i];
		yield WaitForSeconds (.08);
	}
	GetComponent (SpriteRenderer).enabled = false;
	GetComponent (SpriteRenderer).sprite = acornFrames[0];
	dying = false;
}

function LevelComplete () : IEnumerator {
	canControl = false;
	GetComponent (Rigidbody2D).velocity = Vector2.zero;
	GetComponent (Rigidbody2D).angularVelocity = 0.0;
	GetComponent (Rigidbody2D).isKinematic = true;
	
	MoveToCenterOfGate();
	audioSources[0].clip = gateSound;
	audioSources[0].Play();
	yield FlareEffect();
	yield WaitForSeconds (0.5);
	
	manager.NextLevel();
}

function MoveToCenterOfGate () : IEnumerator {
	var startPos = transform.position;
	// Convert transform.position to map position and back to world position to get
	// center of tile in world coordinates
	var mapPos = Tile.WorldToMapPosition (transform.position, 2);
	var endPos = Tile.MapToWorldPosition (mapPos, 2);
	var t = 0.0;
	while (t <= 1.0) {
		t += Time.deltaTime * 3;
		var easeOut = Mathf.Sin (t * Mathf.PI * 0.5);
		transform.position = Vector3.Lerp (startPos, endPos, easeOut);
		yield;
	}
}

function FlareEffect () : IEnumerator {
	var flare = transform.Find ("Flare");
	flare.gameObject.SetActive (true);
	var flareObjects = flare.GetComponentsInChildren.<Transform>();
	for (flareObject in flareObjects) {
		flareObject.GetComponent (SpriteRenderer).sortingLayerName = Tile.GetSortingLayerName (2);
	}
	
	var t = 0.0;
	while (t <= 1.0) {
		t += Time.deltaTime * 0.3;
		for (var i = 0; i < 2; i++) {
			flareObjects[i].GetComponent (SpriteRenderer).color.a = Mathf.Sin (t * Mathf.PI);
			var scale = 0.3 * (Mathf.Sin (t * Mathf.PI) + 1.0);
			flareObjects[0].localScale = Vector3(scale, scale, 1.0);
			flareObjects[1].localScale = Vector3(scale*1.5, scale*1.5, 1.0);
			var angle = t * 90.0;
			flareObjects[0].eulerAngles = Vector3(0.0, 0.0, angle);
			flareObjects[1].eulerAngles = Vector3(0.0, 0.0, -angle/3);
		}
		GetComponent (SpriteRenderer).color.a = 1.0 - t;
		yield;
	}

	flare.gameObject.SetActive (false);
}

function OnCollisionEnter2D () {	
	if (!landed) {
		landed = true;
		audioSources[0].clip = landSound;
	}
	var tileInfo = Tile.GetTile (transform.position, 2);
	var playLandSound = true;
	if (tileInfo.tile >= spikeMin && tileInfo.tile <= spikeMax) {
		playLandSound = false;
	}
	if (groundedCount <= 0 && playLandSound) {
		audioSources[0].Play();
	}
	OnCollisionStay2D();
}

function OnCollisionStay2D () {
	GetComponent (Rigidbody2D).drag = groundDrag;	// Simulate additional friction when grounded
	isGrounded = true;
	groundedCount = 10;
	// This way small bumps where the acorn is briefly not grounded can still be under player control for up to 10 physics frames
	// Has a better feel than being 100% physically correct
}

function OnCollisionExit2D () {
	isGrounded = false;
	GetComponent (Rigidbody2D).drag = 0.1;
}