#pragma strict
import SpriteTile;

var moveSpeed : float;

private var lastPos : Int2;

function Start () {
	lastPos = Tile.WorldToMapPosition (transform.position);
}

function Update () {
	transform.Translate (Vector3.up * moveSpeed * Time.deltaTime);
	var newPos = Tile.WorldToMapPosition (transform.position);
	// When moving to a new tile, check colliders 3 tiles up (across the width of the lift) and below, and reverse direction when appropriate
	if (newPos != lastPos) {
		lastPos = newPos;
		if ((moveSpeed > 0.0 && (Tile.GetCollider (newPos + Int2.up*3) || Tile.GetCollider (newPos + Int2.upLeft + Int2.up*2) || Tile.GetCollider (newPos + Int2.upRight + Int2.up*2))) ||
			(moveSpeed < 0.0 && Tile.GetCollider (newPos))) {
			moveSpeed = -moveSpeed;
		}
		// Pick up color from map
		GetComponent(SpriteRenderer).color = Tile.GetColor (newPos);
	}	
}