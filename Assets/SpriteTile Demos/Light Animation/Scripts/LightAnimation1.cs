using UnityEngine;
using SpriteTile;

public class LightAnimation1 : MonoBehaviour {

	public Transform anchor;
	Int2 lastPos;
	
	void Start () {
		lastPos = Tile.WorldToMapPosition (transform.position);
		Tile.SetLight (lastPos, 0);
		
		// Set up rope
		GetComponent<LineRenderer>().sortingOrder = 1;
		GetComponent<LineRenderer>().SetPosition (0, anchor.position);
	}
	
	void Update () {
		// Move the light if the world position of the sprite has moved to a new tile
		var newPos = Tile.WorldToMapPosition (transform.position);
		if (newPos != lastPos) {
			Tile.MoveLight (lastPos, newPos);
			lastPos = newPos;
		}
		
		GetComponent<LineRenderer>().SetPosition (1, transform.position);
	}
}