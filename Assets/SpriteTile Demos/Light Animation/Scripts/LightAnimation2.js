#pragma strict
import SpriteTile;
import System.Collections.Generic;

function Start () {
	// Use triggers set in the level to get random positions for the light
	var positions : List.<Int2>;
	Tile.GetTriggerPositions (1, positions);
	var spriteRenderer = GetComponent(SpriteRenderer);
	var wait = new WaitForSeconds (1.5);
	
	while (true) {
		var thisPos = positions[Random.Range(0, positions.Count)];
		Tile.SetLight (thisPos, 1);
		// Put sprite at map location of the light
		transform.position = Tile.MapToWorldPosition (thisPos);
		
		// Increase brightness over 1 second for both sprite and tile light
		for (var i = 0.0; i < 1.0; i += Time.deltaTime) {
			spriteRenderer.color.a = i;
			Tile.SetLight (thisPos, i);
			yield;
		}
		
		yield wait;
		
		// Decrease brightness over 1 second
		for (i = 1.0; i > 0.0; i -= Time.deltaTime) {
			spriteRenderer.color.a = i;
			Tile.SetLight (thisPos, i);
			yield;
		}
		
		Tile.DeleteLight (thisPos);
	}
}