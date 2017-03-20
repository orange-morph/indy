using Devdog.General;
using Devdog.InventoryPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockBehaviour : MonoBehaviour
{

    [Header("The rock GameObject this script is attached to")]
    public GameObject boulder; // the parent boulder gameobject this script gets attached to

    [Header("InventoryItem for baby rocks")]
    public UnusableInventoryItem rock; // the child rock items that spawn from the boulder
    
    [Header("Sound Effects")]
    public AudioClip pickaxeSound; // pickaxe mining sound effect for this rock
    public AudioClip rockSplit; // sound of boulder breaking into rocks

    public BoxCollider2D boulderCollider; // the collider object of the rock, for handling triggers
    private GameObject playerObject; // the player object needed to determine facing direction for animations etc
    private CompletePlayerController playerScript;

    private bool hovering;
    private bool inRange;
    private Vector3 boulderOriginalPosition;
    private Transform boulderTransform;
    private IEnumerator coroutine;

    protected void Start()
    {
        playerObject = GameObject.Find("Player");
        playerScript = playerObject.GetComponent<CompletePlayerController>();
        hovering = false;
        inRange = false;
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
        if (hovering && inRange && Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }

    protected void Interact()
    {
         AnimateWithClip(pickaxeSound);  
    }

    protected void AnimateWithClip(AudioClip clip)
    {
        AudioSource audio = gameObject.AddComponent<AudioSource>();
        audio.PlayOneShot(clip);
        coroutine = Shake(clip.length);
        StartCoroutine(coroutine);
        playerScript.pickaxe(clip.length);
    }

    IEnumerator Shake(float duration)
    {
        // Animated 'shake' effect via TransformPositions over time..
        yield return new WaitForSeconds(0.22f); // initial offset before applying shake
        duration -= 0.22f;
        for (int i = 0; i < (duration-1.0f); i++)
        {
            StartCoroutine("ShakeOnce");
            if (i >= (int)(duration-1.0f)) // spawn rocks on last second
            {
                SpawnRocks();
            }
            yield return new WaitForSeconds(1.37f);
        }
        yield return true;
    }


    IEnumerator ShakeOnce()
    {
        // use rock game object and get it's Transform vector (x,y,z) and store it - this is needed to restore to original at end of effect
        boulderTransform = boulder.GetComponent<Transform>();
        boulderOriginalPosition = transform.localPosition;
        float shakeFrames = 20; // number of frames to use for this animation
        float shakeAmount = 0.10f; // the extremes of movement away from the original position
        float slowDownRate = 0.002f; // the rate at which the shake movement lessens in severity over frames

        for (int i = 0; i < shakeFrames; i++)
        {
            Vector3 random = Random.insideUnitSphere * shakeAmount;
            boulderTransform.localPosition = new Vector3(boulderOriginalPosition.x + random.x, boulderOriginalPosition.y + random.y, 0);
            shakeAmount -= slowDownRate;
            yield return new WaitForFixedUpdate();
        }
        boulderTransform.localPosition = boulderOriginalPosition;
        yield return true;
    }

    public void SpawnRocks()
    {
        // spawn several rocks in random positions around the boulder location
        boulderTransform = boulder.GetComponent<Transform>();
        boulderOriginalPosition = transform.localPosition;
        AudioSource audio = gameObject.AddComponent<AudioSource>();
        audio.PlayOneShot(rockSplit);
        for (int i = 0; i < 5; i++)
        {
            UnusableInventoryItem newRock = Instantiate(rock);
            Vector3 random = Random.insideUnitSphere * 2.0f; // getting a randomised vector 3 within a range limit
            newRock.GetComponent<Transform>().position = new Vector3(boulderOriginalPosition.x + random.x, boulderOriginalPosition.y + random.y, 0);
        }
        boulder.GetComponent<SpriteRenderer>().sprite = null;
        Destroy(boulder, rockSplit.length);
    }
}

