#pragma strict
import SpriteTile;

var titleLevel : TextAsset;
var gameLevels : TextAsset[];
var fader : GameObject;
var acorn : GameObject;
var title : GameObject;
var titleText : GameObject;
var livesText : GameObject;
var levelText : GameObject;
var goText : GameObject;
var gameOverText : GameObject;
var initialLives = 3;
var initialLevel = 0;
var followSpring = 4.5;
var levelMaterial : PhysicsMaterial2D;
var cameraStartPos = Vector3(11, 7, -10);

private var lives : int;
private var level : int;
private var player : AcornControl;
private var gameOver : boolean;
private var titleActive : boolean;
private var maxCamPosX : float;
private var standardVolume : float;
private var cam : Transform;

enum FadeType {In, Out};

function Start () {
	Application.targetFrameRate = 60;
	transform.position = cameraStartPos;
	RenderSettings.ambientLight = Color(.4, .4, .4);
#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6
	Screen.showCursor = false;
#else
	Cursor.visible = false;
#endif
	standardVolume = GetComponent(AudioSource).volume;

	Tile.SetCamera();
	Tile.SetTileScale (1.001);	// Ensure there are never any gaps between tiles
	cam = Camera.main.transform;
	Tile.SetColliderMaterial (levelMaterial);
	player = Instantiate (acorn).GetComponent (AcornControl);		
	
	GameLoop();
}

function GameLoop () : IEnumerator {
	while (true) {
		yield TitleScreen();
		yield PlayGame();
		yield GameOver();
	}
}

function TitleScreen () : IEnumerator {
	GetComponent(AudioSource).volume = standardVolume;
	GetComponent(AudioSource).Play();
	gameOver = false;
	LoadTitleLevel();
	titleActive = true;
	AnimateTitle();
	yield Fade (FadeType.In, 0.5);
	
	while (!Input.GetMouseButtonDown (0)) {
		yield;
	}
	
	yield Fade (FadeType.Out, 0.5);
	titleActive = false;
	title.SetActive (false);
	titleText.SetActive (false);
}

function LoadTitleLevel () {
	cam.position = cameraStartPos;
	Tile.LoadLevel (titleLevel);
	SetLayerPositions();
	cam.Translate (Vector2.up * -0.3);
	title.SetActive (true);
	titleText.SetActive (true);
	livesText.SetActive (false);
	levelText.SetActive (false);
	player.gameObject.SetActive (false);
}

function SetLayerPositions () {
	Tile.SetLayerPosition (1, Vector2(-10.0, -3.7));
	Tile.SetLayerPosition (0, Vector2(10.0, 6.6));
}

function AnimateTitle () : IEnumerator {
	while (titleActive) {
		var t = (Mathf.Sin (Time.time * 5) / 8 + .35) + (Mathf.Sin (Time.time * 2) / 12 + .35);
		title.transform.localScale = Vector3(t, t, t*2);
		yield;
	}
}

function PlayGame () : IEnumerator {
	lives = initialLives;
	livesText.SetActive (true);
	UpdateTextObject (livesText, "Lives: " + lives);

	level = initialLevel;
	levelText.SetActive (true);
	UpdateTextObject (levelText, "Level: " + (level + 1));	

	LoadLevel (level);	
	yield Fade (FadeType.In, 0.5);
	yield SpawnAcorn (false);
	FlashGo();
	
	while (!gameOver) {
		yield;
	}
}

function LoadLevel (levelNum : int) {
	cam.position = cameraStartPos;
	Tile.LoadLevel (gameLevels[levelNum]);
	SetLayerPositions();
	maxCamPosX = (Tile.GetMapSize (2).x - 9.0) * Tile.GetTileSize (2).x;
}

function LoseLife () : IEnumerator {
	lives--;
	UpdateTextObject (livesText, "Lives: " + lives);
	
	while (player.dying) {
		yield;
	}
	yield WaitForSeconds (0.5);
	
	if (lives > 0) {
		SpawnAcorn (true);
	}
	else {
		gameOver = true;
	}
}

function GameOver () : IEnumerator {
	gameOverText.SetActive (true);
	FadeMusic();
	Invoke ("GameOverSound", 0.25);
	
	// Game Over text in
	var t = 0.0;
	while (t <= 1.0) {
		t += Time.deltaTime;
		gameOverText.GetComponent(GUIText).color.a = t;
		var smoothT = Mathf.SmoothStep (0.0, 1.0, t);
		gameOverText.transform.localScale = Vector3(Mathf.Lerp(3.0, 0.3, smoothT), Mathf.Lerp(4.0, 0.4, smoothT), 1);
		yield;
	}
	
	yield WaitForSeconds (1.75);
	
	// Game Over text out
	t = 0.0;
	while (t <= 1.0) {
		t += Time.deltaTime;
		gameOverText.GetComponent(GUIText).color.a = 1.0 - t;
		smoothT = Mathf.SmoothStep (0.0, 1.0, t);
		gameOverText.transform.localScale = Vector3(Mathf.Lerp(0.3, 3.0, smoothT), Mathf.Lerp(0.4, 0.0, smoothT), 1);
		yield;
	}	
	
	yield Fade (FadeType.Out, 1.0);
	gameOverText.SetActive (false);
}

function GameOverSound () {
	gameOverText.GetComponent(AudioSource).Play();
}

function FadeMusic () : IEnumerator {
	var t = GetComponent(AudioSource).volume;
	while (t >= 0.0) {
		t -= Time.deltaTime * 0.5;
		GetComponent(AudioSource).volume = t;
		yield;
	}
}

function SpawnAcorn (moveCamera : boolean) : IEnumerator {
	if (moveCamera) {
		// Move camera back to start position
		var startPos = transform.position;
		var endPos = cameraStartPos;
		var t = 0.0;
		while (t <= 1.0) {
			t += Time.deltaTime * 1.5;
			cam.position = Vector3.Lerp (startPos, endPos, Mathf.SmoothStep (0.0, 1.0, t));		
			yield;
		}
	}
	
	player.gameObject.SetActive (true);
	player.Initialize();
	while (!player.landed) {
		yield;
	}
	player.canControl = true;
	player.levelBound = (Tile.GetMapSize (2).y - 1) * Tile.GetTileSize (2).x;
}

function Fade (fade : FadeType, time : float) : IEnumerator {
	fader.SetActive (true);
	if (fade == FadeType.Out) {
		var start = 0.0;
		var end = 0.5;
	}
	else {
		start = 0.5;
		end = 0.0;
	}
	var t = 0.0;
	var rate = 1.0 / time;
	while (t <= 1.0) {
		t += Time.deltaTime * rate;
		fader.GetComponent(GUITexture).color.a = Mathf.Lerp (start, end, t);
		yield;
	}
	fader.SetActive (false);
}

function FlashGo () : IEnumerator {
	for (var i = 0; i < 3; i++) {
		goText.SetActive (true);
		yield WaitForSeconds (0.5);
		goText.SetActive (false);
		yield WaitForSeconds (0.5);
	}
}

function UpdateTextObject (textObject : GameObject, text : String) {
	var objs = textObject.GetComponentsInChildren.<GUIText>();
	for (obj in objs) {
		obj.text = text;
	}
}

function NextLevel () : IEnumerator {
	yield Fade (FadeType.Out, 0.5);
	cam.position = cameraStartPos;
	
	level = (level + 1) % gameLevels.Length;
	UpdateTextObject (levelText, "Level: " + (level + 1));
	LoadLevel (level);
	
	// Extra life every other level, up to 10 lives max
	if (lives < 10 && level % 2 == 0) {
		lives++;
	}
	UpdateTextObject (livesText, "Lives: " + lives);
	
	yield Fade (FadeType.In, 0.5);
	yield SpawnAcorn (false);
	FlashGo();	
}

function Update () {
	if (!player.canControl) return;
	
	// Have camera follow player with a bit of lag, clamped to the level's viewable area
	var wantedPos = player.transform.position;
	wantedPos.x = Mathf.Clamp (wantedPos.x+5.5, 0.0, maxCamPosX);
	wantedPos.y = Mathf.Max (wantedPos.y, 6.5);
	wantedPos.z = transform.position.z;
	cam.position = Vector3.Lerp (transform.position, wantedPos, followSpring * Time.deltaTime);
}