#pragma strict
import SpriteTile;
import UnityEngine.UI;
import System.Collections.Generic;

var level : TextAsset;
var physicsMaterial : PhysicsMaterial2D;
var text : Text;

function Awake () {
	Tile.SetCamera();
	Tile.LoadLevel (level);
	Tile.SetColliderMaterial (physicsMaterial);
	
	// Get positions of lifts in the level, and set speed using the float property
	var positions : List.<Int2>;
	Tile.GetPropertyPositions (positions);
	for (var i = 0; i < positions.Count; i++) {
		var go = Tile.GetProperty.<GameObject> (positions[i]);
		go.GetComponent(Lift).moveSpeed = Tile.GetProperty.<float> (positions[i]);
	}
	
	FadeText();
}

function FadeText () : IEnumerator {
	yield WaitForSeconds (5.0);
	var t = 1.0;
	while (t >= 0.0) {
		t -= Time.deltaTime;
		text.color.a = t;
		yield;
	}
}