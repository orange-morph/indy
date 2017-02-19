using UnityEngine;
using SpriteTile;

public class Lift : MonoBehaviour {

	public float moveSpeed;
	
	private Int2 lastPos;
	
	void Start () {
		lastPos = Tile.WorldToMapPosition (transform.position);
	}
	
	void Update () {
		transform.Translate (Vector3.up * moveSpeed * Time.deltaTime);
		var newPos = Tile.WorldToMapPosition (transform.position);
		// When moving to a new tile, check colliders 3 tiles up (across the width of the lift) and below, and reverse direction when appropriate
		if (newPos != lastPos) {
			lastPos = newPos;
			if ((moveSpeed > 0.0f && (Tile.GetCollider (newPos + Int2.up*3) || Tile.GetCollider (newPos + Int2.upLeft + Int2.up*2) || Tile.GetCollider (newPos + Int2.upRight + Int2.up*2))) ||
				(moveSpeed < 0.0f && Tile.GetCollider (newPos))) {
				moveSpeed = -moveSpeed;
			}
			// Pick up color from map
			GetComponent<SpriteRenderer>().color = Tile.GetColor (newPos);
		}
	}
}