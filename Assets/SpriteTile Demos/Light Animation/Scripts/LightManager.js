#pragma strict
import SpriteTile;

var level : TextAsset;

function Awake () {
	Tile.SetCamera();
	Tile.LoadLevel (level);
}