using System;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using System.Text;
using UnityEngine;

namespace Devdog.InventoryPro
{
    [System.Serializable]
    public class ItemCollectionSerializationModel
    {
        public InventoryItemSerializationModel[] items = new InventoryItemSerializationModel[0];
        public CurrencyDecoratorSerializationModel[] currencies = new CurrencyDecoratorSerializationModel[0];
        
        public ItemCollectionSerializationModel()
        { }

        public ItemCollectionSerializationModel(ItemCollectionBase itemCollection)
        {
            FromCollection(itemCollection);
        }

        /// <summary>
        /// Fill this data model with a collection reference.
        /// Gets the collection data from the collection and stores it in this serializable model.
        /// </summary>
        /// <param name="collection"></param>
        public void FromCollection(ItemCollectionBase collection)
        {
            currencies = collection.currenciesGroup.lookups.Select(o => new CurrencyDecoratorSerializationModel(o)).ToArray();
            items = collection.items.Select(o => new InventoryItemSerializationModel(o.item)).ToArray();
        }

        public void ToCollection(ItemCollectionBase collection)
        {
            var deserializedItems = items.Select(o => o.ToItem()).ToArray();
            if (collection.useReferences)
            {
                collection.Resize((uint)deserializedItems.Length);
                for (int i = 0; i < deserializedItems.Length; i++)
                {
                    collection[i].item = deserializedItems[i];
                    collection[i].Repaint();
                }
            }
            else
            {
                collection.SetItems(deserializedItems, true);
            }

            var deserializedCurrencies = currencies.Select(o => o.ToCurrencyDecorator()).ToList();

            collection.UnRegisterCurrencyEvents();
            collection.currenciesGroup = new CurrencyDecoratorCollection(false);
            collection.currenciesGroup.lookups = deserializedCurrencies;
            collection.RegisterCurrencyEvents();
        }
    }
}
