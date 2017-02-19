// This simply loads in a level and allows you to scroll around it by using the arrow keys

using UnityEngine;
using SpriteTile;

public class LevelViewer : MonoBehaviour {
	public float moveSpeed = 10.0f;
	public TextAsset level;
	
	void Start () {
		Tile.SetCamera();
		Tile.LoadLevel (level);
	}
	
	void Update () {
		transform.Translate (new Vector2(Input.GetAxis ("Horizontal") * moveSpeed * Time.deltaTime,
										 Input.GetAxis ("Vertical") * moveSpeed * Time.deltaTime));
	}
}