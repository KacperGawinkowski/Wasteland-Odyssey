using System;
using System.Collections;
using HealthSystem;
using InventorySystem.Items;
using Player;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace InventorySystem.UI
{
    public class InventoryListEntryInteractions : MonoBehaviour
    {
        [SerializeField] private Button m_equipButton;
        [SerializeField] private Button m_takeOffButton;
        [SerializeField] private Button m_dropButton;
        [SerializeField] private Button m_useButton;
        [SerializeField] private InventoryListEntry m_ThisEntry;

        private IItem m_Item;
        private ItemInteraction m_ItemInteraction = ItemInteraction.INVENTORY;
        private EquipmentController m_PlayerEquipmentController;
        private HealthController m_PlayerHealthController;

        private void Start()
        {
            m_PlayerEquipmentController = PlayerController.Instance.equipmentController;
            m_PlayerHealthController = PlayerController.Instance.healthController;
            m_ThisEntry = GetComponentInParent<InventoryListEntry>();
        }

        public void SpawnInteractions(IItem item, ItemInteraction interaction)
        {
            HideInteractions();
            if (item == null)
            {
                return;
            }

            m_Item = item;
            m_ItemInteraction = interaction;
            SetButtonActions();
        }

        public void HideInteractions()
        {
            m_equipButton.gameObject.SetActive(false);
            m_takeOffButton.gameObject.SetActive(false);
            m_useButton.gameObject.SetActive(false);
            m_dropButton.gameObject.SetActive(false);
            m_ThisEntry.ToggleAmountPicker(false);
        }

        public void EquipButtonAction()
        {
            m_PlayerEquipmentController.EquipItem(m_Item);
            HideInteractions();
            CanvasController.Instance.inventoryInterface.UpdateEntirePlayerPanel();
        }

        public void TakeOffButtonAction()
        {
            m_PlayerEquipmentController.TakeOffItem(m_Item);
            HideInteractions();
            CanvasController.Instance.inventoryInterface.UpdateEntirePlayerPanel();
        }

        public void DropButtonAction()
        {
            if (m_Item is StackableItem && m_PlayerEquipmentController.GetAmount(m_Item) > 1)
            {
                m_ThisEntry.ToggleAmountPicker(true);
                m_ThisEntry.inventoryAmountPicker.action = DropItemAction;
            }
            else
            {
                DropItemAction();
            }
        }

        private void DropItemAction(int amount = 1)
        {
            m_PlayerEquipmentController.RemoveItem(m_Item, amount);
            HideInteractions();
            CanvasController.Instance.inventoryInterface.UpdateEntirePlayerPanel();
        }


        public void UseButtonAction()
        {
            UsableItem usableItem = (UsableItem)m_Item;
            PlayerController.Instance.playerControllerLocal.StartInteractionCoroutine(UseItemCoroutine(usableItem), true, usableItem.UseTime, usableItem.itemName);
        }

        private IEnumerator UseItemCoroutine(UsableItem usableItem)
        {
            yield return usableItem.Use(m_PlayerHealthController, m_PlayerEquipmentController);
            m_ThisEntry.HideInteractionPanel();
        }

        private void SetButtonActions()
        {
            if (m_Item is QuestItem)
            {
                return;
            }

            if (m_ItemInteraction == ItemInteraction.INVENTORY)
            {
                if (m_Item is ArmorInstance or WeaponInstance)
                {
                    m_equipButton.gameObject.SetActive(true);
                }

                if (m_Item is UsableItem)
                {
                    m_useButton.gameObject.SetActive(true);
                }
            }

            if (m_ItemInteraction == ItemInteraction.EQUIPPED)
            {
                if (m_Item is ArmorInstance or WeaponInstance)
                {
                    m_takeOffButton.gameObject.SetActive(true);
                }
            }

            m_dropButton.gameObject.SetActive(true);
        }
    }

    public enum ItemInteraction
    {
        INVENTORY,
        EQUIPPED,
    }
}