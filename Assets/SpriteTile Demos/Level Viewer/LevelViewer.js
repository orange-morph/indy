// This simply loads in a level and allows you to scroll around it by using the arrow keys

#pragma strict
import SpriteTile;

var moveSpeed = 10.0;
var level : TextAsset;

function Start () {
	Tile.SetCamera();
	Tile.LoadLevel (level);
}

function Update () {
	transform.Translate (Vector2(Input.GetAxis ("Horizontal") * moveSpeed * Time.deltaTime,
								 Input.GetAxis ("Vertical") * moveSpeed * Time.deltaTime));
}