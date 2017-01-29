using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour {

    GameObject inventoryPanel; // UI Panel parent holding the inventory UI elements
    GameObject slotPanel; // UI Panel child of inventoryPanel, holds all inventory slot elements
    public GameObject inventorySlot; // Prefab for InventorySlot, linked via Unity interface
    public GameObject inventoryItem; // Prefab for InventoryItem, linked via Unity interface

    ItemDatabase database; // our database object list of items
    int slotAmount; // Preset maximum number of items to be displayed in our inventory panel
    public List<GameObject> slots = new List<GameObject>(); // List of slot objects, to be populated
    public List<Item> items = new List<Item>(); // List of item objects, to be populated

    void Start () {

        database = GetComponent<ItemDatabase>();
        slotAmount = 20;
        inventoryPanel = GameObject.Find("InventoryPanel");
        slotPanel = inventoryPanel.transform.FindChild("SlotsPanel").gameObject;

        for (int i = 0; i < slotAmount; i++) // This sets up our inventory slots ready to receive items
        {
            items.Add(new Item()); // fills out our inventory slots with null items
            slots.Add(Instantiate(inventorySlot)); // adds a blank inventory slot to the slots list
            slots[i].transform.SetParent(slotPanel.transform); // assign the new slot to parent slotpanel object
        }

        AddItem(1);
        AddItem(4);
        AddItem(0);
        AddItem(3);
        AddItem(2);
    }

    // Fetches item by id from database; loops through inventory items until it finds one with dummy item, replaces it.
    public void AddItem(int id)
    {
        Item itemToAdd = database.FetchItemByID(id);
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].ID == -1) // If the inventory slot has a dummy item in it, replace with our real item.
            {
                items[i] = itemToAdd;
                GameObject itemObject = Instantiate(inventoryItem);
                itemObject.transform.SetParent(slots[i].transform);
                itemObject.GetComponent<Image>().sprite = itemToAdd.Sprite;
                //itemObject.transform.position = new Vector2(0,0);
                break;
            }
        }

    }

}



