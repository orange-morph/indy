using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SpriteTile;

public class CompleteCameraController : MonoBehaviour {

	public GameObject player;		//Public variable to store a reference to the player game object
    public TextAsset level;

    private Vector3 offset;			//Private variable to store the offset distance between the player and camera

	// Use this for initialization
	void Start () 
	{
        //Calculate and store the offset value by getting the distance between the player's position and camera's position.
        offset = transform.position - player.transform.position;

	}
	
	// LateUpdate is called after Update each frame
	void LateUpdate () 
	{
		// Set the position of the camera's transform to be the same as the player's, but offset by the calculated offset distance.
		transform.position = player.transform.position + offset;
        var pos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        // Control the bounds of the camera tracking on the player by x and y
        if (pos.x < 20) {
            pos.x = Mathf.Clamp(pos.x, 20, 20);
        }
        if (pos.y < 10)
        {
            pos.y = Mathf.Clamp(pos.y, 10, 10);
        }
        if (pos.x > 80)
        {
            pos.x = Mathf.Clamp(pos.x, 80, 80);
        }
        if (pos.y > 90)
        {
            pos.y = Mathf.Clamp(pos.y, 90, 90);
        }
        transform.position = pos;
    }
}
