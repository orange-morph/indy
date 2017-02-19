// Scroll endlessly through sections of a map
// Demonstrates procedurally making a level through code by copying from an existing level
using UnityEngine;
using SpriteTile;
using UnityEngine.UI;
using System.Collections.Generic;

public class EndlessScroll : MonoBehaviour {

	public int mapSections = 4;
	public float scrollSpeed = 6.0f;
	public TextAsset level;
	public Text text1;
	public Text text2;
	
	private int limit;
	private int sectionWidth;
	private int sectionNumber;
	private Vector3 cameraStartPos;
	private bool advancing = false;
	
	void Start () {
		// The level was made with 16:9 or narrower screens in mind, and wider screens would ruin the seamless effect
		if (Camera.main.aspect > 16.0f/9.0f) {
			Camera.main.aspect = 16.0f/9.0f;
		}
		Tile.SetCamera();
		Tile.LoadLevel (level);
		
		// Each section is marked by a trigger, so use the number of triggers to determine the number of sections
		var triggers = new List<Int2>();
		Tile.GetTriggerPositions (1, ref triggers);
		mapSections = triggers.Count;
		// Get width of each section based on how many map sections there are
		var mapSize = Tile.GetMapSize();
		sectionWidth = mapSize.x / mapSections;
		limit = sectionWidth*2 - 8;	// Camera will be moved back when it hits this limit
		
		// Get map data for each section
		var mapDataSections = new MapData[mapSections];
		var startPos = Int2.zero;
		var endPos = new Int2(sectionWidth - 1, mapSize.y - 1);
		for (int i = 0; i < mapSections; i++) {
			mapDataSections[i] = Tile.GetMapBlock (startPos, endPos);
			startPos.x += sectionWidth;
			endPos.x += sectionWidth;
		}
		
		// Make a new level that's twice as wide as the loaded level
		mapSize.x *= 2;
		Tile.NewLevel (mapSize, 0, 1.0f, 0.0f, LayerLock.None);
		Tile.SetAmbient (new Color(.4f, .4f, .4f, 1));
		
		// Paste each section into the new level twice, so scrolling can be seamless
		startPos.x = 0;
		for (int i = 0; i < mapSections; i++) {
			Tile.SetMapBlock (startPos, mapDataSections[i]);
			Tile.SetMapBlock (startPos + new Int2(sectionWidth, 0), mapDataSections[i]);
			startPos.x += sectionWidth * 2;
		}
		
		cameraStartPos = transform.position;
		sectionNumber = 1;
	}
	
	void Update () {
		transform.Translate (Vector3.right * scrollSpeed * Time.deltaTime);
		// Move camera back by exactly the width of a section for endless, seamless scrolling
		if (transform.position.x >= limit) {
			transform.Translate (Vector3.right * -sectionWidth);
		}
		
		// Press space to advance to next section
		// Not allowed if we're advancing to next section or have reached the last section
		if (Input.GetKeyDown (KeyCode.Space) && !advancing && sectionNumber < mapSections) {
			limit += sectionWidth*2;
			advancing = true;
		}
		
		text1.text = "Section " + sectionNumber;
		if (advancing) {
			// Check to see if we've advanced to the next section
			text2.text = "Advancing to section " + (sectionNumber + 1);
			if (transform.position.x > sectionNumber * sectionWidth*2) {
				advancing = false;
				sectionNumber++;
			}
		}
		else {
			// Indicate what controls are allowed
			if (sectionNumber < mapSections) {
				text2.text = "Press space to advance to the next section";
			}
			else {
				text2.text = "Press R to reset";
			}
		}
		
		// Reset back to the beginning
		if (Input.GetKeyDown (KeyCode.R)) {
			transform.position = cameraStartPos;
			limit = sectionWidth*2 - 8;
			sectionNumber = 1;
			advancing = false;
		}
	}
}