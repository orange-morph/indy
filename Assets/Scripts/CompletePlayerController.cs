﻿using UnityEngine;
using System.Collections;

//Adding this allows us to access members of the UI namespace including Text.
using UnityEngine.UI;

public class CompletePlayerController : MonoBehaviour {

    [Header("Walking Speed")]
    public float walkSpeed = 1; // player left right walk speed
    public float speed;             //Floating point variable to store the player's movement speed.

    public Text countText;			//Store a reference to the UI Text component which will display the number of pickups collected.
	public Text winText;			//Store a reference to the UI Text component which will display the 'You win' message.

    [Header("Audio")]
    public AudioSource footsteps;  //Store a reference to the AudioSource attached to the player for footsteps

    private Rigidbody2D rb2d;		//Store a reference to the Rigidbody2D component required to use 2D Physics.
	private int count;				//Integer to store the number of pickups collected so far.
    public bool moving;            //Whether or not the player is currently moving
    private bool startedMoving;     //Whether or not the player just started moving
    private bool stoppedMoving;     //Whether or not the player just stopped moving
    public bool movingLeft;         //Whether or not the player is currently moving left
    public bool movingRight;        //Whether or not the player is currently moving right
    public bool movingUp;           //Whether or not the player is currently moving up
    public bool movingDown;         //Whether or not the player is currently moving down
    public string direction; // last direction the character was facing in. defaults to 'down' (forward)

    Animator animator;

    //some flags to check when certain animations are playing
    //bool _isPlaying_walk_left = false;
    //bool _isPlaying_walk_right = false;
    //bool _isPlaying_walk_up = false;
    //bool _isPlaying_walk_down = false;
   
    //animation states - the values in the animator conditions
    public const int STATE_IDLE = 0;
    public const int STATE_WALK_LEFT = 1;
    public const int STATE_WALK_RIGHT = 2;
    public const int STATE_WALK_UP = 3;
    public const int STATE_WALK_DOWN = 4;
    public const int STATE_STAND_UP = 5;
    public const int STATE_CHOP_LEFT = 6;
    public const int STATE_CHOP_RIGHT = 7;
    public const int STATE_CHOP_UP = 8;
    public const int STATE_CHOP_DOWN = 9;
    public const int STATE_PICKAXE_LEFT = 10;
    public const int STATE_PICKAXE_RIGHT = 11;
    public const int STATE_PICKAXE_UP = 12;
    public const int STATE_PICKAXE_DOWN = 13;

    //string _currentDirection = "left";
    int _currentAnimationState = STATE_STAND_UP;

    // initialization
    void Start()
	{
        //define the animator attached to the player
        animator = this.GetComponent<Animator>();
        animator.enabled = true;

        //store reference to the Rigidbody2D component so that we can access it.
        rb2d = GetComponent<Rigidbody2D> ();

        //Initialize item pickup count to zero.
        count = 0;

		//Initialze winText to a blank string at beginning.
		winText.text = "";

        direction = "down";

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
                movingLeft = true;
                direction = "left";
                changeState(STATE_WALK_LEFT);
            }
            if (moveHorizontal > 0) // moving right
            {
                movingRight = true;
                direction = "right";
                changeState(STATE_WALK_RIGHT);
            }
            if (moveVertical > 0) // moving up
            {
                movingUp = true;
                if (! (movingLeft || movingRight)) // don't animate down if also moving left/right
                {
                    direction = "up";
                    changeState(STATE_WALK_UP);
                }     
            }
            if (moveVertical < 0) // moving down
            {
                movingDown = true;
                if (!(movingLeft || movingRight)) // don't animate down if also moving left/right
                {
                    direction = "down";
                    changeState(STATE_WALK_DOWN);
                }
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
        changeState(STATE_IDLE);
    }

    //--------------------------------------
    // Change the players animation state
    //--------------------------------------
    public void changeState(int state)
    {

        if (_currentAnimationState == state) // do nothing if state isn't changing
            return;

        switch (state)
        {
            case STATE_WALK_LEFT:
                animator.SetInteger("state", STATE_WALK_LEFT);
                break;

            case STATE_WALK_RIGHT:
                animator.SetInteger("state", STATE_WALK_RIGHT);
                break;

            case STATE_WALK_UP:
                animator.SetInteger("state", STATE_WALK_UP);
                break;

            case STATE_WALK_DOWN:
                animator.SetInteger("state", STATE_WALK_DOWN);
                break;

            case STATE_STAND_UP:
                animator.SetInteger("state", STATE_STAND_UP);
                break;

            case STATE_IDLE:
                animator.SetInteger("state", STATE_IDLE);
                break;

            case STATE_CHOP_LEFT:
                animator.SetInteger("state", STATE_CHOP_LEFT);
                break;
            case STATE_CHOP_RIGHT:
                animator.SetInteger("state", STATE_CHOP_RIGHT);
                break;
            case STATE_CHOP_UP:
                animator.SetInteger("state", STATE_CHOP_UP);
                break;
            case STATE_CHOP_DOWN:
                animator.SetInteger("state", STATE_CHOP_DOWN);
                break;
            case STATE_PICKAXE_LEFT:
                animator.SetInteger("state", STATE_PICKAXE_LEFT);
                break;
            case STATE_PICKAXE_RIGHT:
                animator.SetInteger("state", STATE_PICKAXE_RIGHT);
                break;
            case STATE_PICKAXE_UP:
                animator.SetInteger("state", STATE_PICKAXE_UP);
                break;
            case STATE_PICKAXE_DOWN:
                animator.SetInteger("state", STATE_PICKAXE_DOWN);
                break;
        }

        _currentAnimationState = state;
    }

    public void chop(float length)
    {
        switch (direction)
        {
            case "left":
                changeState(STATE_CHOP_LEFT);
                Invoke("SetPlayerIdle", length);
                break;
            case "right":
                changeState(STATE_CHOP_RIGHT);
                Invoke("SetPlayerIdle", length);
                break;
            case "up":
                changeState(STATE_CHOP_UP);
                Invoke("SetPlayerIdle", length);
                break;
            case "down":
                changeState(STATE_CHOP_DOWN);
                Invoke("SetPlayerIdle", length);
                break;
            default:
                changeState(STATE_CHOP_LEFT);
                Invoke("SetPlayerIdle", length);
                break;
        }
    }

    public void pickaxe(float length)
    {
        switch (direction)
        {
            case "left":
                changeState(STATE_PICKAXE_LEFT);
                Invoke("SetPlayerIdle", length);
                break;
            case "right":
                changeState(STATE_PICKAXE_RIGHT);
                Invoke("SetPlayerIdle", length);
                break;
            case "up":
                changeState(STATE_PICKAXE_UP);
                Invoke("SetPlayerIdle", length);
                break;
            case "down":
                changeState(STATE_PICKAXE_DOWN);
                Invoke("SetPlayerIdle", length);
                break;
            default:
                changeState(STATE_PICKAXE_LEFT);
                Invoke("SetPlayerIdle", length);
                break;
        }
    }

    protected void SetPlayerIdle()
    {
        if (!moving)
        {
            changeState(STATE_IDLE);
        }
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
			
		}
		
	}

}
