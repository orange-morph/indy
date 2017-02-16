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
        public QuestTrigger healThyselfQuestTrigger;

        protected void Start()
        {
            suitcaseLoot = suitcase.GetComponent<LootableObject>();
            // Listen for the OnEmpty event on the instance of the InventoryManager loot object
            suitcaseLoot.OnEmpty += () =>
            {
                StartCoroutine("GotAllItems"); // trigger the quest update and following convo and next quest
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