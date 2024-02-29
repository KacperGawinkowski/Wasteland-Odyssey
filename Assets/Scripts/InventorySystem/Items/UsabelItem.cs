using System.Collections;
using HealthSystem;
using Player;
using UnityEngine;

namespace InventorySystem.Items
{
    public abstract class UsableItem : StackableItem
    {
        [Header("UsableItem Options")]
        public float UseTime;
        public float UseCooldown;
        public string UseText;

        public abstract IEnumerator Use(HealthController healthController, EquipmentController equipmentController);
    }
}
