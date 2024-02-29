using System.Collections.Generic;
using System.Linq;
using InventorySystem.Items;
using UnityEngine;

namespace InventorySystem
{
    public class BasicInventory : MonoBehaviour
    {
        public InventoryDictionary InventoryDictionary { get; set; } = new();

        public virtual void AddItem(IItem item, int amount)
        {
            InventoryDictionary.AddItem(item, amount);
        }

        public virtual void RemoveItem(IItem item, int amount)
        {
            InventoryDictionary.RemoveItem(item, amount);
        }

        public int GetAmount(IItem item)
        {
            return InventoryDictionary.GetAmount(item);
        }

        public List<KeyValuePair<IItem, int>> GetItemList()
        {
            return InventoryDictionary.ToList();
        }
        
        /// <summary>
        /// Calculates number of unique items
        /// </summary>
        /// <returns></returns>
        public int UniqueItemCount()
        {
            return InventoryDictionary.UniqueItemCount();
        }
        /// <summary>
        /// Calculates the total number of items in inventory
        /// </summary>
        /// <returns></returns>
        public int ItemCount()
        {
            return InventoryDictionary.ItemCount();
        }
    }
}
