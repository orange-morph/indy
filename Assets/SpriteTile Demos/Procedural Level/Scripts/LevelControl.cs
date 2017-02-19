// This creates a level procedurally using Perlin noise

using UnityEngine;
using SpriteTile;

public class LevelControl : MonoBehaviour {

	public Vector2 levelSize = new Vector2(100, 100);
	public float perlinSize = 5.0f;
	public float cutoff = 0.4f;
	public int seed = 1;
	public Transform target;
	string seedString;

	void Awake () {
		Tile.SetCamera();
		Tile.NewLevel (new Int2(levelSize), 0, 1.0f, 0.0f, LayerLock.None);
		
		CreateLevel (seed);
		seedString = seed.ToString();
	}
	
	void OnGUI () {
		GUI.color = Color.black;
		GUI.Label (new Rect(10, 10, 100, 25), "Level number:");
		GUI.color = Color.white;
		var chr = Event.current.character;
		if (chr == '\n') {
			GUI.FocusControl ("Dummy");
		}
		// Prevent non-numerical characters from being input
		if (chr < '0' || chr > '9') {
			Event.current.character = '\0';
		}
		seedString = GUI.TextField (new Rect(100, 10, 40, 25), seedString, 4);
		GUI.SetNextControlName ("Dummy");
		GUI.Button (new Rect(10000, 0, 120, 10), " ");
		if (GUI.Button (new Rect(10, 50, 130, 30), "Change level")) {
			CreateLevel (int.Parse(seedString));
		}
	}
	
	void Update () {
		// See if user clicks in the map, but not in upper-left corner where controls are
		if (Input.GetMouseButtonDown(0) && !new Rect(0, Screen.height-85, 150, 85).Contains (Input.mousePosition)) {
			var mapPos = Int2.zero;
			if (Tile.ScreenToMapPosition (Input.mousePosition, out mapPos)) {
				if (new Rect (1, 1, levelSize.x-2, levelSize.y-2).Contains (mapPos.ToVector2())) {	// Make sure user can't click on outer walls
					// Clicking on a floor tile changes it to a wall tile, with collision
					if (Tile.GetTile (mapPos) == new TileInfo(3, 0)) {
						Tile.SetTile (mapPos, new TileInfo(3, 1));
						Tile.SetCollider (mapPos, true);
					}
					// Clicking on a wall tile changes it to a floor tile, and removes collision
					else {
						Tile.SetTile (mapPos, new TileInfo(3, 0));
						Tile.SetCollider (mapPos, false);
					}
				}
			}
		}
	}
	
	void CreateLevel (int i) {
		// Create caves
		i *= 1000;
		var maxSize = new Int2((int)levelSize.x - 1, (int)levelSize.y - 1);
		for (int y = 0; y <= maxSize.y; y++) {
			for (int x = 0; x <= maxSize.x; x++) {
				if (Mathf.PerlinNoise ((x + i)/perlinSize, (y + i)/perlinSize) < cutoff) {
					Tile.SetTile (new Int2(x, y), 3, 1, true);
				}
				else {
					Tile.SetTile (new Int2(x, y), 3, 0, false);
				}
			}
		}
	
		// Create walls around the level	
		Tile.SetBorder (new TileInfo(3, 1), true);
		
		// Make sure character is placed in an empty cell, within bounds of the level
		var pos = new Int2(levelSize/2);
		var placed = false;
		while (!placed && pos.x < levelSize.x-2 && pos.y < levelSize.y-2) {
			if (Tile.GetCollider (pos)) {
				pos += Int2.one;
				continue;
			}
			placed = true;
		}
		target.position = new Vector2(pos.x, pos.y);
		target.SendMessage("SetMapPosition");
	}
}