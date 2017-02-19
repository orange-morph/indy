using UnityEngine;
using System.Collections;
using PixelCrushers.DialogueSystem;
using PixelCrushers.DialogueSystem.SInventory;

namespace PixelCrushers.DialogueSystem.SequencerCommands {
	
	/// <summary>
	/// Sequencer command AdjustCurrency(subject, currency, amount)
	/// </summary>
	public class SequencerCommandAdjustCurrency : SequencerCommand {
		
		public void Start() {
			Transform subject = GetSubject(0);
			string currencyName = GetParameter(1);
			int amount = GetParameterAsInt(2);
			if (subject == null) {
				if (DialogueDebug.LogWarnings) Debug.LogWarning(string.Format("{0}: Sequencer: AdjustCurrency({1}, {2}, {3}): Can't find '{1}'", new object[] { DialogueDebug.Prefix, GetParameter(0), GetParameter(1), GetParameter(2) }));
			} else {
				string subjectName = OverrideActorName.GetActorName(subject);
				if (DialogueDebug.LogInfo) Debug.Log(string.Format("{0}: Sequencer: AdjustCurrency({1}, {2}, {3})", new object[] { DialogueDebug.Prefix, subjectName, currencyName, amount }));
				SInventoryLua.AdjustCurrency(subjectName, currencyName, amount);
			}
			Stop();
		}
		
	}
	
}
