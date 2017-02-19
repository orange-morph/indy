using UnityEngine;
using SpriteTile;
using System.IO;

public class TileDraw : MonoBehaviour {

	public Vector2 levelSize = new Vector2(24, 16);
	public float tileSize = 1.0f;
	string fileName = "MyLevel";
	
	void Start () {
		transform.position = new Vector3(levelSize.x/2 - tileSize/2, levelSize.y/2 - tileSize/2, transform.position.z);
		Tile.SetCamera();
		Tile.NewLevel (new Int2(levelSize), 0, tileSize, 0.0f, LayerLock.None);
		Tile.SetTileBlock (Int2.zero, new Int2(levelSize)-Int2.one, 3, 0);
	}
	
	void Update () {
		if (Input.GetMouseButton(0) || Input.GetMouseButton(1)) {
			Int2 mapPos;
			if (Tile.ScreenToMapPosition (Input.mousePosition, out mapPos)) {
				Tile.SetTile (mapPos, 3, Input.GetMouseButton(0)? 1 : 0);
			}
		}
	}
	
	void OnGUI () {
		GUILayout.BeginVertical();
		GUILayout.Label ("Left-click to draw, right-click to erase");
		GUILayout.BeginHorizontal();
		if (GUILayout.Button ("Save", GUILayout.Width(50))) {
			SaveLevel();
		}
		GUILayout.Label ("Application.dataPath + ");
		fileName = GUILayout.TextField (fileName, GUILayout.Width(100));
		GUILayout.EndHorizontal();
		GUILayout.EndVertical();
	}
	
	void SaveLevel () {
#if UNITY_WEBGL || UNITY_WEBPLAYER
		Debug.LogError ("Saving external files doesn't work in web players");
#else
		var levelBytes = Tile.GetLevelBytes();
		var thisFileName = fileName;
		if (!thisFileName.EndsWith (".bytes")) {
			thisFileName += ".bytes";
		}
#endif
		File.WriteAllBytes (Application.dataPath + "/" + thisFileName, levelBytes);
#if UNITY_EDITOR
		UnityEditor.AssetDatabase.Refresh();
#endif
	}
}