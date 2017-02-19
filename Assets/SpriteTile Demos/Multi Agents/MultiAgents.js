// Makes a number of moving agents in a level, where none of the agents will move into a position occupied by another
#pragma strict
import SpriteTile;
import UnityEngine.GUILayout;

var level : TextAsset;
var prefab : GameObject;
var numberOfAgents = 20;
var moveSpeed = 2.0;

private var agents : Agent[];
private var newNumber : String;
private var newSpeed : String;

class Agent {
	var transform : Transform;
	var position : Int2;
	var direction : Int2;
}

private var directions = [Int2.right, Int2.left, Int2.up, Int2.down];

function Start () {
	Tile.SetCamera();
	Tile.LoadLevel (level);
	
	SetupAgents();
	MoveAgents();
	
	newNumber = numberOfAgents.ToString();
	newSpeed = moveSpeed.ToString();
}

function SetupAgents () {
	// Set up all agents with a random position in the level, with a random initial direction
	agents = new Agent[numberOfAgents];
	var size = Tile.GetMapSize();
	for (var i = 0; i < numberOfAgents; i++) {
		agents[i] = new Agent();
		agents[i].transform = Instantiate (prefab).transform;
		// Prevent agents from being placed on top of an existing agent
		agents[i].position = Int2.zero;
		while (Tile.GetCollider (agents[i].position)) {
			agents[i].position = Int2(Random.Range (1, size.x-1), Random.Range (1, size.y-1));
		}
		Tile.SetCollider (agents[i].position, true);
		agents[i].transform.position = agents[i].position.ToVector2();
		agents[i].direction = directions[Random.Range (0, 4)];
	}
}

function MoveAgents () : IEnumerator {
	while (true) {		
		for (var i = 0; i < numberOfAgents; i++) {
			// If the agent wasn't moving (dir = Int2.zero) last time, try a new random direction; otherwise use the current direction
			var currentDir = (agents[i].direction == Int2.zero)? directions[Random.Range (0, 4)] : agents[i].direction;
			// If not surrounded, pick a new random direction if there's a collider in the way (otherwise the current dir will remain unchanged)
			if (!CompletelySurrounded (agents[i].position)) {
				while (Tile.GetCollider (agents[i].position + currentDir)) {
					currentDir = directions[Random.Range (0, 4)];
				}
				agents[i].direction = currentDir;
			}
			// Completely surrounded, so don't move
			else {
				agents[i].direction = Int2.zero;
			}
			// Set collider for next position, so other agents won't try to move into this agent
			Tile.SetCollider (agents[i].position + agents[i].direction, true);
		}
		
		// Move from one tile to the next
		var t = 0.0;
		while (t < 1.0) {
			t += Time.deltaTime * moveSpeed;
			for (i = 0; i < numberOfAgents; i++) {
				agents[i].transform.position = Vector2.Lerp (agents[i].position.ToVector2(), (agents[i].position + agents[i].direction).ToVector2(), t);
			}
			yield;
		}
		
		// Turn off collider for current position (if moving) and move agent position to the next tile
		for (i = 0; i < numberOfAgents; i++) {
			if (agents[i].direction != Int2.zero) {
				Tile.SetCollider (agents[i].position, false);
			}
			agents[i].position += agents[i].direction;
		}
	}
}

function CompletelySurrounded (pos : Int2) : boolean {
	for (var i = 0; i < 4; i++) {
		if (!Tile.GetCollider (pos + directions[i])) {
			return false;
		}
	}
	return true;
}

function OnGUI () {
	// Prevent non-numerical characters from being input
	var chr = Event.current.character;
	if (chr < "0"[0] || chr > "9"[0]) {
		Event.current.character = "\0"[0];
	}
	
	BeginHorizontal();
	Label ("Number of agents", Width(105));
	newNumber = TextField (newNumber, Width(35));
	if (Button ("Set", Width(30))) {
		for (var i = 0; i < numberOfAgents; i++) {
			Tile.SetCollider (agents[i].position, false);
			Tile.SetCollider (agents[i].position + agents[i].direction, false);
			Destroy (agents[i].transform.gameObject);
		}
		numberOfAgents = Mathf.Clamp (parseInt (newNumber), 1, 100);
		newNumber = numberOfAgents.ToString();
		StopAllCoroutines();
		SetupAgents();
		MoveAgents();
	}
	EndHorizontal();

	BeginHorizontal();
	Label ("Move speed", Width(105));
	newSpeed = TextField (newSpeed, Width(35));
	if (Button ("Set", Width(30))) {
		moveSpeed = Mathf.Clamp (parseInt (newSpeed), 1, 100);
		newSpeed = moveSpeed.ToString();
	}
	EndHorizontal();
}