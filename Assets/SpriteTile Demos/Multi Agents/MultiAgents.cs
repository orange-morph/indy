// Makes a number of moving agents in a level, where none of the agents will move into a position occupied by another
using UnityEngine;
using System.Collections;
using SpriteTile;

class Agent {
	public Transform transform;
	public Int2 position;
	public Int2 direction;
}

public class MultiAgents : MonoBehaviour {

	public TextAsset level;
	public GameObject prefab;
	public int numberOfAgents = 20;
	public float moveSpeed = 2.0f;
	
	private Agent[] agents;
	private string newNumber;
	private string newSpeed;
	
	private Int2[] directions = new Int2[]{Int2.right, Int2.left, Int2.up, Int2.down};
	
	void Start () {
		Tile.SetCamera();
		Tile.LoadLevel (level);
		
		SetupAgents();
		StartCoroutine (MoveAgents());
		
		newNumber = numberOfAgents.ToString();
		newSpeed = moveSpeed.ToString();
	}
	
	void SetupAgents () {
		// Set up all agents with a random position in the level, with a random initial direction
		agents = new Agent[numberOfAgents];
		var size = Tile.GetMapSize();
		for (int i = 0; i < numberOfAgents; i++) {
			agents[i] = new Agent();
			agents[i].transform = Instantiate (prefab).transform;
			// Prevent agents from being placed on top of an existing agent
			agents[i].position = Int2.zero;
			while (Tile.GetCollider (agents[i].position)) {
				agents[i].position = new Int2(Random.Range (1, size.x-1), Random.Range (1, size.y-1));
			}
			Tile.SetCollider (agents[i].position, true);
			agents[i].transform.position = agents[i].position.ToVector2();
			agents[i].direction = directions[Random.Range (0, 4)];
		}
	}
	
	IEnumerator MoveAgents () {
		while (true) {		
			for (int i = 0; i < numberOfAgents; i++) {
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
			float t = 0.0f;
			while (t < 1.0f) {
				t += Time.deltaTime * moveSpeed;
				for (int i = 0; i < numberOfAgents; i++) {
					agents[i].transform.position = Vector2.Lerp (agents[i].position.ToVector2(), (agents[i].position + agents[i].direction).ToVector2(), t);
				}
				yield return null;
			}
			
			// Turn off collider for current position (if moving) and move agent position to the next tile
			for (int i = 0; i < numberOfAgents; i++) {
				if (agents[i].direction != Int2.zero) {
					Tile.SetCollider (agents[i].position, false);
				}
				agents[i].position += agents[i].direction;
			}
		}
	}
	
	bool CompletelySurrounded (Int2 pos) {
		for (int i = 0; i < 4; i++) {
			if (!Tile.GetCollider (pos + directions[i])) {
				return false;
			}
		}
		return true;
	}
	
	void OnGUI () {
		// Prevent non-numerical characters from being input
		var chr = Event.current.character;
		if (chr < '0' || chr > '9') {
			Event.current.character = '\0';
		}
		
		GUILayout.BeginHorizontal();
		GUILayout.Label ("Number of agents", GUILayout.Width(105));
		newNumber = GUILayout.TextField (newNumber, GUILayout.Width(35));
		if (GUILayout.Button ("Set", GUILayout.Width(30))) {
			for (var i = 0; i < numberOfAgents; i++) {
				Tile.SetCollider (agents[i].position, false);
				Tile.SetCollider (agents[i].position + agents[i].direction, false);
				Destroy (agents[i].transform.gameObject);
			}
			numberOfAgents = Mathf.Clamp (System.Int32.Parse (newNumber), 1, 100);
			newNumber = numberOfAgents.ToString();
			StopAllCoroutines();
			SetupAgents();
			StartCoroutine (MoveAgents());
		}
		GUILayout.EndHorizontal();
	
		GUILayout.BeginHorizontal();
		GUILayout.Label ("Move speed", GUILayout.Width(105));
		newSpeed = GUILayout.TextField (newSpeed, GUILayout.Width(35));
		if (GUILayout.Button ("Set", GUILayout.Width(30))) {
			moveSpeed = Mathf.Clamp (System.Int32.Parse (newSpeed), 1, 100);
			newSpeed = moveSpeed.ToString();
		}
		GUILayout.EndHorizontal();
	}
}