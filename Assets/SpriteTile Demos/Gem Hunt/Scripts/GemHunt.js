// This script illustrates several SpriteTile concepts, such as layers, animation, GetTriggerPositions, GetTilePositions, SetColor,
// deleting and restoring tiles, and performing actions based on the trigger and tile values of a map cell.
// For best results, edit the default Horizontal and Vertical axes in the input manager so the gravity and sensitivity are 99.

#pragma strict
import SpriteTile;
import System.Collections.Generic;

var level : TextAsset;
var gemText : GUIText;
var titleText : GUIText;
var flare : GameObject;
var gemColors = [Color.red, Color(1, .6, 0), Color.yellow, Color.green, Color.cyan, Color.blue, Color.magenta];
var animFrames : Sprite[];		// All the walk animation frames
var animFrameCount : int = 5;	// Number of frames in a walk animation
var animSpeed : float = 10.0;
var idleFrames : Sprite[];
var doorOpenSound : AudioClip;
var doorCloseSound : AudioClip;
var gemCollectSound : AudioClip;
var gameCompleteSound : AudioClip;
var footstepSounds : AudioClip[];

private var bridgeTriggerOn = 1;
private var bridgeTriggerOff = 2;
private var doorTriggerOn1 = 10;
private var doorTriggerOff1 = 11;
private var doorTriggerOn2 = 20;
private var doorTriggerOff2 = 21;
private var doorClosed = TileInfo(4, 62);
private var doorOpen = TileInfo(4, 63);
private var gem = TileInfo(4, 71);

private var gemCount = 0;
private var totalGemCount : int;

private var storedPositions1 : List.<Int2>;
private var storedRoofTiles1 : TileInfo[];
private var storedPositions2 : List.<Int2>;
private var storedRoofTiles2 : TileInfo[];

private var animTimer = 0.0;
private var timerTrigger = 0.0;
private var footstepIndex : int;
private var pitchAlternate = 0.0;

private var spriteRenderer : SpriteRenderer;
private var audioSources : AudioSource[];

function Start () : IEnumerator {
	Cursor.visible = false;
	Tile.SetCamera();
	Tile.LoadLevel (level);
	Tile.UseTrueColor (false);	// Save some memory by using less accurate color
	
	// Animate water
	Tile.AnimateTileRange (TileInfo(4, 16), 8, 8.0);
	// Animate gems
	Tile.AnimateTile (TileInfo(4, 71), 9, 15.0);
	
	// Store all roof tiles in a pair of arrays; roof tiles have all been set with a trigger value of 100 or 200
	Tile.GetTriggerPositions (100, 2, storedPositions1);
	storedRoofTiles1 = new TileInfo[storedPositions1.Count];
	for (var i = 0; i < storedRoofTiles1.Length; i++) {
		storedRoofTiles1[i] = Tile.GetTile (storedPositions1[i], 2);
	}
	Tile.GetTriggerPositions (200, 2, storedPositions2);
	storedRoofTiles2 = new TileInfo[storedPositions2.Count];
	for (i = 0; i < storedRoofTiles2.Length; i++) {
		storedRoofTiles2[i] = Tile.GetTile (storedPositions2[i], 2);
	}
	
	// Set gem colors, by getting the positions of all gems, then setting colors at those positions
	var gemPositions : List.<Int2>;
	Tile.GetTilePositions (gem, 1, gemPositions);
	totalGemCount = gemPositions.Count;
	if (totalGemCount > gemColors.Length) {
		Debug.LogError ("Number of gems in level (" + totalGemCount + ") exceeds gemColors array length (" + gemColors.Length + ")");
		return;
	}
	for (i = 0; i < gemPositions.Count; i++) {
		var thisColor = gemColors[i];
		thisColor.a = .75;	// Make gems a little transparent
		Tile.SetColor (gemPositions[i], 1, thisColor);
	}
	
	// Set up function that runs when character starts moving from one cell to another
	GetComponent (CharacterControl).SetStartMoveDelegate (StartMove);
	// Set up function that is called every frame while character is moving to another cell
	GetComponent (CharacterControl).SetMoveDelegate (CharAnim);
	// Set up function that runs when character has finished moving across a cell
	GetComponent (CharacterControl).SetEndMoveDelegate (CheckCell);
	
	spriteRenderer = GetComponent (SpriteRenderer);
	spriteRenderer.sortingLayerName = Tile.GetSortingLayerName (1);
	
	audioSources = GetComponents.<AudioSource>();
	// We use 3 audio sources to avoid cutting off sounds...0 = footsteps, 1 = misc sounds, 2 = fanfare
	if (audioSources.Length != 3) {
		Debug.LogError ("Need 3 AudioSource components attached");
		return;
	}
	audioSources[0].volume = 0.6;
	
	SetGemText();
	titleText.text = "Find all " + totalGemCount + " gems!\nUse arrow keys to move";
	yield WaitForSeconds (5);
	titleText.gameObject.SetActive (false);
}

function SetGemText () {
	gemText.text = "Gems: " + gemCount + " / " + totalGemCount;
}

function StartMove (dir : Int2, mapPos : Int2, characterWasMoving : boolean) {
	// Delete roof tiles when entering a door. Another way to do this would be to have the roof tiles on their own layer, and just turn the
	// layer off with SetLayerActive. In this case, though, there's other stuff on layer 2 that we don't want to be deactivated. Also, this way
	// we can use two sets of triggers, and put houses close to each other on alternating sets, so entering one house doesn't reveal the other.
	var trigger = Tile.GetTrigger (mapPos + dir, 1);
	if ((trigger == doorTriggerOn1 || trigger == doorTriggerOn2) && Tile.GetTile (mapPos + dir, 1) != doorOpen) {
		var positionArray = (trigger == doorTriggerOn1)? storedPositions1 : storedPositions2;
		for (var i = 0; i < positionArray.Count; i++) {
			Tile.DeleteTile (positionArray[i], 2);
		}
		Tile.SetTile (mapPos + dir, 1, doorOpen);
		audioSources[1].clip = doorOpenSound;
		audioSources[1].Play();
	}
	
	footstepIndex = 0;	// Default grass footstep index
	var thisTile = Tile.GetTile (mapPos + dir, 2);	// Get tile from layer 2 first to check for bridge
	if (thisTile.tile >= 34 && thisTile.tile <= 36) {	// Bridge
		footstepIndex = 3;
	}
	else {
		thisTile = Tile.GetTile (mapPos + dir, 1);	// Get tile from layer 1 if not on bridge
		if (thisTile == TileInfo.empty || thisTile.tile == 71) {	// If walking on empty or gem, get tile from layer 0 instead
			thisTile = Tile.GetTile (mapPos + dir, 0);
		}
		if (thisTile.tile >= 24 && thisTile.tile <= 26) {	// Gravel
			footstepIndex = 1;
		}
		else if (thisTile.tile == 63 || thisTile.tile == 69) {	// Floor
			footstepIndex = 2;
		}
	}
	
	if (!characterWasMoving) {	// Reset animation timer if character was stopped and is just starting to move now
		animTimer = 0.0;
		timerTrigger = 0.0;
	}
}

function CharAnim (dir : Int2, t : float, speedFactor : float) {
	animTimer = (animTimer + Time.deltaTime*animSpeed*speedFactor) % animFrameCount;
	
	// Add a number to the index depending on which way the character is facing, to get the right anim sequence (0-4 = down, 5-9 = up, etc.)
	var frameOffset = GetDirectionIndex (dir);
	var index = Mathf.Min (parseInt(animTimer), animFrameCount-1);	// Clamp anim index just in case, to prevent going out of range
	spriteRenderer.sprite = animFrames[index + frameOffset*animFrameCount];
	
	if (animTimer >= timerTrigger && animTimer < timerTrigger + animFrameCount/2.0) {
		audioSources[0].clip = footstepSounds[footstepIndex];
		pitchAlternate = 0.1 - pitchAlternate;
		audioSources[0].pitch = 1.0 - pitchAlternate;	// Alternate footsteps pitch between 1.0 and 0.9
		audioSources[0].Play();
		timerTrigger = (timerTrigger + animFrameCount/2.0) % animFrameCount;
	}
}

function GetDirectionIndex (dir : Int2) : int {
	var dirIndex = 0;	// Down
	if (dir == Int2.up) {
		dirIndex = 1;
	}
	else if (dir == Int2.left) {
		dirIndex = 2;
	}
	else if (dir == Int2.right) {
		dirIndex = 3;
	}
	return dirIndex;
}

function CheckCell (dir : Int2, mapPos : Int2, characterIsMoving : boolean) {
	// Set idle frame based on direction, if stopitchAlternateed
	if (!characterIsMoving) {
		var dirFrame = GetDirectionIndex (dir);
		spriteRenderer.sprite = idleFrames[dirFrame];
	}
	
	// See if we got a gem
	if (Tile.GetTile (mapPos, 1) == gem) {
		Tile.DeleteTile (mapPos, 1);
		if (++gemCount == totalGemCount) {
			EndGame();
		}
		SetGemText();
		AnimateFlare();
		audioSources[1].clip = gemCollectSound;
		audioSources[1].Play();
		return;
	}
	
	var trigger = Tile.GetTrigger (mapPos, 1);
	// Set sorting layer when encountering bridges (which are on layer 2)
	if (trigger == bridgeTriggerOn) {
		spriteRenderer.sortingLayerName = Tile.GetSortingLayerName (2);
	}
	else if (trigger == bridgeTriggerOff) {
		spriteRenderer.sortingLayerName = Tile.GetSortingLayerName (1);
	}
	// Restore roof tiles when exiting a door
	else if (trigger == doorTriggerOff1 || trigger == doorTriggerOff2) {
		if (Tile.GetTile (mapPos + Int2.up, 1) != doorClosed) {
			var positionArray = (trigger == doorTriggerOff1)? storedPositions1 : storedPositions2;
			var roofTileArray = (trigger == doorTriggerOff1)? storedRoofTiles1 : storedRoofTiles2;
			for (var i = 0; i < positionArray.Count; i++) {
				Tile.SetTile (positionArray[i], 2, roofTileArray[i]);
			}
			Tile.SetTile (mapPos + Int2.up, 1, doorClosed);
			audioSources[1].clip = doorCloseSound;
			audioSources[1].Play();
		}
	}
}

function AnimateFlare () : IEnumerator {
	flare.SetActive (true);
	flare.GetComponent (SpriteRenderer).sortingLayerName = Tile.GetSortingLayerName (1);
	flare.transform.position = transform.position;
	flare.transform.localScale = Vector3.zero;
	yield AnimateTransformScale (flare.transform, Vector3.zero, Vector3(.3, .3, 0));
	yield AnimateTransformScale (flare.transform, Vector3(.3, .3, 0), Vector3.zero);
	flare.SetActive (false);
}

function AnimateTransformScale (thisTransform : Transform, startScale : Vector3, endScale : Vector3) : IEnumerator {
	var t = 0.0;
	while (t < 1.0) {
		t += Time.deltaTime*4;
		thisTransform.localScale = Vector3.Lerp (startScale, endScale, Mathf.SmoothStep(0.0, 1.0, t));
		yield;
	}
}

function EndGame () : IEnumerator {
	audioSources[2].clip = gameCompleteSound;
	audioSources[2].Play();
	titleText.gameObject.SetActive (true);
	titleText.text = "Congratulations!\nYou found all the gems!";
	yield WaitForSeconds (10);
	titleText.gameObject.SetActive (false);	
}