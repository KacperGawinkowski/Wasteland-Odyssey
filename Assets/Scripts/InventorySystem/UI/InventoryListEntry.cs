using System;
using Interactions;
using InventorySystem.Items;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace InventorySystem.UI
{
    public class InventoryListEntry : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        #region components

        [SerializeField] private GameObject m_Selection;
        [SerializeField] private TextMeshProUGUI m_ItemLvlText;
        [SerializeField] private TextMeshProUGUI m_ItemQuantityText;
        [SerializeField] private TextMeshProUGUI m_ItemPriceText;
        [SerializeField] private TextMeshProUGUI m_ItemNameText;
        [SerializeField] private Image m_ItemIcon;
        [SerializeField] private Image m_ItemEquipped;
        [SerializeField] private TextMeshProUGUI m_WeaponSlot;
        [SerializeField] private GameObject m_BuyButton;
        [SerializeField] private GameObject m_SellButton;

        [FormerlySerializedAs("interactionsInterface")]
        public InventoryListEntryInteractions inventoryListEntryInteractions;
        [FormerlySerializedAs("amountPickerInterface")]
        public InventoryAmountPicker inventoryAmountPicker;

        #endregion

        private static InventoryListEntryInteractions s_CurrentlyActiveInteractionsPanel;


        [NonSerialized] public InventoryEntryLocation location;
        [NonSerialized] public IItem item;
        [NonSerialized] public int itemQuantity;


        public void TransferItem()
        {
            if (item is StackableItem && itemQuantity > 5)
            {
                ToggleAmountPicker(!inventoryAmountPicker.gameObject.activeSelf);
                inventoryAmountPicker.action = TransferItem2;
            }
            else
            {
                TransferItem2(1);
            }
        }

        private void TransferItem2(int amount)
        {
            switch (location)
            {
                case InventoryEntryLocation.PlayerInventory:
                    CanvasController.Instance.inventoryInterface.SellItem(item, amount);
                    break;
                case InventoryEntryLocation.PlayerNewItems:
                    CanvasController.Instance.inventoryInterface.SellBackItem(item, amount);
                    break;
                case InventoryEntryLocation.TradeInventory:
                    CanvasController.Instance.inventoryInterface.BuyItem(item, amount);
                    break;
                case InventoryEntryLocation.TradeNewItems:
                    CanvasController.Instance.inventoryInterface.BuyBackItem(item, amount);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            ToggleAmountPicker(false);
        }

        public void SetData(IItem itemInstance, float priceModifier, int quantity, bool isEquipped, InventoryEntryArrowDirection arrowDirection, InventoryInterfaceType type, InventoryEntryLocation entryLocation)
        {
            item = itemInstance;
            m_ItemNameText.text = itemInstance.ItemName;
            itemQuantity = quantity;
            m_ItemQuantityText.text = quantity.ToString();
            location = entryLocation;

            if (itemInstance.Icon != null)
            {
                m_ItemIcon.sprite = itemInstance.Icon;
            }
            
            if (itemInstance is UniqueItemInstance uniqueInstance)
            {
                m_ItemLvlText.text = uniqueInstance.ItemLvl.ToString();
            }
            else
            {
                m_ItemLvlText.text = "";
            }

            m_ItemEquipped.gameObject.SetActive(isEquipped);
            if (isEquipped)
            {
                if (itemInstance is WeaponInstance weaponInstance)
                {
                    m_WeaponSlot.text = $"EQ{(weaponInstance.GetWeaponTypeInt() + 1)}";
                }
            }

            switch (arrowDirection)
            {
                case InventoryEntryArrowDirection.None:
                    m_BuyButton.SetActive(false);
                    m_SellButton.SetActive(false);
                    break;
                case InventoryEntryArrowDirection.Sell:
                    m_SellButton.SetActive(true);
                    m_BuyButton.SetActive(false);
                    break;
                case InventoryEntryArrowDirection.Buy:
                    m_SellButton.SetActive(false);
                    m_BuyButton.SetActive(true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(arrowDirection), arrowDirection, null);
            }

            if (type == InventoryInterfaceType.Trading)
            {
                m_ItemPriceText.text = entryLocation switch
                {
                    InventoryEntryLocation.PlayerInventory => $"{item.SellPrice * priceModifier}$",
                    InventoryEntryLocation.PlayerNewItems => $"{item.BuyPrice * priceModifier}$",
                    InventoryEntryLocation.TradeInventory => $"{item.BuyPrice * priceModifier}$",
                    InventoryEntryLocation.TradeNewItems => $"{item.SellPrice * priceModifier}$",
                    _ => throw new ArgumentOutOfRangeException(nameof(entryLocation), entryLocation, null)
                };
            }
            else
            {
                m_ItemPriceText.text = $"{item.SellPrice * priceModifier}$";
            }
            
            // switch (type)
            // {
            //     case InventoryInterfaceType.OnlyPlayerInventory:
            //         m_ItemPriceText.text = $"{item.SellPrice * priceModifier}$";
            //         //m_ItemPriceText.gameObject.SetActive(true);
            //         break;
            //     case InventoryInterfaceType.Looting:
            //         m_ItemPriceText.text = $"{item.SellPrice * priceModifier}$";
            //         //m_ItemPriceText.gameObject.SetActive(true);
            //         break;
            //     case InventoryInterfaceType.Trading:
            //         //m_ItemPriceText.gameObject.SetActive(true);
            //         m_ItemPriceText.text = entryLocation switch
            //         {
            //             InventoryEntryLocation.PlayerInventory => $"{item.SellPrice * priceModifier}$",
            //             InventoryEntryLocation.PlayerNewItems => $"{item.BuyPrice * priceModifier}$",
            //             InventoryEntryLocation.TradeInventory => $"{item.BuyPrice * priceModifier}$",
            //             InventoryEntryLocation.TradeNewItems => $"{item.SellPrice * priceModifier}$",
            //             _ => throw new ArgumentOutOfRangeException(nameof(entryLocation), entryLocation, null)
            //         };
            //         break;
            //     default:
            //         throw new ArgumentOutOfRangeException(nameof(type), type, null);
            // }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            m_Selection.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            m_Selection.SetActive(false);
        }

        private void OnDisable()
        {
            m_Selection.SetActive(false);
            HideInteractionPanel();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                Debug.Log("Show Item Description");
                CanvasController.Instance.inventoryInterface.itemDescriptionController.gameObject.SetActive(true);
                CanvasController.Instance.inventoryInterface.itemDescriptionController.ShowDescription(item, itemQuantity);
            }
            else if (inventoryListEntryInteractions != null && eventData.button == PointerEventData.InputButton.Right && CanvasController.Instance.inventoryInterface.currentInterfaceType == InventoryInterfaceType.OnlyPlayerInventory)
            {
                HideInteractionPanel();
                s_CurrentlyActiveInteractionsPanel = inventoryListEntryInteractions;
                s_CurrentlyActiveInteractionsPanel.gameObject.SetActive(true);

                if (m_ItemEquipped.IsActive())
                {
                    inventoryListEntryInteractions.SpawnInteractions(item, ItemInteraction.EQUIPPED);
                }
                else
                {
                    inventoryListEntryInteractions.SpawnInteractions(item, ItemInteraction.INVENTORY);
                }
            }
            else
            {
                HideInteractionPanel();
            }
        }

        public void HideInteractionPanel()
        {
            if (inventoryListEntryInteractions != null)
            {
                if (s_CurrentlyActiveInteractionsPanel != null)
                {
                    s_CurrentlyActiveInteractionsPanel.HideInteractions();
                    s_CurrentlyActiveInteractionsPanel.gameObject.SetActive(false);
                }

                inventoryListEntryInteractions.HideInteractions();
            }
        }

        public void ToggleAmountPicker(bool value)
        {
            inventoryAmountPicker.ToggleAmountPicker(value);
        }
    }
}