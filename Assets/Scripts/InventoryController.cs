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
        AddItem(4);
        AddItem(2);
        AddItem(4);
        AddItem(4);
        AddItem(4);
        AddItem(4);
    }

    // Fetches item by id from database; loops through inventory items until it finds one with dummy item, replaces it.
    public void AddItem(int id)
    {
        Item itemToAdd = database.FetchItemByID(id);

        if (itemToAdd.Stackable && CheckItemInInventory(itemToAdd) ) // if the item is already present in the inventory and is stackable
        {
            // find the inventory item again and increment its stackable amount counter by 1
            for (int i = 0; i < items.Count; i++) // loop through items already held in inventory
            {
                if (items[i].ID == id) // matched in inventory
                {
                    ItemData data = slots[i].transform.GetChild(0).GetComponent<ItemData>();
                    data.amount++; // At the moment, all we do is increment by 1. In future, we will have stack-splitting etc, so this will change.
                    data.transform.GetChild(0).GetComponent<Text>().text = data.amount.ToString();
                    break;
                }
            }
        } 
        else // otherwise, it does not stack, so we'll just add it as normal.
        {
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

    public bool CheckItemInInventory(Item item)
    {
        for (int i = 0; i < items.Count; i++) // loop through items already held in inventory
        {
            if (items[i].ID == item.ID) // matched in inventory
            {
                return true;
            }
        }
        return false;
    }

}



