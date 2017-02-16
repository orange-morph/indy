using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Devdog.General;
using Devdog.General.UI;
using Devdog.InventoryPro;
using Devdog.InventoryPro.UI;
using UnityEngine.Serialization;
using PixelCrushers.DialogueSystem;

namespace Devdog.InventoryPro
{
    [AddComponentMenu(InventoryPro.AddComponentMenuPath + "Triggers/Lootable objet")]
    public class SuitcaseEmptyQuestUpdate : MonoBehaviour
    {
        public InventoryPlayer player; // the player in relation to Inventory, collections, etc
        public GameObject suitcase; // reference to lootable item - suitcase: instance of lootui which will throw OnEmpty
        private LootableObject suitcaseLoot; // obtained as child component of suitcase GameObject parent
        public QuestTrigger gotAllItemsTrigger; // reference to the DialogueSystem quest itself, set via the UI
        public ConversationTrigger healThyselfConvo; // The next convo to be triggered after the find items quest.
        public QuestTrigger healThyselfQuestTrigger; // Quest trigger for healing head injury quest
        public QuestTrigger gotFirstAidKitTrigger; // Quest trigger for picking up the first aid kit
        public QuestTrigger usedBandageTrigger; // Quest trigger for having used the bandage to heal head injury
        public GameObject firstAidKit;
        private ItemPouchInventoryItem firstAidKitItem; // first aid kit item
        public GameObject bandage;
        private ConsumableInventoryItem bandageItem;  // bandage item
        public ItemCollectionBase playerInventoryCollection; // player inventory, used for item events (added item, used item)
        private bool gotAllItems;

        protected void Start()
        {
            gotAllItems = false;
            suitcaseLoot = suitcase.GetComponent<LootableObject>();
            firstAidKitItem = firstAidKit.GetComponent<ItemPouchInventoryItem>();
            bandageItem = bandage.GetComponent<ConsumableInventoryItem>();
            
            // Listen for the OnEmpty event on the instance of the InventoryManager loot object
            suitcaseLoot.OnEmpty += () =>
            {
                StartCoroutine("GotAllItems"); // trigger the quest update and following convo and next quest
            };

            // Listen for item added to inventory and check for first aid kit match - trigger quest update
            //playerInventoryCollection.OnAddedItem += (items, amount, cameFromCollection) =>
            //{
            //    foreach (InventoryItemBase item in items)
            //    {
            //        if (item.ID.Equals(firstAidKitItem.ID)) {
            //            Debug.LogWarning("The first aid kit was added to the player inventory");
            //        }
            //    }
            //};

            playerInventoryCollection.OnUsedItem += (item, amount, cameFromCollection, something) =>
            {
                if (item.ID.Equals(firstAidKitItem.ID) && gotAllItems == true )
                {
                    Debug.LogWarning("The first aid kit has been opened by the player in their inventory!");
                    gotFirstAidKitTrigger.Fire(); // fire the quest completion trigger
                }
                if (item.ID.Equals(bandageItem.ID))
                {
                    Debug.LogWarning("The bandage has been used by the player in their inventory!");
                    usedBandageTrigger.Fire(); // fire the quest completion trigger
                }
            };
        }

        IEnumerator GotAllItems()
        {
            gotAllItemsTrigger.Fire(); // fire the quest completion trigger
            yield return new WaitForSeconds(3); // wait 3 seconds for success alert to clear nicely
            StartCoroutine("RunNextConvo"); // kick off next convo
            yield return true;    
        }

        IEnumerator RunNextConvo()
        {
            gotAllItems = true;
            DialogueManager.StartConversation("beach2 heal thyself"); // Kick off the next conversation
            StartCoroutine("RunNextQuest"); // trigger the next quest
            yield return true;
        }

        IEnumerator RunNextQuest()
        {
            yield return new WaitForSeconds(7); // give 7 seconds for the convo to run before imposing the quest tracker
            healThyselfQuestTrigger.Fire(); // fire the quest trigger
            yield return true;
        }

    }

}