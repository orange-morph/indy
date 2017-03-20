using Devdog.General;
using Devdog.InventoryPro;
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
    public Sprite stumpSprite; // replacing tree with stump once harvested

    [Header("Initial plant state of growth")]
    public bool seed; // does it start as a seed?
    public bool sprout; // does it start as a sprout?
    public bool juvenile; // does it start as a juvenile?
    public bool adult; // does it start as an adult?
    public bool newStump; // whether it is a stump
    public bool oldStump;

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
    public AudioClip logSplit; 

    public BoxCollider2D plantCollider; // the collider object of the plant, for handling triggers
    public GameObject playerObject; // the player object needed to determine facing direction for animations
    public CompletePlayerController playerScript;

    public UnusableInventoryItem log;

    private SpriteRenderer treeSprite;
    private BoxCollider2D treeCollider;

    private bool hovering;
    private bool inRange;
    private Vector3 treeOriginalPosition;
    private Transform treeTransform;
    private IEnumerator coroutine;

    protected void Start()
    {

        playerScript = playerObject.GetComponent<CompletePlayerController>();
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
            Interact();
        }

        // if seeding -> seed spawning update behaviour
        // if fruiting -> fruit update behaviour
    }

    protected void Interact()
    {
        
        if (seed)
        {
            //AnimateWithClip(seedPickup);
            AnimateWithClip(chopSound);
        }
        else if (sprout)
        {
            //AnimateWithClip(sproutSound);
            AnimateWithClip(chopSound);
        }
        else if (juvenile)
        {
            //AnimateWithClip(juvenileSound);
            AnimateWithClip(chopSound);
        }
        else
        {
            AnimateWithClip(chopSound);
        }
    }

    protected void AnimateWithClip(AudioClip clip)
    {
        AudioSource audio = gameObject.AddComponent<AudioSource>();
        audio.PlayOneShot(clip);
        coroutine = Shake(clip.length);
        StartCoroutine(coroutine);
        playerScript.chop(clip.length);
    }

    IEnumerator Shake(float duration)
    {
        // Animated 'shake' effect via TransformPositions over time..
        yield return new WaitForSeconds(0.7f); // initial offset before applying shake
        duration -= 0.7f;
        for (int i = 0; i < duration; i++)
        {
            StartCoroutine("ShakeOnce");
            if (i == (int)(duration)) // spawn logs on last second
            {
                SpawnLogs();
            }
            yield return new WaitForSeconds(1);
        }
        yield return true;
    }
 

    IEnumerator ShakeOnce()
    {
        // use plant game object and get it's Transform vector (x,y,z) and store it - this is needed to restore to original at end of effect
        treeTransform = plant.GetComponent<Transform>();
        treeOriginalPosition = transform.localPosition;
        float shakeFrames = 20; // number of frames to use for this animation
        float shakeAmount = 0.10f; // the extremes of movement away from the original position
        float slowDownRate = 0.002f; // the rate at which the shake movement lessens in severity over frames
     
        for (int i = 0; i < shakeFrames; i++)
        {
            Vector3 random = Random.insideUnitSphere * shakeAmount;
            treeTransform.localPosition = new Vector3(treeOriginalPosition.x + random.x, treeOriginalPosition.y + random.y, 0);
            shakeAmount -= slowDownRate;
            yield return new WaitForFixedUpdate();
        }
        treeTransform.localPosition = treeOriginalPosition;
        yield return true;
    }

    public void SpawnLogs()
    {
        
        if (adult && !oldStump)
        {
            ReplaceWithStump();
        }

        // spawn several logs in random positions around the tree location
        treeTransform = plant.GetComponent<Transform>();
        treeOriginalPosition = transform.localPosition;
        AudioSource audio = gameObject.AddComponent<AudioSource>();
        audio.PlayOneShot(logSplit);
        for (int i = 0; i < 5; i++)
        {
            UnusableInventoryItem newLog = Instantiate(log);
            Vector3 random = Random.insideUnitSphere * 2.0f; // getting a randomised vector 3 within a range limit
            newLog.GetComponent<Transform>().position = new Vector3(treeOriginalPosition.x + random.x, treeOriginalPosition.y + random.y, 0);
        }
        if (oldStump)
        {
            Destroy(plant, 0.25f);
        }
        else if (newStump)
        {
            oldStump = true;
        }
        
    }

    public void ReplaceWithStump()
    {
        treeTransform = plant.GetComponent<Transform>();
        treeOriginalPosition = transform.localPosition;
        treeTransform.position = new Vector3(treeOriginalPosition.x, treeOriginalPosition.y - 2.0f, 0);

        treeCollider = plant.GetComponent<BoxCollider2D>();
        treeCollider.offset = new Vector2(-0.01f, 0.01f);
        treeCollider.size = new Vector2(0.12f, 0.1f);

        treeSprite = plant.GetComponent<SpriteRenderer>();
        treeSprite.sprite = stumpSprite;
        newStump = true;
    }

}

