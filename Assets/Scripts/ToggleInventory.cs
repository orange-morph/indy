using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleInventory : MonoBehaviour {

    public GameObject inventoryMenu; // Assign in inspector
    private bool isShowing;

    // Use this for initialization
    void Start () {
        inventoryMenu.SetActive(false);
        isShowing = false;
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.I))
        {
            isShowing = !isShowing;
            inventoryMenu.SetActive(isShowing);
        }

    }
}
