using UnityEngine;
using System.Collections;

namespace PixelCrushers.DialogueSystem.SInventory {
		
	/// <summary>
	/// This script treats certain S-Inventory actions like conversations by
	/// sending OnConversationStart and OnConversationEnd to the actor. This
	/// allows you to use Set Component Enabled On Dialogue Event to disable
	/// and enable player camera and movement control during, for example, vendors.
	/// </summary>
	[AddComponentMenu("Dialogue System/Third Party/S-Inventory/Treat As Conversation")]
	public class SInventoryTreatAsConversation : MonoBehaviour {

        /// <summary>
        /// Set <c>true</c> for opening item groups such as containers.
        /// </summary>
        [Tooltip("Treat opening item groups the same as being in a conversation (e.g., freeze player).")]
        public bool itemGroups = true;

        /// <summary>
        /// Set <c>true</c> to for interacting with vendors.
        /// </summary>
        [Tooltip("Treat opening vendors the same as being in a conversation (e.g., freeze player).")]
        public bool vendors = true;

		public void OnVendorStart(Transform sInventoryObject) {
			if (vendors) SimulateConversationStart(sInventoryObject);
		}

		public void OnVendorEnd(Transform sInventoryObject) {
			if (vendors) SimulateConversationEnd(sInventoryObject);
		}

		public void OnItemGroupStart(Transform sInventoryObject) {
			if (itemGroups) SimulateConversationStart(sInventoryObject);
		}
		
		public void OnItemGroupEnd(Transform sInventoryObject) {
			if (itemGroups) SimulateConversationEnd(sInventoryObject);
		}

		public void SimulateConversationStart(Transform sInventoryObject) {
			SendMessage("OnConversationStart", sInventoryObject, SendMessageOptions.DontRequireReceiver);
		}

		public void SimulateConversationEnd(Transform sInventoryObject) {
			SendMessage("OnConversationEnd", sInventoryObject, SendMessageOptions.DontRequireReceiver);
		}
		
	}

}
