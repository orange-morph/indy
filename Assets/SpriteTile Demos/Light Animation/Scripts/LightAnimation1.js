#pragma strict
import SpriteTile;

var anchor : Transform;
private var lastPos : Int2;

function Start () {
	lastPos = Tile.WorldToMapPosition (transform.position);
	Tile.SetLight (lastPos, 0);
	
	// Set up rope
	GetComponent(LineRenderer).sortingOrder = 1;
	GetComponent(LineRenderer).SetPosition (0, anchor.position);
}

function Update () {
	// Move the light if the world position of the sprite has moved to a new tile
	var newPos = Tile.WorldToMapPosition (transform.position);
	if (newPos != lastPos) {
		Tile.MoveLight (lastPos, newPos);
		lastPos = newPos;
	}
	
	GetComponent(LineRenderer).SetPosition (1, transform.position);
}