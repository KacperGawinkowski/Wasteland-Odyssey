using System.Collections;
using HealthSystem;
using Player;
using UnityEngine;

namespace InventorySystem.Items
{
    [CreateAssetMenu(fileName = nameof(UsableItem), menuName = "InventorySystem/Items/" + nameof(UsableItem))]
    public class Medicament : UsableItem
    {
        [Header("Medicament Options")] 
        public int HealValue;
        public bool FullHeal;

        public override IEnumerator Use(HealthController healthController, EquipmentController equipmentController)
        {
            yield return new WaitForSeconds(UseTime);
            if (FullHeal)
            {
                healthController.HealAll();
            }
            else
            {
                healthController.HealParts(HealValue);
            }
            equipmentController.RemoveItem(this,1);
            
            if (equipmentController.isPlayer)
            {
                CanvasController.Instance.inventoryInterface.UpdateEntirePlayerPanel();
            }
        }
    }
}