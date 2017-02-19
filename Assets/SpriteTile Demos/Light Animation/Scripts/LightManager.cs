using UnityEngine;
using SpriteTile;

public class LightManager : MonoBehaviour {

	public TextAsset level;
	
	void Awake () {
		Tile.SetCamera();
		Tile.LoadLevel (level);
	}
}
