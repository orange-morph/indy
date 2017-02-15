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
    //using Devdog.General.ThirdParty.UniLinq;

    [AddComponentMenu(InventoryPro.AddComponentMenuPath + "Triggers/Lootable objet")]
    public class SuitcaseEmptyQuestUpdate : MonoBehaviour
    {
        // reference to the lootable item - the suitcase itself: it's instance of lootui which will throw the OnEmpty
        public GameObject suitcase;
        private LootableObject suitcaseLoot; // obtained as child component of suitcase GameObject parent
        public QuestTrigger gotAllItemsTrigger; // reference to the DialogueSystem quest itself, set via the UI
        public ConversationTrigger healThyselfConvo; // The next convo to be triggered after this quest.

        protected void Start()
        {
            suitcaseLoot = suitcase.GetComponent<LootableObject>();
            // Listen for the OnEmpty event on the instance of the InventoryManager loot object
            suitcaseLoot.OnEmpty += () =>
            {
                gotAllItemsTrigger.Fire(); // fire the quest completion trigger
                healThyselfConvo.Start(); // Kick off the next conversation which in turn triggers the next quest
            };
        }
    }
}