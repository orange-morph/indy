using UnityEngine;
using SpriteTile;

public class TriggerControl : MonoBehaviour {

	public TextAsset triggerLevel;
	public Transform[] textObjects;
	
	bool wallSet = true;
	bool rotationSet = false;
	float rotation = 0.0f;

	void Awake () {
		Tile.SetCamera();
		Tile.LoadLevel (triggerLevel);
		
		// Set position of text objects to specific tiles
		foreach (var tObj in textObjects) {
			tObj.gameObject.SetActive (true);
			tObj.GetComponent<Renderer>().sortingOrder = 1;	// Makes text draw above tiles (0) but below the player sprite (2)
		}
		textObjects[0].position = Tile.MapToWorldPosition (new Int2(2, 17));
		textObjects[1].position = Tile.MapToWorldPosition (new Int2(3, 5));
		textObjects[2].position = Tile.MapToWorldPosition (new Int2(3, 2));
		textObjects[3].position = Tile.MapToWorldPosition (new Int2(19, 14));
		textObjects[4].position = Tile.MapToWorldPosition (new Int2(19, 5));
	}
	
	void Update () {
		if (rotationSet) {
			RotateTile();
		}
	
		var trigger = Tile.GetTrigger (transform.position);
		if (trigger == 0) return;
		
		switch (trigger) {
			case 1:
				SetWalls (false);
				break;
			case 2:
				SetWalls (true);
				break;
			case 3:
				rotationSet = true;
				break;
			case 4:
				rotationSet = false;
				break;
		}
	}
	
	void SetWalls (bool activate) {
		if (wallSet == activate) return;
		
		wallSet = activate;
		var tile = activate? 1 : 0;
		Tile.SetTileBlock (new Int2(8, 3), new Int2(8, 16), 3, tile, activate);
		Tile.SetTileBlock (new Int2(8, 3), new Int2(22, 3), 3, tile, activate);
		Tile.SetTileBlock (new Int2(8, 16), new Int2(22, 16), 3, tile, activate);
		Tile.SetTileBlock (new Int2(22, 3), new Int2(22, 16), 3, tile, activate);
	}
	
	void RotateTile () {
		rotation += Time.deltaTime * 45.0f;
		Tile.SetRotation (new Int2(15, 9), rotation);
	}
}
