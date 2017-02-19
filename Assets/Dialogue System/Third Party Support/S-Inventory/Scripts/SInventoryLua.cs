using UnityEngine;
using PixelCrushers.DialogueSystem;

namespace PixelCrushers.DialogueSystem.SInventory
{

    /// <summary>
    /// This static class provides Lua functions to access S-Inventory.
    /// You must call RegisterLuaFunctions() at least once before using
    /// the Lua functions.
    /// </summary>
    public static class SInventoryLua
    {

        private static bool hasRegisteredLuaFunctions = false;

        /// <summary>
        /// Registers the S-Inventory Lua functions with the Dialogue System.
        /// </summary>
        public static void RegisterLuaFunctions()
        {
            if (hasRegisteredLuaFunctions) return;
            hasRegisteredLuaFunctions = true;
            Lua.RegisterFunction("GetCurrency", null, SymbolExtensions.GetMethodInfo(() => GetCurrency(string.Empty, string.Empty)));
            Lua.RegisterFunction("SetCurrency", null, SymbolExtensions.GetMethodInfo(() => SetCurrency(string.Empty, string.Empty, (double)0)));
            Lua.RegisterFunction("AdjustCurrency", null, SymbolExtensions.GetMethodInfo(() => AdjustCurrency(string.Empty, string.Empty, (double)0)));
            Lua.RegisterFunction("GetItemAmount", null, SymbolExtensions.GetMethodInfo(() => GetItemAmount(string.Empty, string.Empty)));
            Lua.RegisterFunction("AddItem", null, SymbolExtensions.GetMethodInfo(() => AddItem(string.Empty, string.Empty, (double)0)));
            Lua.RegisterFunction("RemoveItem", null, SymbolExtensions.GetMethodInfo(() => RemoveItem(string.Empty, string.Empty, (double)0)));
        }

        /// <summary>
        /// Finds an actor by its GameObject name.
        /// </summary>
        /// <returns>The actor.</returns>
        /// <param name="actor">Actor.</param>
        public static GameObject FindActor(string actor)
        {
            return string.IsNullOrEmpty(actor) ? GameObject.FindGameObjectWithTag("Player") : Tools.GameObjectHardFind(actor);
        }

        /// <summary>
        /// Gets the InventoryManager on a GameObject with the specified name.
        /// </summary>
        /// <returns>The inventory manager.</returns>
        /// <param name="actor">Actor.</param>
        public static T GetActorComponent<T>(string actor) where T : Component
        {
            var go = FindActor(actor);
            var component = (go != null) ? go.GetComponentInChildren<T>() : null;
            if (component == null) component = GameObject.FindObjectOfType<T>();
            if ((component == null) && DialogueDebug.LogWarnings) Debug.LogWarning(string.Format("{0}: Can't find {1} on {2}", new object[] { DialogueDebug.Prefix, typeof(T).Name, actor }));
            return component;
        }

        /// <summary>
        /// Gets the InventoryManager on a GameObject with the specified name.
        /// </summary>
        /// <returns>The inventory manager.</returns>
        /// <param name="actor">Actor.</param>
        public static T GetActorComponent<T>(Transform actor) where T : Component
        {
            var go = (actor == null) ? null : actor.gameObject;
            var component = (go != null) ? go.GetComponentInChildren<T>() : null;
            if (component == null) component = GameObject.FindObjectOfType<T>();
            if ((component == null) && DialogueDebug.LogWarnings) Debug.LogWarning(string.Format("{0}: Can't find {1} on {2}", new object[] { DialogueDebug.Prefix, typeof(T).Name, actor }));
            return component;
        }


        /// <summary>
        /// Gets the actor name to use in the Lua environment.
        /// </summary>
        /// <returns>The actor name.</returns>
        /// <param name="actor">Actor.</param>
        public static string GetActorName(string actor)
        {
            var go = FindActor(actor);
            return (go != null) ? OverrideActorName.GetActorName(go.transform) : "Actor";
        }

        public static string GetCurrencyFieldName(string currencyName)
        {
            return "Currency_" + currencyName;
        }

        public const string MaxItemsFieldName = "MaxItems";
        public const string ItemNamesFieldName = "ItemNames";
        public const string ItemAmountsFieldName = "ItemAmounts";
        public const string EquipmentNamesFieldName = "EquipmentNames";
        public const string EquipmentAmountsFieldName = "EquipmentAmounts";
        public const string SkillBarNamesFieldName = "SkillBarNames";
        public const string SkillBarAmountsFieldName = "SkillBarAmounts";

        public static double GetCurrency(string subject, string currencyName)
        {
            var invManager = GetActorComponent<InventoryManager>(subject);
            if (invManager == null) return 0;
            foreach (var currency in invManager.Riches)
            {
                if (string.Equals(currency.Name, currencyName))
                {
                    return currency.Amount;
                }
            }
            return 0;
        }

        public static void SetCurrency(string subject, string currencyName, double amount)
        {
            var invManager = GetActorComponent<InventoryManager>(subject);
            if (invManager == null) return;
            foreach (var currency in invManager.Riches)
            {
                if (string.Equals(currency.Name, currencyName))
                {
                    currency.Amount = (int)amount;
                    PlayAudio(invManager, invManager.NewCurrency);
                    DialogueLua.SetActorField(GetActorName(subject), GetCurrencyFieldName(currencyName), amount);
                }
            }
            invManager.SaveCurrencies();
            RefreshCurrencyUI(invManager);
        }

        public static void RefreshCurrencyUI(InventoryManager invManager)
        {
            if (invManager == null || invManager.InvUI == null) return;
            var originalInvID = invManager.InvUI.InvID;
            invManager.InvUI.InvID = 2; // Special code for currency.
            invManager.InvUI.RefreshRiches();
            invManager.InvUI.InvID = originalInvID;
        }

        public static void AdjustCurrency(string subject, string currencyName, double amount)
        {
            var current = GetCurrency(subject, currencyName);
            SetCurrency(subject, currencyName, current + amount);
        }

        public static double GetItemAmount(string subject, string itemName)
        {
            var invManager = GetActorComponent<InventoryManager>(subject);
            if (invManager == null) return 0;
            int amount = 0;
            for (int i = 0; i < invManager.MaxItems; i++)
            {
                var slot = invManager.Slots[i];
                if (slot.IsTaken && (slot.Item != null))
                {
                    var item = slot.Item.GetComponent<global::Item>();
                    if (item != null && string.Equals(item.Name, itemName))
                    {
                        amount += item.Amount;
                    }
                }
            }
            var equipment = GetActorComponent<Equipment>(subject);
            if (equipment != null)
            {
                foreach (var slot in equipment.EquipmentSlots)
                {
                    if (slot.IsTaken && (slot.Item != null))
                    {
                        var item = slot.Item.GetComponent<global::Item>();
                        if (item != null && string.Equals(item.Name, itemName))
                        {
                            amount += item.Amount;
                        }
                    }
                }
            }
            return amount;
        }

        public static void AddItem(string subject, string itemName, double amount)
        {
            AddItem(GetActorComponent<InventoryManager>(subject), itemName, amount);
        }

        public static void AddItem(Transform actor, string itemName, double amount)
        {
            if (actor == null) return;
            AddItem(GetActorComponent<InventoryManager>(actor), itemName, amount);
        }

        public static void AddItem(InventoryManager invManager, string itemName, double amount)
        {
            if (invManager == null) return;
            var item = InstantiateItemPrefab(itemName);
            if (item == null)
            {
                if (DialogueDebug.LogWarnings) Debug.LogWarning("Couldn't instantiate item '" + itemName + "' to add it.");
                return;
            }
            item.Amount = (int)amount;
            if (item != null) invManager.AddItem(item.transform);
            RefreshItemUI(invManager);
        }

        public static void RemoveItem(string subject, string itemName, double amount)
        {
            RemoveItem(GetActorComponent<InventoryManager>(subject), itemName, amount);
        }

        public static void RemoveItem(Transform actor, string itemName, double amount)
        {
            if (actor == null) return;
            RemoveItem(GetActorComponent<InventoryManager>(actor), itemName, amount);
        }

        public static void RemoveItem(InventoryManager invManager, string itemName, double amount)
        {
            if (invManager == null) return;
            var item = FindItemInInventory(invManager, itemName);
            if (item == null)
            {
                var equipment = invManager.GetComponent<Equipment>();
                if (equipment == null & invManager.transform.parent != null)
                {
                    equipment = invManager.transform.parent.GetComponentInChildren<Equipment>();
                }
                if (equipment != null)
                {
                    foreach (var slot in equipment.EquipmentSlots)
                    {
                        if (slot.IsTaken && (slot.Item != null))
                        {
                            var eqItem = slot.Item.GetComponent<global::Item>();
                            if ((eqItem != null) && string.Equals(eqItem.Name, itemName))
                            {
                                slot.IsTaken = false;
                                slot.Name = string.Empty;
                                slot.Item = null;
                                if (slot.Icon != null)
                                {
                                    slot.Icon.sprite = null;
                                    slot.Icon.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
                                }
                                GameObject.Destroy(eqItem);
                                return;
                            }
                        }
                    }
                }
            }
            if (item == null) item = InstantiateItemPrefab(itemName);
            if (item == null)
            {
                if (DialogueDebug.LogWarnings) Debug.LogWarning("Couldn't instantiate item '" + itemName + "' to remove it.");
                return;
            }
            invManager.RemoveItem(item.transform, (int)amount);
            RefreshItemUI(invManager);
        }

        public static void RefreshItemUI(InventoryManager invManager)
        {
            if (invManager == null || invManager.InvUI == null) return;
            var originalInvID = invManager.InvUI.InvID;
            invManager.InvUI.InvID = 1; // Special code for items.
            invManager.InvUI.RefreshItems();
            invManager.InvUI.InvID = originalInvID;
        }

        public static global::Item FindItemInInventory(InventoryManager invManager, string itemName)
        {
            if (invManager == null) return null;
            for (int i = 0; i < invManager.MaxItems; i++)
            {
                var slot = invManager.Slots[i];
                if (slot.IsTaken && (slot.Item != null))
                {
                    var item = slot.Item.GetComponent<global::Item>();
                    if ((item != null) && string.Equals(item.Name, itemName))
                    {
                        return item;
                    }
                }
            }
            return null;
        }

        public static global::Item InstantiateItemPrefab(string itemName)
        {
            global::Item item = null;
            if (string.IsNullOrEmpty(itemName))
            {
                if (DialogueDebug.LogWarnings) Debug.LogWarning(string.Format("{0}: Can't load an S-Inventory item with a blank name", DialogueDebug.Prefix));
                return null;
            }
            var prefab = DialogueManager.LoadAsset(itemName, typeof(GameObject));
            if (prefab == null)
            {
                if (DialogueDebug.LogWarnings) Debug.LogWarning(string.Format("{0}: Can't load S-Inventory item prefab '{1}'",
                    new object[] { DialogueDebug.Prefix, itemName }));
                return null;
            }
            var go = GameObject.Instantiate(prefab) as GameObject;
            if (go == null)
            {
                if (DialogueDebug.LogWarnings) Debug.LogWarning(string.Format("{0}: Can't instantiate S-Inventory item from prefab '{1}'",
                    new object[] { DialogueDebug.Prefix, itemName }));
                return null;
            }
            item = go.GetComponent<global::Item>();
            if (item == null)
            {
                if (DialogueDebug.LogWarnings) Debug.LogWarning(string.Format("{0}: S-Inventory item prefab '{1}' doesn't have an Item component",
                    new object[] { DialogueDebug.Prefix, itemName }));
            }
            return item;
        }

        private static void PlayAudio(InventoryManager invManager, AudioClip audioClip)
        {
            if (invManager == null || audioClip == null) return;
            var audioSource = Tools.GetComponentAnywhere<AudioSource>(invManager.gameObject);
            if (audioSource != null) audioSource.PlayOneShot(audioClip);
        }

        public static string SanitizeItemName(string s)
        {
            return s.Replace(",", "_COMMA_").Replace(":", "_COLON_").Replace("'", "_QUOTE_").Replace("\"", "_DQUOTE_").Replace("\n", "_NEWLINE_");
        }

        public static string DesanitizeItemName(string s)
        {
            return s.Replace("_COMMA_", ",").Replace("_COLON_", ":").Replace("_QUOTE_", "'").Replace("_DQUOTE_", "\"").Replace("_NEWLINE_", "\n");
        }



    }

}
