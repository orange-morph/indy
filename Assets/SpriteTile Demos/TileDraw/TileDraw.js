// This sets tiles where the user clicks, and allows saving to a file in Application.dataPath

#pragma strict
import SpriteTile;
import System.IO;
import UnityEngine.GUILayout;

var levelSize = Vector2(24, 16);
var tileSize = 1.0;
private var fileName = "MyLevel";

function Start () {
	transform.position = Vector3(levelSize.x/2 - tileSize/2, levelSize.y/2 - tileSize/2, transform.position.z);
	Tile.SetCamera();
	Tile.NewLevel (Int2(levelSize), 0, tileSize, 0.0, LayerLock.None);
	Tile.SetTileBlock (Int2.zero, Int2(levelSize)-Int2.one, 3, 0);
}

function Update () {
	if (Input.GetMouseButton(0) || Input.GetMouseButton(1)) {
		var mapPos : Int2;
		if (Tile.ScreenToMapPosition (Input.mousePosition, mapPos)) {
			Tile.SetTile (mapPos, 3, Input.GetMouseButton(0)? 1 : 0);
		}
	}
}

function OnGUI () {
	BeginVertical();
	Label ("Left-click to draw, right-click to erase");
	BeginHorizontal();
	if (Button ("Save", Width(50))) {
		SaveLevel();
	}
	Label ("Application.dataPath + ");
	fileName = TextField (fileName, Width(100));
	EndHorizontal();
	EndVertical();
}

function SaveLevel () {
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