using Devdog.General;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantBehaviour : MonoBehaviour {

    [Header("The plant GameObject this script is attached to")]
    public GameObject plant; // the parent plant gameobject this script gets attached to

    [Header("Sprites for all growth stages")]
    public Sprite seedSprite; // seed image for the plant
    public Sprite sproutSprite; // sprout image for the plant
    public Sprite juvenileSprite; // juvenile (sapling/young) image for the plant
    public Sprite adultSprite; // adult image for the plant at full growth

    [Header("Initial plant state of growth")]
    public bool seed; // does it start as a seed?
    public bool sprout; // does it start as a sprout?
    public bool juvenile; // does it start as a juvenile?
    public bool adult; // does it start as an adult?

    [Header("Plant Behaviours")]
    public bool seeding; // Should this plant randombly respawn / seed itself?
    public bool fruiting; // Does this plant produce fruit at regular intervals?
    public bool choppable; // Can this plant be chopped by the player to produce resources?
    public bool harvestable; // Is this plant harvestable by the player?

    [Header("Sound Effects")]
    public AudioClip chopSound; // wood chopping sound effect for this plant
    public AudioClip harvestSound; // plant harvesting sound effect for this plant when mature
    public AudioClip seedPickup; // sound to be made if seed is picked up
    public AudioClip sproutSound; // sound to be made if a sprout is harvested for this plant
    public AudioClip juvenileSound; // sound to be made if a juvenile is harvested for this plant

    public BoxCollider2D plantCollider; // the collider object of the plant, for handling triggers
    public GameObject playerObject; // the player object needed to determine facing direction for animations
    public CompletePlayerController playerScript;

    private bool hovering;
    private bool inRange;

    protected void Start()
    {
        hovering = false;
        inRange = false;

        // Initialise starting sprites, and adjust box collider
        if (seed)
        {
            plant.GetComponent<SpriteRenderer>().sprite = seedSprite;
            plantCollider.size = new Vector2(0.05f, 0.18f);
            plantCollider.offset = new Vector2(0, 0);

        } else if (sprout)
        {
            plant.GetComponent<SpriteRenderer>().sprite = sproutSprite;
            plantCollider.size = new Vector2(0.09f, 0.15f);
            plantCollider.offset = new Vector2(0, -0.1f);
        } else if (juvenile)
        {
            plant.GetComponent<SpriteRenderer>().sprite = juvenileSprite;
            plantCollider.size = new Vector2(0.12f, 0);
            plantCollider.offset = new Vector2(-0.01f, -0.37f);
        } else
        {
            plant.GetComponent<SpriteRenderer>().sprite = adultSprite;
            plantCollider.size = new Vector2(0.12f, 0.4f);
            plantCollider.offset = new Vector2(-0.01f, -0.37f);
        }

        // if seeding -> initialise seeding behaviour
        // if fruiting -> initialise fruiting behaviour
    }

    void OnMouseOver()
    {
        hovering = true;
    }

    private void OnMouseExit()
    {
        hovering = false;
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        inRange = true;
    }

    void OnCollisionExit2D(Collision2D coll)
    {
        inRange = false;   
    }

    protected void Update()
    {
        if (hovering && inRange && Input.GetKeyDown(KeyCode.E) )
        {
            // on interact -> interact behaviour (choppable or harvestable or fruiting) or whatever
            Debug.LogWarning("Hello, we are in range of a palm tree, moused over and have also pressed the E key!");
            Interact();
        }

        // if seeding -> seed spawning update behaviour
        // if fruiting -> fruit update behaviour
    }

    protected void Interact()
    {
        AudioSource audio = gameObject.AddComponent<AudioSource>();
        if (seed)
        {
            audio.PlayOneShot(seedPickup);
            switch (playerScript.direction)
            {
                case "left":
                    Debug.LogWarning("..facing left so animate harvest left..");
                    playerScript.changeState(6);
                    break;
                case "right":
                    Debug.LogWarning("..facing right so animate harvest right..");
                    playerScript.changeState(7);
                    break;
                case "down":
                    Debug.LogWarning("..facing down so animate harvest down..");
                    playerScript.changeState(6);
                    break;
                default:
                    Debug.LogWarning("..facing up so animate harvest up (default)..");
                    playerScript.changeState(6);
                    break;
            }
        }
        else if (sprout)
        {
            audio.PlayOneShot(sproutSound);
            // animate pickup
            switch (playerScript.direction)
            {
                case "left":
                    Debug.LogWarning("..facing left so animate harvest left..");
                    playerScript.changeState(6);
                    break;
                case "right":
                    Debug.LogWarning("..facing right so animate harvest right..");
                    playerScript.changeState(7);
                    break;
                case "down":
                    Debug.LogWarning("..facing down so animate harvest down..");
                    playerScript.changeState(6);
                    break;
                default:
                    Debug.LogWarning("..facing up so animate harvest up (default)..");
                    playerScript.changeState(6);
                    break;
            }
        }
        else if (juvenile)
        {
            // animate short chop or harvest
            audio.PlayOneShot(juvenileSound);
            switch (playerScript.direction)
            {
                case "left":
                    Debug.LogWarning("..facing left so animate harvest left..");
                    playerScript.changeState(6);
                    break;
                case "right":
                    Debug.LogWarning("..facing right so animate harvest right..");
                    playerScript.changeState(7);
                    break;
                case "down":
                    Debug.LogWarning("..facing down so animate harvest down..");
                    playerScript.changeState(6);
                    break;
                default:
                    Debug.LogWarning("..facing up so animate harvest up (default)..");
                    playerScript.changeState(6);
                    break;
            }
            
        }
        else
        {
            audio.PlayOneShot(chopSound);
            // animate full chop
            switch (playerScript.direction)
            {
                case "left":
                    Debug.LogWarning("..facing left so animate harvest left..");
                    playerScript.changeState(6);
                    while (audio.isPlaying)
                    {
                        Debug.LogWarning("the chop clip is playing");
                    }
                    //if (playerScript.)
                    playerScript.changeState(0);
                    break;
                case "right":
                    Debug.LogWarning("..facing right so animate harvest right..");
                    playerScript.changeState(7);
                    break;
                case "down":
                    Debug.LogWarning("..facing down so animate harvest down..");
                    playerScript.changeState(6);
                    break;
                default:
                    Debug.LogWarning("..facing up so animate harvest up (default)..");
                    playerScript.changeState(6);
                    break;
            }
            
        }
    }

}

