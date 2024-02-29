using InventorySystem;
using InventorySystem.UI;
using UnityEngine;

namespace Interactions
{
    public class Lootable : Interactable
    {
        [Space]
        public string lootableName;
        [SerializeField] private BasicInventory m_Inventory;

        public override void Interact()
        {
            base.Interact();
            
            if (!CanvasController.Instance.inventoryInterface.isActiveAndEnabled)
            {
                CanvasController.Instance.ToggleInventory();
            }
            CanvasController.Instance.inventoryInterface.CreateTradePanel(m_Inventory, InventoryInterfaceType.Looting, lootableName);
        }

        private void OnValidate()
        {
            if (m_Inventory == null) m_Inventory = GetComponent<BasicInventory>();
        }
        
        private void OnMouseOver()
        {
            Debug.Log($"Chuj {gameObject.name}");
        }
    }
}
