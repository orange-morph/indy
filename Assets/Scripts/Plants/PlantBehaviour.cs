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


    [Header("Plant Animations")]
    public Animation choppingAnimation; // the animation to be used when this plant is chopped by the player
    public Animation harvestingAnimation; // the animation to be used when this plant is harvested by the player
    public Animation pickupAnimation; // the animation to be used when a seed / sprout is picked up by the player

    public BoxCollider2D plantCollider; // the collider object of the plant, for handling triggers

    private bool hovering;
    private bool inRange;
    private bool interacting;

    protected void Start()
    {
        hovering = false;
        inRange = false;
        interacting = false;

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

    protected void FixedUpdate()
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
            // animate pickup
            audio.PlayOneShot(seedPickup);
        }
        else if (sprout)
        {
            // animate pickup
            audio.PlayOneShot(sproutSound);
        }
        else if (juvenile)
        {
            // animate short chop or harvest
            audio.PlayOneShot(juvenileSound);
        }
        else
        {
            if (choppable)
            {
                // animate full chop
                audio.PlayOneShot(chopSound);
            } else if (harvestable)
            {
                // animate full harvest
                audio.PlayOneShot(harvestSound);
            }
            
        }
    }

}

