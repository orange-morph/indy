using UnityEngine;
using System.Collections;
using PixelCrushers.DialogueSystem;

namespace PixelCrushers.DialogueSystem.SInventory
{

    /// <summary>
    /// Persistent data component for S-Inventory items that get picked up when you click them with the mouse.
    /// </summary>
    [AddComponentMenu("Dialogue System/Third Party/S-Inventory/S-Inventory Persistent Item Pickup")]
    public class SInventoryPersistentItemPickup : MonoBehaviour
    {

        [Tooltip("Save the picked-up status under this name. Uses the GameObject name if blank. The Save Name must be unique.")]
        public string saveName = string.Empty;

        public bool debug = false;

        private global::Item item;
        private InventoryManager invManager;
        private Vector3 recentPosition;
        private bool pickedUp = false;
        private string m_internalName = string.Empty;

        public string internalName
        {
            get
            {
                if (string.IsNullOrEmpty(m_internalName)) m_internalName = !string.IsNullOrEmpty(saveName) ? saveName : OverrideActorName.GetActorName(transform);
                return m_internalName;
            }
            set
            {
                m_internalName = value;
            }
        }

        public string variableName { get { return "ItemPickup_" + internalName; } }


        private const float FrequencyToCheckPosition = 5;

        protected virtual void OnEnable()
        {
            PersistentDataManager.RegisterPersistentData(gameObject);
        }

        protected virtual void OnDisable()
        {
            PersistentDataManager.UnregisterPersistentData(gameObject);
        }

        private IEnumerator Start()
        {
            invManager = FindObjectOfType<InventoryManager>();
            item = GetComponent<global::Item>();
            if (string.IsNullOrEmpty(saveName)) saveName = OverrideActorName.GetInternalName(transform);
            while (!pickedUp)
            {
                recentPosition = transform.position;
                yield return new WaitForSeconds(FrequencyToCheckPosition);
            }
            PersistentDataManager.includeAllItemData = true;
        }

        /// <summary>
        /// Records in Lua that the item has been picked up.
        /// </summary>
        public void RecordPickedUp()
        {
            pickedUp = true;
            if (item == null) return;
            DialogueLua.SetVariable(variableName, true);
            if (debug) Debug.Log("Dialogue System: Saving item pickup state to save name '" + variableName + "': (picked up)", this);
        }

        /// <summary>
        /// Destroys the game object if it's been picked up.
        /// </summary>
        public void OnApplyPersistentData()
        {
            if (item == null) return;
            if (!DialogueLua.DoesVariableExist(variableName)) return;
            pickedUp = DialogueLua.GetVariable(variableName).AsBool;
            if (pickedUp)
            {
                Destroy(item.gameObject);
                if (debug) Debug.Log("Dialogue System: Restoring item pickup state from save name '" + variableName + "': (destroy object; already picked up)", this);
            }
            else
            {
                if (debug) Debug.Log("Dialogue System: Restoring item pickup state from save name '" + variableName + "': (leave object alone; not picked up)", this);
            }
        }

        private void OnMouseDown()
        {
            if (pickedUp || (invManager == null) || (item == null) || (invManager.PickupType != InventoryManager.PickupTypes.Mouse)) return;
            var distance = Vector3.Distance(recentPosition, invManager.Player.transform.position);
            if (distance <= item.MinDistance)
            {
                RecordPickedUp();
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (pickedUp || (invManager == null) || (other != invManager.Player.GetComponent<Collider>())) return;
            pickedUp = (InventoryUI.PickingUP == true) || (invManager.OnCollisionPickup == true) ||
                ((invManager.PickupType == InventoryManager.PickupTypes.Keyboard) && Input.GetKey(invManager.ActionKey));
            if (pickedUp)
            {
                RecordPickedUp();
            }
        }
    }

}
