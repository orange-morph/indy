using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SpriteTile;

public class LightAnimation2 : MonoBehaviour {

	IEnumerator Start () {
		// Use triggers set in the level to get random positions for the light
		var positions = new List<Int2>();
		Tile.GetTriggerPositions (1, ref positions);
		var spriteRenderer = GetComponent<SpriteRenderer>();
		var wait = new WaitForSeconds (1.5f);
		var color = spriteRenderer.color;
		
		while (true) {
			var thisPos = positions[Random.Range(0, positions.Count)];
			Tile.SetLight (thisPos, 1);
			// Put sprite at map location of the light
			transform.position = Tile.MapToWorldPosition (thisPos);
			
			// Increase brightness over 1 second for both sprite and tile light
			for (float i = 0.0f; i < 1.0f; i += Time.deltaTime) {
				color.a = i;
				spriteRenderer.color = color;
				Tile.SetLight (thisPos, i);
				yield return null;
			}
			
			yield return wait;
			
			// Decrease brightness over 1 second
			for (float i = 1.0f; i > 0.0f; i -= Time.deltaTime) {
				color.a = i;
				spriteRenderer.color = color;
				Tile.SetLight (thisPos, i);
				yield return null;
			}
			
			Tile.DeleteLight (thisPos);
		}
	}
}