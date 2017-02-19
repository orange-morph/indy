using UnityEngine;
using System.Collections.Generic;
using PixelCrushers.DialogueSystem;

namespace PixelCrushers.DialogueSystem.SInventory {

	/// <summary>
	/// Persistent data component for S-Inventory vendors.
	/// </summary>
	[AddComponentMenu("Dialogue System/Third Party/S-Inventory/S-Inventory Persistent Vendor")]
	public class SInventoryPersistentVendor : MonoBehaviour {

        [Tooltip("Save the vendor's for sale items under this name. Uses the GameObject name if blank. The Save Name must be unique.")]
        public string saveName = string.Empty;

		public bool debug = false;

		private Vendor m_vendor;
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

        public string variableName { get { return "VendorForSale_" + internalName; } }

        protected virtual void Awake()
        {
            m_vendor = GetComponent<Vendor>();
        }

        protected virtual void OnEnable()
		{
			PersistentDataManager.RegisterPersistentData(gameObject);
		}
		
		protected virtual void OnDisable()
		{
			PersistentDataManager.UnregisterPersistentData(gameObject);
		}
		
		/// <summary>
		/// Saves the vendors's for sale contents in Variable VendorForSale_saveName. The 
		/// saved data is a string in the format "#:name, #:name, ..." where # is the 
		/// amount and name is the name of the item.
		/// </summary>
		public void OnRecordPersistentData() {
			if (m_vendor == null) return;

			var saveData = m_vendor.ForSale.Length + ",";
			foreach (var itemTransform in m_vendor.ForSale) {
				var item = (itemTransform == null) ? null : itemTransform.GetComponent<global::Item>();
				saveData += (item == null) ? "0:null," : item.Amount.ToString() + ":" + SInventoryLua.SanitizeItemName(item.Name) + ",";
			}
			DialogueLua.SetVariable(variableName, saveData);
			if (debug) Debug.Log("Dialogue System: Saving vendor state to save name '" + variableName + "': " + saveData, this);
		}

		/// <summary>
		/// Replaces the contents of the item group with the saved data.
		/// </summary>
		public void OnApplyPersistentData() {
			if (m_vendor == null) return;
            var saveData = DialogueLua.DoesVariableExist(variableName) ? DialogueLua.GetVariable(variableName).AsString : string.Empty;
			if (string.IsNullOrEmpty(saveData) || string.Equals(saveData, "nil")) {
				if (debug) Debug.Log("Dialogue System: Vendor " + name + " doesn't have previously-saved data. Leaving inventory as-is.", this);
				return;
			}
			if (debug) Debug.Log("Dialogue System: Restoring vendor state from save name '" + variableName + "': " + saveData, this);

			// Remove the current contents:
			foreach (var item in m_vendor.ForSale) {
                if (item.gameObject != null && item.gameObject.transform.parent != null)
                {
                    if (debug) Debug.Log("Dialogue System: Destroying '" + item.name + "' to make room to restore saved items in " + name + ".");
                    Destroy(item.gameObject);
                }
			}
			m_vendor.ForSale = new Transform[0];
			
			// Add the new contents:
			var content = new List<Transform>();
			var itemRecords = saveData.Split(new char[] { ',' });
			foreach (var itemRecord in itemRecords) {
				var fields = itemRecord.Split(new char[] { ':' });
				if (fields.Length == 2) {
					var amount = Tools.StringToInt(fields[0]);
                    var itemName = SInventoryLua.DesanitizeItemName(fields[1]);
                    if (debug) Debug.Log("Dialogue System: Adding " + amount + " '" + itemName + "' to " + name, this);
					var item = SInventoryLua.InstantiateItemPrefab(itemName);
					if (item != null) {
						item.name = itemName;
						item.transform.SetParent(m_vendor.transform);
						item.gameObject.SetActive(false);
						item.Amount = amount;
						content.Add(item.transform);
					}
				}
			}
			m_vendor.ForSale = content.ToArray();
		}

	}

}
