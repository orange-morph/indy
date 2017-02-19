using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using SpriteTile;

public class DemoManager : MonoBehaviour {

	public TextAsset level;
	public PhysicsMaterial2D physicsMaterial;
	public Text text;
	
	void Awake () {
		Tile.SetCamera();
		Tile.LoadLevel (level);
		Tile.SetColliderMaterial (physicsMaterial);
		
		List<Int2> positions = null;
		Tile.GetPropertyPositions (ref positions);
		for (int i = 0; i < positions.Count; i++) {
			var go = Tile.GetProperty<GameObject> (positions[i]);
			go.GetComponent<Lift>().moveSpeed = Tile.GetProperty<float> (positions[i]);
		}
		
		StartCoroutine (FadeText());
	}
	
	IEnumerator FadeText () {
		yield return new WaitForSeconds (5.0f);
		var t = 1.0f;
		while (t >= 0.0f) {
			t -= Time.deltaTime;
			var color = text.color;
			color.a = t;
			text.color = color;
			yield return null;
		}
	}
}