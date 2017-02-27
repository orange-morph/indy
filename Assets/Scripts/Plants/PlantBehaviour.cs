using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantBehaviour : MonoBehaviour {

    public Sprite sprite; // All plants should have an associated image
    private bool seeding; // Should this plant randombly respawn / seed itself?
    private bool choppable; // Can this plant be chopped by the player to produce resources?
    private bool harvestable; // Is this plant harvestable by the player?
    private bool fruiting; // Does this plant produce fruit at regular intervals?

    protected void Start()
    {
        // on collider enter -> collider behaviour
        // on interact -> interact behaviour (choppable or harvestable or fruiting)
        // if seeding -> initialise seeding behaviour
        // if fruiting -> initialise fruiting behaviour
    }

    protected void Update()
    {
        // if seeding -> seed update behaviour
        // if fruiting -> fruit update behaviour
    }


}

