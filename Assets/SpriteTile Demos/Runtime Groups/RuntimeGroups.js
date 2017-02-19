// Demonstrates a few ways of using terrain groups and random groups at runtime
#pragma strict
import SpriteTile;
import System.Collections.Generic;

var groups : TextAsset;

function Start () {
	// Set up a 10 x 10 level
	Tile.SetCamera();
	var levelSize = Int2(10, 10);
	Tile.NewLevel (levelSize, 0, 1.0, 0.0, LayerLock.None);
	Tile.LoadGroups (groups);

	var positions1 = [Int2(3, 4), Int2(4, 4), Int2(3, 5), Int2(4, 5), Int2(5, 4), Int2(5, 5), Int2(3, 6), Int2(4, 6), Int2(5, 6), Int2(3, 3), Int2(4, 3), Int2(5, 3), Int2(5, 2), Int2(4, 2), Int2(3, 2)];
	var positions2 = [Int2(5, 4), Int2(4, 4), Int2(3, 4)];
	
	// Draw some tiles one at a time, using a terrain group. The tile (4, 55) is part of the terrain group we're using here
	Tile.UseTerrainGroup (true, 0, 1);
	for (var i = 0; i < positions1.Length; i++) {
		Tile.SetTile (positions1[i], 4, 55);
		yield WaitForSeconds (.3);
	}
	// Draw some more tiles using another terrain group
	Tile.UseTerrainGroup (true, 0, 0);
	for (i = positions1.Length-1; i >= 0; i--) {
		Tile.SetTile (positions1[i], 4, 0);
		yield WaitForSeconds (.3);
	}
	// Delete a few tiles
	for (i = 0; i < positions2.Length; i++) {
		Tile.DeleteTile (positions2[i]);
		yield WaitForSeconds (.3);
	}
	// From now on, the terrain groups won't be used
	Tile.UseTerrainGroup (false, 0, 0);
	Tile.UseTerrainGroup (false, 0, 1);
	
	// Set up a list of all possible positions in the level
	var positions = new List.<Int2>(levelSize.x * levelSize.y);
	for (var y = 0; y < levelSize.y; y++) {
		for (var x = 0; x < levelSize.x; x++) {
			positions.Add (Int2(x, y));
		}
	}
	yield WaitForSeconds (.5);
	// Draw some random tiles at random positions using random group (0, 1)
	Tile.UseRandomGroup (true, 0, 1);
	while (positions.Count > 0) {
		var pos = Random.Range (0, positions.Count);
		Tile.SetTile (positions[pos], 0, 0);	// Since UseRandomGroup is active, it doesn't matter what the actual tile number is, so we just use 0, 0
		positions.RemoveAt (pos);
		yield WaitForSeconds (.015);
	}
	
	yield WaitForSeconds (.5);
	// Deactivate the random group and draw a non-random tile block
	Tile.UseRandomGroup (false);
	Tile.SetTileBlock (Int2.zero, Int2(levelSize.x - 1, levelSize.y - 1), 4, 0);
	yield WaitForSeconds (1.5);
	// Activate another random group and draw a block of random tiles
	Tile.UseRandomGroup (true, 0, 3);
	Tile.SetTileBlock (Int2.zero, Int2(levelSize.x - 1, levelSize.y - 1), 0, 0);
	yield WaitForSeconds (1.5);
	// Delete everything...technically setting UseRandomGroup to false won't make any difference here since we're done now,
	// but it's good practice to turn it off when not needed
	Tile.UseRandomGroup (false);
	Tile.DeleteTileBlock (Int2.zero, Int2(levelSize.x - 1, levelSize.y - 1));
}