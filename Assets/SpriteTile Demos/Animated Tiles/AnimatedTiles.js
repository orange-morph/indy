#pragma strict
import SpriteTile;

// In addition to animation, this demo shows the usage of layers to reduce the number of tiles;
// specifically the grass edge tiles work with both the water and path tiles.

var level : TextAsset;

function Start () : IEnumerator {
	Tile.SetCamera();
	Tile.LoadLevel (level);
	
	// This line animates the water tiles at 8.0fps, which start at tile #16 in set 4 and have a total of 8 tiles.
	Tile.AnimateTileRange (TileInfo(4, 16), 8, 8.0);
	
	// This line animates the #71 gem tile at 15.0fps, cycling through the 9 gem tiles.
	Tile.AnimateTile (TileInfo(4, 71), 9, 15.0);
	
	// This line animates the #79 gem tile. We want it to cycle through 5 specific gem tiles at 2fps,
	// rather than a range, so we specify the tiles in an array.
	var tiles = [TileInfo(4, 79), TileInfo(4, 77), TileInfo(4, 75), TileInfo(4, 73), TileInfo(4, 71)];
	Tile.AnimateTile (TileInfo(4, 79), tiles, 2.0);
	
	while (true) {
		// This line animates the tile at position (8, 10) on layer 1, where it cycles through the 9 gem tiles at 30 fps using the ping-pong animation type.
		// Note that the tile in the map starts as the #72 gem tile; this way the previous AnimateTile calls for the #71 and #79 gem tiles won't affect it.
		// That's because AnimateTile animates all tiles of the specified TileInfo. AnimateTilePosition by contrast only animates the tile at the specified position.
		Tile.AnimateTilePosition (Int2(8, 10), 1, TileInfo(4, 71), 9, 30.0, AnimType.PingPong);
		// Now we wait a second then stop animating the tile.
		yield WaitForSeconds (1);
		Tile.StopAnimatingTilePosition (Int2(8, 10), 1);
		// This does the same as above, but the next tile over.
		Tile.AnimateTilePosition (Int2(9, 10), 1, TileInfo(4, 71), 9, 30.0, AnimType.PingPong);
		yield WaitForSeconds (1);
		Tile.StopAnimatingTilePosition (Int2(9, 10), 1);
		// The while loop continues this seqeuence forever.
	}
}