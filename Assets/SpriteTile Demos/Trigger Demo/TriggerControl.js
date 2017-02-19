// Make stuff happen when different triggers are encountered

#pragma strict
import SpriteTile;

var triggerLevel : TextAsset;
var textObjects : Transform[];

private var wallSet = true;
private var rotationSet = false;
private var rotation = 0.0;

function Awake () {
	Tile.SetCamera();
	Tile.LoadLevel (triggerLevel);
	
	// Set position of text objects to specific tiles
	for (tObj in textObjects) {
		tObj.gameObject.SetActive (true);
		tObj.GetComponent(Renderer).sortingOrder = 1;	// Makes text draw above tiles (0) but below the player sprite (2)
	}
	textObjects[0].position = Tile.MapToWorldPosition (Int2(2, 17));
	textObjects[1].position = Tile.MapToWorldPosition (Int2(3, 5));
	textObjects[2].position = Tile.MapToWorldPosition (Int2(3, 2));
	textObjects[3].position = Tile.MapToWorldPosition (Int2(19, 14));
	textObjects[4].position = Tile.MapToWorldPosition (Int2(19, 5));
}

function Update () {
	if (rotationSet) {
		RotateTile();
	}

	var trigger = Tile.GetTrigger (transform.position);
	if (trigger == 0) return;
	
	switch (trigger) {
		case 1:
			SetWalls (false);
			break;
		case 2:
			SetWalls (true);
			break;
		case 3:
			rotationSet = true;
			break;
		case 4:
			rotationSet = false;
			break;
	}
}

function SetWalls (activate : boolean) {
	if (wallSet == activate) return;
	
	wallSet = activate;
	var tile = activate? 1 : 0;
	Tile.SetTileBlock (Int2(8, 3), Int2(8, 16), 3, tile, activate);
	Tile.SetTileBlock (Int2(8, 3), Int2(22, 3), 3, tile, activate);
	Tile.SetTileBlock (Int2(8, 16), Int2(22, 16), 3, tile, activate);
	Tile.SetTileBlock (Int2(22, 3), Int2(22, 16), 3, tile, activate);
}

function RotateTile () {
	rotation += Time.deltaTime * 45.0;
	Tile.SetRotation (Int2(15, 9), rotation);
}