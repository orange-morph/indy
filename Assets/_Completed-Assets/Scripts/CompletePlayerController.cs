using UnityEngine;
using System.Collections;

//Adding this allows us to access members of the UI namespace including Text.
using UnityEngine.UI;

public class CompletePlayerController : MonoBehaviour {

	public float speed;				//Floating point variable to store the player's movement speed.
	public Text countText;			//Store a reference to the UI Text component which will display the number of pickups collected.
	public Text winText;			//Store a reference to the UI Text component which will display the 'You win' message.
    public AudioSource footsteps;  //Store a reference to the AudioSource attached to the player for footsteps

    private Rigidbody2D rb2d;		//Store a reference to the Rigidbody2D component required to use 2D Physics.
	private int count;				//Integer to store the number of pickups collected so far.
    private bool moving;            //Whether or not the player is currently moving
    private bool startedMoving;     //Whether or not the player just started moving
    private bool stoppedMoving;     //Whether or not the player just stopped moving
    private bool movingLeft;        //Whether or not the player is currently moving left
    private bool movingRight;       //Whether or not the player is currently moving right
    private bool movingUp;          //Whether or not the player is currently moving up
    private bool movingDown;        //Whether or not the player is currently moving down


    // initialization
    void Start()
	{
		//store reference to the Rigidbody2D component so that we can access it.
		rb2d = GetComponent<Rigidbody2D> ();

        //Initialize item pickup count to zero.
        count = 0;

		//Initialze winText to a blank string at beginning.
		winText.text = "";

		//Call our SetCountText function which will update the text with the current value for count.
		SetCountText ();
	}

	//FixedUpdate called at a fixed interval and is independent of frame rate. Put physics code here.
	void FixedUpdate()
	{
		//Store the current horizontal and vertical inputs
		float moveHorizontal = Input.GetAxisRaw ("Horizontal");
        float moveVertical = Input.GetAxisRaw ("Vertical");

        // Check and set movement indicator variables, and call player movement functions
        checkAndHandleMovement(moveHorizontal, moveVertical);

        //Use the two store floats to create a new Vector2 variable movement.
        Vector2 movement = new Vector2 (moveHorizontal, moveVertical);

		//Call the AddForce function of our Rigidbody2D rb2d supplying movement multiplied by speed to move our player.
		rb2d.AddForce (movement * speed);
    }

    // Check and set movement indicators based on horizontal and vertical axis inputs
    private void checkAndHandleMovement(float moveHorizontal, float moveVertical)
    {
        // Reset all indicators prior to checking (except for previous overall player moving indicator)
        movingLeft = false;
        movingRight = false;
        movingUp = false;
        movingDown = false;
        startedMoving = false;
        stoppedMoving = false;

        if ((moveHorizontal == 0) && (moveVertical == 0)) // There is no movement
        {
            if (moving == true) // just stopped moving
            {
                stoppedMoving = true;
                playerStoppedMoving();
            }
            moving = false;   
        }
        else // There is movement. TODO: set variables for diagonal movement ready for animation handling
        {
            if (moving == false) // just started moving
            {
                startedMoving = true;
                playerStartedMoving();
            }
            moving = true;
            if (moveHorizontal < 0) // moving left
            {
                print("Moving left");
                movingLeft = true;
            }
            if (moveHorizontal > 0) // moving right
            {
                print("Moving right");
                movingRight = true;
            }
            if (moveVertical > 0) // moving up
            {
                movingUp = true;
                print("Moving up");
            }
            if (moveVertical < 0) // moving down
            {
                movingDown = true;
                print("Moving down");
            }
        }
    }

    // This method will be called whenever player starts moving
    private void playerStartedMoving()
    {
        // We will start playing the footsteps IF they are not already playing
        footsteps.Play();  
    }

    // This method will be called whenever player is moving
    private void playerIsMoving()
    {
        // Any thing that needs to happen all the time while player is moving e.g. animation
    }

    // This method will be called whenever player stops moving
    private void playerStoppedMoving()
    {
        footsteps.Stop();
    }

    private bool anyMovementKeyDepressed()
    {
        return (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D));
    }

    //OnTriggerEnter2D is called whenever this object overlaps with a trigger collider.
    void OnTriggerEnter2D(Collider2D other) 
	{
		//Check the provided Collider2D parameter other to see if it is tagged "PickUp", if it is...
		if (other.gameObject.CompareTag ("PickUp")) 
		{
			//... then set the other object we just collided with to inactive.
			other.gameObject.SetActive(false);
			
			//Add one to the current value of our count variable.
			count = count + 1;
			
			//Update the currently displayed count by calling the SetCountText function.
			SetCountText ();
		}
		
	}

	//This function updates the text displaying the number of objects we've collected and displays our victory message if we've collected all of them.
	void SetCountText()
	{
		//Set the text property of our our countText object to "Count: " followed by the number stored in our count variable.
		countText.text = "Count: " + count.ToString ();

		//Check if we've collected all 12 pickups. If we have...
		if (count >= 12)
			//... then set the text property of our winText object to "You win!"
			winText.text = "You win!";
	}
}
