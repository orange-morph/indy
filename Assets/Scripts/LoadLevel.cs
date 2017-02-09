using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpriteTile;

public class LoadLevel : MonoBehaviour
{

	public TextAsset level;
    
    void Start() {
        Tile.SetCamera();
        Tile.LoadLevel(level);
        //Tile.AnimateTileRange(new TileInfo(0, 93), 8, 2.0f); // Animate from set 0, tile 93 for 8 tiles, at a rate of 2 frames per second
        Tile.AnimateTileRange(new TileInfo(6, 0), 10, 1.0f); // Animate from set 6, tile 12 for 18 tiles, at a rate of 5 frames per second
    }

}
