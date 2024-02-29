using System;
using InventorySystem.Items;
using UnityEngine;

namespace InventorySystem
{
    [RequireComponent(typeof(BasicInventory))]
    public class ChestSeeder : MonoBehaviour
    {
        [SerializeField, HideInInspector] private BasicInventory m_BasicInventory;

        [SerializeField] private EquipmentEntry[] m_BaseItems;

        private void Start()
        {
            foreach (EquipmentEntry item in m_BaseItems)
            {
                AddItem(item);
            }
        }

        public void AddItem(EquipmentEntry equipmentEntry)
        {
            switch (equipmentEntry.item)
            {
                case UniqueItem uniqueItem:
                    m_BasicInventory.AddItem(uniqueItem.CreateInstance(equipmentEntry.ilvl), equipmentEntry.amount);
                    break;
                case StackableItem stackableItem:
                    m_BasicInventory.AddItem(stackableItem, equipmentEntry.amount);
                    break;
                default:
                    throw new Exception("Item type is sus");
            }
        }

        private void OnValidate()
        {
            if (!m_BasicInventory) m_BasicInventory = GetComponent<BasicInventory>();
        }
    }
}