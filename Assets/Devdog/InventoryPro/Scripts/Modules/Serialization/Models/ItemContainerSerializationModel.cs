using System;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using System.Text;
using UnityEngine;
using UnityEngine.Assertions;
using Object = UnityEngine.Object;

namespace Devdog.InventoryPro
{
    [System.Serializable]
    public class ItemContainerSerializationModel
    {
        public InventoryItemSerializationModel[] items = new InventoryItemSerializationModel[0];
        

        public ItemContainerSerializationModel()
        { }

        public ItemContainerSerializationModel(IInventoryItemContainer container)
        {
            FromContainer(container);
        }

        /// <summary>
        /// Fill this data model with a collection reference.
        /// Gets the collection data from the collection and stores it in this serializable model.
        /// </summary>
        public void FromContainer(IInventoryItemContainer container)
        {
            items = container.items.Select(o => new InventoryItemSerializationModel(o)).ToArray();
        }

        /// <summary>
        /// Fill a collection using this data object.
        /// </summary>
        public void ToContainer(IInventoryItemContainer container)
        {
            container.items = items.Select(o => o.ToItem()).ToArray();
        }
    }
}
