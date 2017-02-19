using UnityEngine;
using System.Text;
using PixelCrushers.DialogueSystem;

namespace PixelCrushers.DialogueSystem.SInventory
{

    /// <summary>
    /// This class provides a data bridge between S-Inventory and
    /// the Dialogue System. Add it to your actor(s).
    /// </summary>
    [AddComponentMenu("Dialogue System/Third Party/S-Inventory/S-Inventory Dialogue Actor")]
    public class SInventoryDialogueActor : MonoBehaviour
    {

        /// <summary>
        /// Set <c>true</c> to hook the actor's S-Inventory data into the
        /// Dialogue System's Save/Load/PersistentData system.
        /// </summary>
        [Tooltip("Save the actor's inventory with the Dialogue System's Save System.")]
        public bool saveInventory = true;

        /// <summary>
        /// Set <c>true</c> to automatically hide the inventory GUI when this
        /// actor is involved in conversations.
        /// </summary>
        [Tooltip("Hide the inventory GUI when the actor is involved in conversations.")]
        public bool hideInventoryInConversations = true;

        [Tooltip("Log debug info.")]
        public bool debug = false;

        //[HideInInspector]
        //public string actorName;

        [HideInInspector]
        public InventoryManager invManager = null;

        [HideInInspector]
        public Equipment equipment = null;

        [HideInInspector]
        public SkillBar skillBar = null;

        [HideInInspector]
        public InventoryUI invUI = null;

        [HideInInspector]
        public EquipmentUI equipUI = null;

        [HideInInspector]
        public SkillBarUI skillBarUI = null;

        [HideInInspector]
        public CraftUI craftUI = null;

        [HideInInspector]
        public VendorUI vendorUI = null;

        [HideInInspector]
        public ItemGroupUI itemGroupUI = null;

        private bool wasInvUIShowing = false;
        private bool wasEquipUIShowing = false;
        private bool wasSkillBarUIShowing = false;
        private bool wasCraftUIShowing = false;
        private bool wasVendorUIShowing = false;
        private bool wasItemGroupUIShowing = false;

        private string m_internalName;

        public string internalName
        {
            get
            {
                if (string.IsNullOrEmpty(m_internalName)) m_internalName = OverrideActorName.GetActorName(transform);
                return m_internalName;
            }
            set
            {
                m_internalName = value;
            }
        }

        public virtual void Awake()
        {
            invManager = FindObjectOfType<InventoryManager>();
            equipment = FindObjectOfType<Equipment>();
            skillBar = FindObjectOfType<SkillBar>();
            invUI = FindObjectOfType<InventoryUI>();
            equipUI = FindObjectOfType<EquipmentUI>();
            skillBarUI = FindObjectOfType<SkillBarUI>();
            craftUI = FindObjectOfType<CraftUI>();
            vendorUI = FindObjectOfType<VendorUI>();
            itemGroupUI = FindObjectOfType<ItemGroupUI>();
            SInventoryLua.RegisterLuaFunctions();
        }

        protected virtual void OnEnable()
        {
            PersistentDataManager.RegisterPersistentData(gameObject);
        }

        protected virtual void OnDisable()
        {
            PersistentDataManager.UnregisterPersistentData(gameObject);
        }

        public void OnConversationStart(Transform actor)
        {
            if (hideInventoryInConversations) SetInvGUI(false);
        }

        public void OnConversationEnd(Transform actor)
        {
            if (hideInventoryInConversations) SetInvGUI(true);
        }

        /// <summary>
        /// Sets the inventory GUI on or off.
        /// </summary>
        /// <param name="show">Show if <c>true</c>; otherwise hide.</param>
        public void SetInvGUI(bool show)
        {
            if (debug) Debug.Log("Dialogue System: " + name + ".SetInventoryGUI(" + show + ")");
            if (!show)
            {
                wasInvUIShowing = (invUI != null) && invUI.ShowInv;
                wasEquipUIShowing = (equipUI != null) && equipUI.Panel.activeInHierarchy;
                wasSkillBarUIShowing = (skillBarUI != null) && skillBarUI.Panel.activeInHierarchy;
                wasCraftUIShowing = (craftUI != null) && craftUI.Panel.activeInHierarchy;
                wasVendorUIShowing = (vendorUI != null) && vendorUI.Panel.activeInHierarchy;
                wasItemGroupUIShowing = (itemGroupUI != null) && itemGroupUI.Panel.activeInHierarchy;
            }
            if ((invUI != null) && wasInvUIShowing) invUI.ToggleInventory();
            if ((equipUI != null) && wasEquipUIShowing) equipUI.ToggleEquipment();
            if ((skillBarUI != null) && wasSkillBarUIShowing) skillBarUI.Panel.SetActive(show);
            if ((craftUI != null) && wasCraftUIShowing) craftUI.Panel.SetActive(show);
            if ((vendorUI != null) && wasVendorUIShowing) vendorUI.Panel.SetActive(show);
            if ((itemGroupUI != null) && wasItemGroupUIShowing) itemGroupUI.Panel.SetActive(show);
        }

        public void OnRecordPersistentData()
        {
            if (debug) Debug.Log("Dialogue System: " + name + ".OnRecordPersistentData()");
            SaveCurrencies();
            SaveItems();
            SaveEquipment();
            SaveSkillBar();
        }

        public void OnApplyPersistentData()
        {
            if (debug) Debug.Log("Dialogue System: " + name + ".OnApplyPersistentData()");
            if (!DoesActorHaveSaveData())
            {
                if (debug) Debug.Log("Dialogue System: " + name + " doesn't have previously-saved data. Leaving inventory as-is.");
                return;
            }
            LoadCurrencies();
            LoadItems();
            LoadEquipment();
            LoadSkillBar();
            SInventoryLua.RefreshCurrencyUI(invManager);
            SInventoryLua.RefreshItemUI(invManager);
        }

        public bool DoesActorHaveSaveData()
        {
            var result = DialogueLua.GetActorField(internalName, SInventoryLua.MaxItemsFieldName);
            return !result.Equals(Lua.NoResult) && result.HasReturnValue;
        }

        public void SaveCurrencies()
        {
            if (debug) Debug.Log("Dialogue System: " + name + ".SaveCurrencies()");
            if (invManager == null) return;
            foreach (var currency in invManager.Riches)
            {
                if (debug) Debug.Log("Dialogue System: Save Actor[" + name + "]." + SInventoryLua.GetCurrencyFieldName(currency.Name) + "=" + currency.Amount);
                DialogueLua.SetActorField(internalName, SInventoryLua.GetCurrencyFieldName(currency.Name), currency.Amount);
            }
        }

        public void LoadCurrencies()
        {
            if (invManager == null) return;
            foreach (var currency in invManager.Riches)
            {
                currency.Amount = DialogueLua.GetActorField(internalName, SInventoryLua.GetCurrencyFieldName(currency.Name)).AsInt;
                if (debug) Debug.Log("Dialogue System: Load Actor[" + name + "]." + SInventoryLua.GetCurrencyFieldName(currency.Name) + "<--" + currency.Amount);
            }
            if (invManager.SaveAndLoad) invManager.SaveCurrencies();
        }

        public void SaveItems()
        {
            if (debug) Debug.Log("Dialogue System: " + name + ".SaveItems()");
            if (invManager == null) return;
            StringBuilder itemNames = new StringBuilder();
            StringBuilder itemAmounts = new StringBuilder();
            for (int i = 0; i < invManager.MaxItems; i++)
            {
                var slot = invManager.Slots[i];
                string itemName = string.Empty;
                int itemAmount = 0;
                var item = (slot.IsTaken && slot.Item != null) ? slot.Item.GetComponent<global::Item>() : null;
                if (item != null)
                {
                    itemName = item.Name;
                    itemAmount = item.Amount;
                }
                itemNames.AppendFormat("{0}|", itemName);
                itemAmounts.AppendFormat("{0}|", itemAmount);
            }
            DialogueLua.SetActorField(internalName, SInventoryLua.MaxItemsFieldName, invManager.MaxItems);
            DialogueLua.SetActorField(internalName, SInventoryLua.ItemNamesFieldName, itemNames.ToString());
            DialogueLua.SetActorField(internalName, SInventoryLua.ItemAmountsFieldName, itemAmounts.ToString());
            if (debug)
            {
                Debug.Log("Dialogue System: Save Actor[" + name + "]." + SInventoryLua.ItemNamesFieldName + "=" + itemNames.ToString());
                Debug.Log("Dialogue System: Save Actor[" + name + "]." + SInventoryLua.ItemAmountsFieldName+ "=" + itemAmounts.ToString());
            }
        }

        public void LoadItems()
        {
            if (debug) Debug.Log("Dialogue System: " + name + ".LoadItems()");
            if (invManager == null) return;
            if (debug)
            {
                Debug.Log("Dialogue System: Load Actor[" + name + "]." + SInventoryLua.ItemNamesFieldName + "<--" + DialogueLua.GetActorField(internalName, SInventoryLua.ItemNamesFieldName).AsString);
                Debug.Log("Dialogue System: Load Actor[" + name + "]." + SInventoryLua.ItemAmountsFieldName + "<--" + DialogueLua.GetActorField(internalName, SInventoryLua.ItemAmountsFieldName).AsString);
            }
            invManager.Items = 0;
            int maxItems = Mathf.Min(invManager.MaxItems, DialogueLua.GetActorField(internalName, SInventoryLua.MaxItemsFieldName).AsInt);
            string[] itemNames = DialogueLua.GetActorField(internalName, SInventoryLua.ItemNamesFieldName).AsString.Split(new char[] { '|' });
            string[] itemAmounts = DialogueLua.GetActorField(internalName, SInventoryLua.ItemAmountsFieldName).AsString.Split(new char[] { '|' });
            for (int i = 0; i < invManager.MaxItems; i++)
            {
                var slot = invManager.Slots[i];
                global::Item item = null;
                int itemAmount = 0;
                if (i < maxItems)
                {
                    itemAmount = Tools.StringToInt(itemAmounts[i]);
                    if (debug && itemAmount > 0) Debug.Log("Dialogue System: Putting item '" + itemNames[i] + "' in inventory slot " + i);
                    if (itemAmount > 0) item = SInventoryLua.InstantiateItemPrefab(itemNames[i]);
                }
                if (slot.Item != null)
                {
                    if (debug) Debug.Log("Dialogue System: Destroying old item in inventory slot " + i + " first.");
                    Destroy(slot.Item.gameObject);
                }
                if (item == null)
                {
                    slot.IsTaken = false;
                    slot.Item = null;
                }
                else
                {
                    slot.IsTaken = true;
                    slot.Item = item.transform;
                    item.Amount = itemAmount;
                    var itemCollider = item.GetComponent<Collider>();
                    if (itemCollider != null) itemCollider.isTrigger = true;
                    var itemRenderer = item.GetComponent<Renderer>();
                    if (itemRenderer != null && item.gameObject.activeInHierarchy) itemRenderer.enabled = false;
                    item.transform.parent = invManager.transform;
                    item.transform.localPosition = Vector3.zero;
                    item.gameObject.SetActive(false);
                    invManager.Items++;
                }
            }
            if (invManager.SaveAndLoad) invManager.SaveItems();
        }

        public void SaveEquipment()
        {
            if (debug) Debug.Log("Dialogue System: " + name + ".SaveEquipment()");
            if (equipment == null) return;
            StringBuilder equipmentNames = new StringBuilder();
            for (int i = 0; i < equipment.EquipmentSlots.Length; i++)
            {
                var slot = equipment.EquipmentSlots[i];
                string itemName = string.Empty;
                var item = (slot.IsTaken && slot.Item != null) ? slot.Item.GetComponent<global::Item>() : null;
                if (item != null)
                {
                    itemName = item.Name;
                }
                equipmentNames.AppendFormat("{0}|", itemName);
            }
            DialogueLua.SetActorField(internalName, SInventoryLua.EquipmentNamesFieldName, equipmentNames.ToString());
            if (debug) Debug.Log("Dialogue System: Save Actor[" + name + "]." + SInventoryLua.EquipmentNamesFieldName + "=" + equipmentNames.ToString());
        }

        public void LoadEquipment()
        {
            if (debug) Debug.Log("Dialogue System: " + name + ".LoadEquipment()");
            if (equipment == null) return;
            if (debug) Debug.Log("Dialogue System: Load Actor[" + name + "]." + SInventoryLua.EquipmentNamesFieldName + "<--" + DialogueLua.GetActorField(internalName, SInventoryLua.EquipmentNamesFieldName).AsString);
            string[] equipmentNames = DialogueLua.GetActorField(internalName, SInventoryLua.EquipmentNamesFieldName).AsString.Split(new char[] { '|' });
            for (int i = 0; i < equipment.EquipmentSlots.Length; i++)
            {
                var slot = equipment.EquipmentSlots[i];
                global::Item item = null;
                int itemAmount = 0;
                if ((i < equipmentNames.Length) && !string.IsNullOrEmpty(equipmentNames[i]))
                {
                    if (debug) Debug.Log("Dialogue System: Putting equipment item '" + equipmentNames[i] + "' in equipment slot " + i);
                    item = SInventoryLua.InstantiateItemPrefab(equipmentNames[i]);
                    itemAmount = 1;
                }
                if (slot.Item != null)
                {
                    if (debug) Debug.Log("Dialogue System: Destroying old item in equipment slot " + i + " first.");
                    Destroy(slot.Item.gameObject);
                }
                if (item == null)
                {
                    slot.IsTaken = false;
                    slot.Item = null;
                    slot.Icon.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
                    slot.Icon.sprite = null;
                }
                else
                {
                    slot.IsTaken = true;
                    slot.Item = item.transform;
                    item.Amount = itemAmount;
                    var itemCollider = item.GetComponent<Collider>();
                    if (itemCollider != null) itemCollider.isTrigger = true;
                    var itemRenderer = item.GetComponent<Renderer>();
                    if (itemRenderer != null) itemRenderer.enabled = false;
                    item.transform.parent = equipment.transform;
                    item.transform.localPosition = Vector3.zero;
                    item.gameObject.SetActive(false);
                    slot.Icon.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                    slot.Icon.sprite = item.Icon;
                    equipment.ActivateEqObj(item.Name);
                }
            }
            invUI.IsEquippingItem = false;
            equipment.SaveEquipments();
        }

        public void SaveSkillBar()
        {
            if (debug) Debug.Log("Dialogue System: " + name + ".SaveSkillBar()");
            if (skillBar == null) return;
            StringBuilder names = new StringBuilder();
            StringBuilder amounts = new StringBuilder();
            for (int i = 0; i < skillBar.SkillSlot.Length; i++)
            {
                var slot = skillBar.SkillSlot[i];
                string itemName = string.Empty;
                int itemAmount = 0;
                var item = (slot.IsTaken && slot.Item != null) ? slot.Item.GetComponent<global::Item>() : null;
                if (item != null)
                {
                    itemName = item.Name;
                    itemAmount = item.Amount;
                }
                names.AppendFormat("{0}|", itemName);
                amounts.AppendFormat("{0}|", itemAmount);
            }
            DialogueLua.SetActorField(internalName, SInventoryLua.SkillBarNamesFieldName, names.ToString());
            DialogueLua.SetActorField(internalName, SInventoryLua.SkillBarAmountsFieldName, amounts.ToString());
        }

        public void LoadSkillBar()
        {
            if (debug) Debug.Log("Dialogue System: " + name + ".LoadSkillBar()");
            if (skillBar == null) return;
            string[] names = DialogueLua.GetActorField(internalName, SInventoryLua.SkillBarNamesFieldName).AsString.Split(new char[] { '|' });
            string[] amounts = DialogueLua.GetActorField(internalName, SInventoryLua.SkillBarAmountsFieldName).AsString.Split(new char[] { '|' });
            for (int i = 0; i < skillBar.SkillSlot.Length; i++)
            {
                var slot = skillBar.SkillSlot[i];
                global::Item item = null;
                int itemAmount = 0;
                if ((i < names.Length) && !string.IsNullOrEmpty(names[i]))
                {
                    item = SInventoryLua.InstantiateItemPrefab(names[i]);
                    if (i < amounts.Length) itemAmount = Tools.StringToInt(amounts[i]);
                }
                if (slot.Item != null) Destroy(slot.Item.gameObject);
                if (item == null)
                {
                    slot.IsTaken = false;
                    slot.Item = null;
                    slot.Icon.sprite = null;
                    slot.Icon.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
                }
                else
                {
                    slot.IsTaken = true;
                    slot.Item = item.transform;
                    slot.Icon.sprite = item.Icon;
                    slot.Icon.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                    var countText = slot.Icon.GetComponentInChildren<UnityEngine.UI.Text>();
                    if (countText != null)
                    {
                        countText.text = (itemAmount == 0) ? string.Empty : itemAmount.ToString();
                        countText.gameObject.SetActive(true);
                    }
                    item.Amount = itemAmount;
                    var itemCollider = item.GetComponent<Collider>();
                    if (itemCollider != null) itemCollider.isTrigger = true;
                    var itemRenderer = item.GetComponent<Renderer>();
                    if (itemRenderer != null) itemRenderer.enabled = false;
                    item.transform.parent = skillBar.transform;
                    item.transform.localPosition = Vector3.zero;
                    item.gameObject.SetActive(false);
                }
            }
            if (skillBar.SaveAndLoad) skillBar.SendMessage("SaveSkillBar");
        }

    }

}
