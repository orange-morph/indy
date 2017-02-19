// Demonstrates a few ways of using terrain groups and random groups at runtime
using UnityEngine;
using SpriteTile;
using System.Collections;
using System.Collections.Generic;

public class RuntimeGroups : MonoBehaviour {

	public TextAsset groups;
	
	IEnumerator Start () {
		// Set up a 10 x 10 level
		Tile.SetCamera();
		var levelSize = new Int2(10, 10);
		Tile.NewLevel (levelSize, 0, 1.0f, 0.0f, LayerLock.None);
		Tile.LoadGroups (groups);
	
		Int2[] positions1 = new[] {new Int2(3, 4), new Int2(4, 4), new Int2(3, 5), new Int2(4, 5), new Int2(5, 4), new Int2(5, 5), new Int2(3, 6), new Int2(4, 6), new Int2(5, 6), new Int2(3, 3), new Int2(4, 3), new Int2(5, 3), new Int2(5, 2), new Int2(4, 2), new Int2(3, 2)};
		Int2[] positions2 = new[] {new Int2(5, 4), new Int2(4, 4), new Int2(3, 4)};
		
		// Draw some tiles one at a time, using a terrain group. The tile (4, 55) is part of the terrain group we're using here
		Tile.UseTerrainGroup (true, 0, 1);
		for (int i = 0; i < positions1.Length; i++) {
			Tile.SetTile (positions1[i], 4, 55);
			yield return new WaitForSeconds (.3f);
		}
		// Draw some more tiles using another terrain group
		Tile.UseTerrainGroup (true, 0, 0);
		for (int i = positions1.Length-1; i >= 0; i--) {
			Tile.SetTile (positions1[i], 4, 0);
			yield return new WaitForSeconds (.3f);
		}
		// Delete a few tiles
		for (int i = 0; i < positions2.Length; i++) {
			Tile.DeleteTile (positions2[i]);
			yield return new WaitForSeconds (.3f);
		}
		// From now on, the terrain groups won't be used
		Tile.UseTerrainGroup (false, 0, 0);
		Tile.UseTerrainGroup (false, 0, 1);
		
		// Set up a list of all possible positions in the level
		var positions = new List<Int2>(levelSize.x * levelSize.y);
		for (int y = 0; y < levelSize.y; y++) {
			for (int x = 0; x < levelSize.x; x++) {
				positions.Add (new Int2(x, y));
			}
		}
		yield return new WaitForSeconds (.5f);
		// Draw some random tiles at random positions using random group (0, 1)
		Tile.UseRandomGroup (true, 0, 1);
		while (positions.Count > 0) {
			var pos = Random.Range (0, positions.Count);
			Tile.SetTile (positions[pos], 0, 0);	// Since UseRandomGroup is active, it doesn't matter what the actual tile number is, so we just use 0, 0
			positions.RemoveAt (pos);
			yield return new WaitForSeconds (.015f);
		}
		
		yield return new WaitForSeconds (.5f);
		// Deactivate the random group and draw a non-random tile block
		Tile.UseRandomGroup (false);
		Tile.SetTileBlock (Int2.zero, new Int2(levelSize.x - 1, levelSize.y - 1), 4, 0);
		yield return new WaitForSeconds (1.5f);
		// Activate another random group and draw a block of random tiles
		Tile.UseRandomGroup (true, 0, 3);
		Tile.SetTileBlock (Int2.zero, new Int2(levelSize.x - 1, levelSize.y - 1), 0, 0);
		yield return new WaitForSeconds (1.5f);
		// Delete everything...technically setting UseRandomGroup to false won't make any difference here since we're done now,
		// but it's good practice to turn it off when not needed
		Tile.UseRandomGroup (false);
		Tile.DeleteTileBlock (Int2.zero, new Int2(levelSize.x - 1, levelSize.y - 1));
	}
}