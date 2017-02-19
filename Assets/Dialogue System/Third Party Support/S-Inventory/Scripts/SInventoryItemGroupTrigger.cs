using UnityEngine;
using System.Collections;

namespace PixelCrushers.DialogueSystem.SInventory {
		
	/// <summary>
	/// This component starts the S-Inventory item group GUI when the game object receives a specified 
	/// trigger event. For example, you can add this and a static trigger collider to an area. 
	/// When the player enters the trigger area, this component could open the item group.
	/// 
	/// It also sends OnItemGroupStart(transform) and OnItemGroupEnd(transform) messages and
	/// shows the cursor.
	/// </summary>
	[AddComponentMenu("Dialogue System/Third Party/S-Inventory/S-Inventory Item Group Trigger")]
	public class SInventoryItemGroupTrigger : DialogueEventStarter {
	
		/// <summary>
		/// The trigger that opens the item group.
		/// </summary>
		public DialogueTriggerEvent trigger = DialogueTriggerEvent.OnUse;

        /// <summary>
        /// The conditions under which the trigger will fire.
        /// </summary>
        [Tooltip("If set, only allow the item group to open if the condition is true.")]
        public Condition condition;

        [Tooltip("Close the item group if the player gets this far away.")]
        public float walkAwayDistance = 8;

        [Tooltip("Show this message if the item group is empty.")]
        public string messageIfEmpty = "Empty";

        [Tooltip("Optional localized text table to localize the Message If Empty.")]
        public LocalizedTextTable localizedTextTable;
		
		private bool tryingToStart = false;

		public void OnBarkEnd(Transform actor) {
			if (enabled && (trigger == DialogueTriggerEvent.OnBarkEnd)) TryStart(actor);
		}
		
		public void OnConversationEnd(Transform actor) {
			if (enabled && (trigger == DialogueTriggerEvent.OnConversationEnd)) TryStart(actor);
		}
		
		public void OnSequenceEnd(Transform actor) {
			if (enabled && (trigger == DialogueTriggerEvent.OnSequenceEnd)) TryStart(actor);
		}
		
		public void OnUse(Transform actor) {
			if (enabled && (trigger == DialogueTriggerEvent.OnUse)) TryStart(actor);
		}
		
		public void OnUse(string message) {
			if (enabled && (trigger == DialogueTriggerEvent.OnUse)) TryStart(null);
		}
		
		public void OnUse() {
			if (enabled && (trigger == DialogueTriggerEvent.OnUse)) TryStart(null);
		}
		
		public void OnTriggerEnter(Collider other) {
			if (enabled && (trigger == DialogueTriggerEvent.OnTriggerEnter)) TryStart(other.transform);
		}
		
		public void OnTriggerEnter2D(Collider2D other) {
			if (enabled && (trigger == DialogueTriggerEvent.OnTriggerEnter)) TryStart(other.transform);
		}
		
		void Start() {
			// Waits one frame to allow all other components to finish their Start() methods.
			if (trigger == DialogueTriggerEvent.OnStart) StartCoroutine(StartAfterOneFrame());
		}
		
		void OnEnable() {
			// Waits one frame to allow all other components to finish their OnEnable() methods.
			if (trigger == DialogueTriggerEvent.OnEnable) StartCoroutine(StartAfterOneFrame());
		}
		
		private IEnumerator StartAfterOneFrame() {
			yield return null;
			TryStart(null);
		}
		
		/// <summary>
		/// Runs the Lua code if the condition is true.
		/// </summary>
		public void TryStart(Transform actor) {
			if (tryingToStart) return;
			tryingToStart = true;
			try {
				if ((condition == null) || condition.IsTrue(actor)) {
					var itemGroup = Tools.GetComponentAnywhere<ItemGroup>(gameObject);
					if (itemGroup == null) {
						if (DialogueDebug.LogWarnings) Debug.LogWarning(string.Format("{0}: Can't find an ItemGroup component on {1} or its parent/children", new object[] { DialogueDebug.Prefix, name }), gameObject);
					} else {
						StartCoroutine(RunItemGroup(itemGroup, actor));
					}
				}
			} finally {
				tryingToStart = false;
			}
		}

		private IEnumerator RunItemGroup(ItemGroup itemGroup, Transform actor) {
			if (itemGroup == null) yield break;
			if (IsItemGroupEmpty(itemGroup)) {
				if (!string.IsNullOrEmpty(messageIfEmpty)) {
					DialogueManager.ShowAlert(((localizedTextTable != null) && localizedTextTable.ContainsField(messageIfEmpty)) ? localizedTextTable[messageIfEmpty] : messageIfEmpty);
				}
				yield break;
			}
			if (actor != null) actor.SendMessage("OnItemGroupStart", itemGroup.transform, SendMessageOptions.DontRequireReceiver);
			Tools.SetCursorActive(true);
			float originalMinDistance = itemGroup.MinDistance;
			itemGroup.MinDistance = walkAwayDistance;
			itemGroup.TriggerItemGroup();
			itemGroup.ItemSelected = null;
			var audioSource = itemGroup.GetComponent<AudioSource>();
			if (audioSource != null && itemGroup.Opening != null) audioSource.PlayOneShot(itemGroup.Opening);
			while (itemGroup.Panel.Panel.activeInHierarchy) {
				yield return null;
			}
			itemGroup.MinDistance = originalMinDistance;
			if (actor != null) actor.SendMessage("OnItemGroupEnd", itemGroup.transform, SendMessageOptions.DontRequireReceiver);
			if (actor != null) SInventoryLua.RefreshCurrencyUI(SInventoryLua.GetActorComponent<InventoryManager>(actor));
			DestroyIfOnce();
		}

		private bool IsItemGroupEmpty(ItemGroup itemGroup) {
			if (itemGroup == null) return true;
			foreach (var item in itemGroup.Content) {
				if (item != null) return false;
			}
			return true;
		}
		
	}

}
