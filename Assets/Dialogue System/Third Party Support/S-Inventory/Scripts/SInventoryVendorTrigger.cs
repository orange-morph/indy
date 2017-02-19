using UnityEngine;
using System.Collections;

namespace PixelCrushers.DialogueSystem.SInventory {
		
	/// <summary>
	/// This component starts the S-Inventory vendor when the game object receives a specified 
	/// trigger event. For example, you can add this and a static trigger collider to an area. 
	/// When the player enters the trigger area, this component could open the vendor.
	/// 
	/// It also sends OnSInventoryGUIStart(transform) and OnSInventoryGUIEnd(transform) messages, 
	/// shows the cursor, and temporarily disables BarkOnIdle.
	/// </summary>
	[AddComponentMenu("Dialogue System/Third Party/S-Inventory/S-Inventory Vendor Trigger")]
	public class SInventoryVendorTrigger : DialogueEventStarter {
	
		/// <summary>
		/// The trigger that runs the Lua code..
		/// </summary>
		public DialogueTriggerEvent trigger = DialogueTriggerEvent.OnUse;

        /// <summary>
        /// The conditions under which the trigger will fire.
        /// </summary>
        [Tooltip("If set, only allow the vendor to open if the condition is true.")]
        public Condition condition;

        [Tooltip("Close the vendor window if the player gets this far away.")]
        public float walkAwayDistance = 8;
		
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
					var vendor = Tools.GetComponentAnywhere<Vendor>(gameObject);
					if (vendor == null) {
						if (DialogueDebug.LogWarnings) Debug.LogWarning(string.Format("{0}: Can't find a Vendor component on {1} or its parent/children", new object[] { DialogueDebug.Prefix, name }), gameObject);
					} else {
						StartCoroutine(RunVendor(vendor, actor));
					}
				}
			} finally {
				tryingToStart = false;
			}
		}

		private IEnumerator RunVendor(Vendor vendor, Transform actor) {
			if (vendor == null) yield break;
			if (actor != null) actor.SendMessage("OnVendorStart", vendor.transform, SendMessageOptions.DontRequireReceiver);
			Tools.SetCursorActive(true);
			bool wasBarkOnIdleEnabled = DisableBarkOnIdle();
			float originalMinDistance = vendor.MinDistance;
			vendor.MinDistance = walkAwayDistance;
			if (vendor.GetComponent<AudioSource>() == null) vendor.gameObject.AddComponent<AudioSource>();
			yield return null;
			vendor.TriggerVendor();
			yield return null;
			yield return null;
			var audioSource = vendor.GetComponent<AudioSource>();
			if (audioSource != null && vendor.Opening != null) audioSource.PlayOneShot(vendor.Opening);
			while (vendor.Panel.Panel.activeInHierarchy) {
				yield return null;
			}
			vendor.MinDistance = originalMinDistance;
			if (actor != null) actor.SendMessage("OnVendorEnd", vendor.transform, SendMessageOptions.DontRequireReceiver);
			if (wasBarkOnIdleEnabled) EnableBarkOnIdle();
			DestroyIfOnce();
		}

		private bool DisableBarkOnIdle() {
			var barkOnIdle = Tools.GetComponentAnywhere<BarkOnIdle>(gameObject);
			if (barkOnIdle != null) {
				var wasEnabled = barkOnIdle.enabled;
				barkOnIdle.enabled = false;
				return wasEnabled;
			} else {
				return false;
			}
		}

		private void EnableBarkOnIdle() {
			var barkOnIdle = Tools.GetComponentAnywhere<BarkOnIdle>(gameObject);
			if (barkOnIdle != null) barkOnIdle.enabled = true;
		}
		
	}

}
