using System.Collections;
using System.Collections.Generic;
using System.Linq;
using InventorySystem.Items;
using NPC;
using UnityEngine;

namespace InventorySystem
{
    public class InventoryDictionary : IEnumerable<KeyValuePair<IItem, int>>
    {
        private readonly Dictionary<IItem, int> m_ItemContainer = new();

        public void AddItem(IItem item, int amount)
        {
            if (m_ItemContainer.ContainsKey(item))
            {
                m_ItemContainer[item] += amount;
            }
            else
            {
                m_ItemContainer.Add(item, amount);
            }
        }

        /// <summary>
        /// Removes item from inventory
        /// </summary>
        /// <param name="item"></param>
        /// <param name="amount"></param>
        /// <returns>number of removed items</returns>
        public int RemoveItem(IItem item, int amount)
        {
            if (m_ItemContainer.ContainsKey(item))
            {
                int itemCount = m_ItemContainer[item];

                int temp = itemCount - amount;
                if (temp <= 0)
                {
                    m_ItemContainer.Remove(item);
                    return itemCount;
                }
                else
                {
                    m_ItemContainer[item] -= amount;
                    return amount;
                }
            }

            return 0;
        }

        public int GetAmount(IItem item)
        {
            if (item != null)
            {
                return m_ItemContainer.GetValueOrDefault(item);
            }

            return 0;
        }

        public void SetAmount(IItem item, int amount)
        {
            m_ItemContainer[item] = amount;
        }

        public void Clear()
        {
            m_ItemContainer.Clear();
        }

        public int UniqueItemCount()
        {
            return m_ItemContainer.Count;
        }

        public int ItemCount()
        {
            int count = 0;
            foreach (var item in m_ItemContainer)
            {
                count += item.Value;
            }

            return count;
        }

        public IEnumerator<KeyValuePair<IItem, int>> GetEnumerator()
        {
            return m_ItemContainer.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}