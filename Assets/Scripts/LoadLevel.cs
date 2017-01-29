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
    }

}
