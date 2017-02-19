using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using LitJson;

public class ItemDatabase : MonoBehaviour {

    //private List<Item> database = new List<Item>(); // our database is just a list collection of items
    //private JsonData itemData; // the data is held in a Json file which we will map to and from

	//void Start ()
 //   {
 //       // Start by loading initial item data from the json file
 //       // NOTE: file is held in StreamingAssets folder so it does not get included in build 
 //       itemData = JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/StreamingAssets/Items.json"));
 //       ConstructItemDatabase();
 //       Debug.Log(FetchItemByID(2).Description);
		
	//}

    //void ConstructItemDatabase ()
    //{
    //    // Loop thru json item data and create a database Item element for each one
    //    foreach (JsonData item in itemData )
    //    {
    //        Debug.Log(item["title"]);
    //        database.Add(new Item(
    //            (int)item["id"], 
    //            item["title"].ToString(), 
    //            (int)item["value"],
    //            item["description"].ToString(),
    //            (bool)item["stackable"],
    //            (int)item["rarity"],
    //            item["slug"].ToString()
    //        ));
    //    }

    //}

    //public Item FetchItemByID(int id)
    //{
    //    // Loop thru database items and return the first one with matching id. 
    //    // If no item matches, return null
    //    foreach (Item item in database)
    //    {
    //        if (item.ID == id)
    //        {
    //            return item;
    //        }
    //    }
    //    return null;
    //}
	
//}

//public class Item {

//    public int ID { get; set; }
//    public string Title { get; set; }
//    public int Value { get; set; }
//    public string Description { get; set; }
//    public bool Stackable { get; set; }
//    public int Rarity { get; set; }
//    public string Slug { get; set; }
//    public Sprite Sprite { get; set; }

//    // Standard Inventory Item constructor
//    public Item (int id, string title, int value, string description, bool stackable, int rarity, string slug)
//    {
//        this.ID = id;
//        this.Title = title;
//        this.Value = value;
//        this.Description = description;
//        this.Stackable = stackable;
//        this.Rarity = rarity;
//        this.Slug = slug;
//        this.Sprite = Resources.Load<Sprite>("Sprites/Items/" + slug);
//    }

//    // Empty constructor, used for creating dummy Item objects
//    public Item ()
//    {
//        this.ID = -1;
//    }

}
