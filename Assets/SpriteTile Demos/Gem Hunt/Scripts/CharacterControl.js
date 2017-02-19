// This moves a character around a SpriteTile level using square-by-square movement,
// and prevents the character from moving into collider cells
// Permission is granted to use, modify, and redistribute this script in any way.

#pragma strict
import SpriteTile;

var moveSpeed = 5.0;
var fastSpeedFactor = 4.0;
var positionOffset = Vector2.zero;
var layer = 0;
var ignoreColliders = false;

private var mapPos : Int2;
private var t = 0.0;
private var xInput = 0.0;
private var yInput = 0.0;
private var isMoving = false;

private var useMoveDelegate = false;
private var moveDelegate : function(Int2, float, float):void;
private var useStartDelegate = false;
private var startDelegate : function(Int2, Int2, boolean):void;
private var useEndDelegate = false;
private var endDelegate : function(Int2, Int2, boolean):void;

function Start () {
	yield;
	SetMapPosition();
	transform.position += positionOffset;
	PlayerInput();
}

function SetMapPosition () {
	mapPos = Tile.WorldToMapPosition (transform.position, layer);
}

// Coroutine that waits for input in the horizontal and vertical axes, and moves player accordingly
private function PlayerInput () : IEnumerator {
	while (true) {
		while (xInput == 0.0 && yInput == 0.0) {
			SetInput();
			yield;
		}
		
		var dir = GetDirection();
		if (useStartDelegate) {
			startDelegate (dir, mapPos, isMoving);
		}
		
		yield Move (dir);
	}
}

private function GetDirection () : Int2 {
	if (xInput < 0.0) {
		return Int2.left;
	}
	else if (xInput > 0.0) {
		return Int2.right;
	}
	else if (yInput < 0.0) {
		return Int2.down;
	}
	return Int2.up;
}

private function Move (dir : Int2) : IEnumerator {
	// No movement if the tile being moved into is a collider
	if (Tile.GetCollider (mapPos + dir, layer) && !ignoreColliders) {
		transform.position = Tile.MapToWorldPosition (mapPos, layer) + positionOffset;	// Make sure position is exact in case the player was moving previously
		SetInput();
		return;
	}
	
	if (Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift)) {
		var multiplySpeed = fastSpeedFactor;
	}
	else {
		multiplySpeed = 1.0;
	}
	
	// Move to next tile
	var startPos = Tile.MapToWorldPosition (mapPos, layer);
	mapPos += dir;
	var endPos = Tile.MapToWorldPosition (mapPos, layer);
	
	while (t <= 1.0) {
		transform.position = Vector3.Lerp (startPos, endPos, t) + positionOffset;
		if (useMoveDelegate) {
			moveDelegate (dir, t, multiplySpeed);
		}
		t += Time.deltaTime * moveSpeed * multiplySpeed;
		yield;
	}
	t -= 1.0;	// Subtract 1 rather than resetting to 0, so that movement over multiple tiles is 100% smooth
	
	// If there's no input, then make the final position be exactly equal to endPos
	isMoving = true;
	SetInput();
	if ((xInput == 0.0 && yInput == 0.0) || (Tile.GetCollider (mapPos + GetDirection(), layer) && !ignoreColliders)) {
		transform.position = endPos + positionOffset;
		t = 0.0;
		isMoving = false;
	}
	
	if (useEndDelegate) {
		endDelegate (dir, mapPos, isMoving);
	}
}

private function SetInput () {
	xInput = Input.GetAxis ("Horizontal");
	yInput = Input.GetAxis ("Vertical");
}

function SetMoveDelegate (thisDelegate : function(Int2, float, float):void) {
	useMoveDelegate = true;
	moveDelegate = thisDelegate;
}

function SetStartMoveDelegate (thisDelegate : function(Int2, Int2, boolean):void) {
	useStartDelegate = true;
	startDelegate = thisDelegate;
}

function SetEndMoveDelegate (thisDelegate : function(Int2, Int2, boolean):void) {
	useEndDelegate = true;
	endDelegate = thisDelegate;
}