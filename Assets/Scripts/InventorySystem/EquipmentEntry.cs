using System;
using InventorySystem.Items;

namespace InventorySystem
{
    /// <summary>
    /// Uzywane jest do ustawienia startowego ekwipunku gracza w edytorze UNITY.
    /// </summary>
    [Serializable]
    public class EquipmentEntry
    {
        public BaseItem item;
        public int amount;
        public int ilvl;
    }
}