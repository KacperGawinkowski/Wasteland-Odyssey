using System;
using System.Collections.Generic;
using InventorySystem.Items;
using InventorySystem.UI;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

namespace InventorySystem
{
    public class InventoryCanvasController : MonoBehaviour
    {
        //[SerializeField] private InventoryListEntry m_ListEntryPrefab;
        [SerializeField] private InventoryListEntry m_GigaEntryPrefab;

        public ScrollRect scrollRect;
        [SerializeField] private Transform m_PlayerListEntryContainer;
        [SerializeField] private GameObject m_TradePanel;
        [SerializeField] private TextMeshProUGUI m_TradePanelLabel;
        [SerializeField] private TextMeshProUGUI m_ApplyButton;
        [SerializeField] private Transform m_TradeListEntryContainer;
        [SerializeField] private GameObject m_InventoryActionButtons;
        [SerializeField] private GameObject m_TradeInformationPanel;
        [SerializeField] private TextMeshProUGUI m_WeightCounter;
        [SerializeField] private TextMeshProUGUI m_MoneyCounter;

        [SerializeField] private TextMeshProUGUI m_WeightDeltaCounter;
        [SerializeField] private TextMeshProUGUI m_MoneyDeltaCounter;
        [SerializeField] private GameObject m_MoneyDeltaObject;

        [FormerlySerializedAs("itemDescriptionInterface")]
        public ItemDescriptionController itemDescriptionController;

        [SerializeField] private EquipmentController m_PlayerEquipment;
        private BasicInventory m_CurrentTradeInventory;

        // player panel
        private readonly InventoryDictionary m_TempPlayerInventory = new(); //temp inv sprzedazy 
        private readonly InventoryDictionary m_TempPlayerNewItems = new(); //temp inv sprzedazy 
        private readonly List<InventoryListEntry> m_PlayerListEntries = new();

        // trade panel
        private readonly InventoryDictionary m_TempTradeInventory = new(); //temp inv kupna 
        private readonly InventoryDictionary m_TempTradeNewItems = new(); //temp inv kupna 
        private readonly List<InventoryListEntry> m_TradeListEntries = new();


        [NonSerialized] public InventoryInterfaceType currentInterfaceType;

        private int tempWeightSum;

        private int tempCurrencySum;

        #region trade buttons

        public void AcceptTrade()
        {
            if (currentInterfaceType == InventoryInterfaceType.Trading && tempCurrencySum < 0) return;

            m_CurrentTradeInventory.InventoryDictionary.Clear();
            JoinInventories(m_CurrentTradeInventory.InventoryDictionary, m_TempTradeInventory);
            JoinInventories(m_CurrentTradeInventory.InventoryDictionary, m_TempTradeNewItems);

            m_PlayerEquipment.InventoryDictionary.Clear();
            JoinInventories(m_PlayerEquipment.InventoryDictionary, m_TempPlayerInventory);
            JoinInventories(m_PlayerEquipment.InventoryDictionary, m_TempPlayerNewItems);

            m_PlayerEquipment.CheckAllEquippedItems();
            if (m_CurrentTradeInventory is EquipmentController equipmentController)
            {
                equipmentController.CheckAllEquippedItems();
            }

            switch (currentInterfaceType)
            {
                case InventoryInterfaceType.OnlyPlayerInventory:
                    break;
                case InventoryInterfaceType.Looting:
                    m_PlayerEquipment.currentWeight = tempWeightSum;
                    break;
                case InventoryInterfaceType.Trading:
                    m_PlayerEquipment.currentWeight = tempWeightSum;
                    m_PlayerEquipment.SetMoney(tempCurrencySum);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (PlayerController.Instance.playerControllerGlobal.currentLocation is Quest quest)
            {
                if (quest is LootQuest lootQuest)
                {
                    quest.CheckIfQuestCompleted();
                    m_PlayerEquipment.RemoveItem((QuestItem)ItemIndex.GetById(lootQuest.questItemId), 1);
                }
            }
        }

        public void ResetTrade()
        {
            CloneInventory(m_TempPlayerInventory, m_PlayerEquipment.InventoryDictionary);
            m_TempPlayerNewItems.Clear();

            CloneInventory(m_TempTradeInventory, m_CurrentTradeInventory.InventoryDictionary);
            m_TempTradeNewItems.Clear();

            tempWeightSum = m_PlayerEquipment.currentWeight;
            tempCurrencySum = m_PlayerEquipment.GetMoney(); //(???)

            UpdateEntirePlayerPanel();
            UpdateEntireTradePanel();
        }

        public void BuyItem(IItem item, int amount)
        {
            int v = m_TempTradeInventory.RemoveItem(item, amount);
            m_TempPlayerNewItems.AddItem(item, v);

            tempWeightSum += item.Weight * amount;
            tempCurrencySum -= item.BuyPrice * amount;

            UpdateEntirePlayerPanel();
            UpdateEntireTradePanel();
        }

        public void SellBackItem(IItem item, int amount)
        {
            int v = m_TempPlayerNewItems.RemoveItem(item, amount);
            m_TempTradeInventory.AddItem(item, v);

            tempWeightSum -= item.Weight * amount;
            tempCurrencySum += item.BuyPrice * amount;

            UpdateEntirePlayerPanel();
            UpdateEntireTradePanel();
        }

        public void SellItem(IItem item, int amount)
        {
            int v = m_TempPlayerInventory.RemoveItem(item, amount);
            m_TempTradeNewItems.AddItem(item, v);

            tempWeightSum -= item.Weight * amount;
            tempCurrencySum += item.SellPrice * amount;

            UpdateEntirePlayerPanel();
            UpdateEntireTradePanel();
        }

        public void BuyBackItem(IItem item, int amount)
        {
            int v = m_TempTradeNewItems.RemoveItem(item, amount);
            m_TempPlayerInventory.AddItem(item, v);

            tempWeightSum += item.Weight * amount;
            tempCurrencySum -= item.SellPrice * amount;

            UpdateEntirePlayerPanel();
            UpdateEntireTradePanel();
        }

        #endregion

        private void ConfigureTradeListEntries(InventoryInterfaceType type)
        {
            int uniqueItemCount = m_TempTradeInventory.UniqueItemCount() + m_TempTradeNewItems.UniqueItemCount();
            ExpandEntriesList(m_TradeListEntries, m_TradeListEntryContainer, uniqueItemCount);

            int index = 0;
            foreach (var item in m_TempTradeInventory)
            {
                if (!(item.Key is Currency && type == InventoryInterfaceType.Looting))
                {
                    bool isEquipped = m_CurrentTradeInventory is EquipmentController equipmentController && equipmentController.CheckIfItemIsEquipped(item.Key);
                    m_TradeListEntries[index].gameObject.SetActive(true);
                    m_TradeListEntries[index].SetData(item.Key, /*priceModifier*/1, item.Value, isEquipped, InventoryEntryArrowDirection.Buy, type, InventoryEntryLocation.TradeInventory);
                    index++;
                }
            }

            foreach (var item in m_TempTradeNewItems)
            {
                m_TradeListEntries[index].gameObject.SetActive(true);
                m_TradeListEntries[index].SetData(item.Key, /*priceModifier*/1, item.Value, false, InventoryEntryArrowDirection.Buy, type, InventoryEntryLocation.TradeNewItems);
                index++;
            }

            for (int i = index; i < m_TradeListEntries.Count; i++)
            {
                m_TradeListEntries[i].gameObject.SetActive(false);
            }

            if (type == InventoryInterfaceType.Trading)
            {
                m_ApplyButton.transform.parent.GetComponent<Button>().interactable = (m_PlayerEquipment.GetMoney()-tempCurrencySum <= m_PlayerEquipment.GetMoney());
            }
        }

        private void ConfigurePlayerListEntries(InventoryInterfaceType type)
        {
            if (type == InventoryInterfaceType.OnlyPlayerInventory)
            {
                int uniqueItemCount = m_PlayerEquipment.UniqueItemCount();
                ExpandEntriesList(m_PlayerListEntries, m_PlayerListEntryContainer, uniqueItemCount);

                int index = 0;
                foreach (var item in m_PlayerEquipment.InventoryDictionary)
                {
                    if (!(item.Key is Currency))
                    {
                        bool isEquipped = m_PlayerEquipment.CheckIfItemIsEquipped(item.Key);
                        m_PlayerListEntries[index].gameObject.SetActive(true);
                        m_PlayerListEntries[index].SetData(item.Key, /*priceModifier*/1, item.Value, isEquipped,
                            InventoryEntryArrowDirection.None, type, InventoryEntryLocation.PlayerInventory);
                        index++;
                    }
                }

                for (int i = index; i < m_PlayerListEntries.Count; i++)
                {
                    m_PlayerListEntries[i].gameObject.SetActive(false);
                }
            }
            else
            {
                int uniqueItemCount = m_TempPlayerInventory.UniqueItemCount() + m_TempPlayerNewItems.UniqueItemCount();
                ExpandEntriesList(m_PlayerListEntries, m_PlayerListEntryContainer, uniqueItemCount);

                int index = 0;
                foreach (var item in m_TempPlayerInventory)
                {
                    if (!(item.Key is Currency))
                    {
                        bool isEquipped = m_PlayerEquipment.CheckIfItemIsEquipped(item.Key);
                        m_PlayerListEntries[index].gameObject.SetActive(true);
                        m_PlayerListEntries[index].SetData(item.Key, /*priceModifier*/1, item.Value, isEquipped,
                            InventoryEntryArrowDirection.Sell, type, InventoryEntryLocation.PlayerInventory);
                        index++;
                    }
                }

                foreach (var item in m_TempPlayerNewItems)
                {
                    m_PlayerListEntries[index].gameObject.SetActive(true);
                    m_PlayerListEntries[index].SetData(item.Key, /*priceModifier*/1, item.Value, false,
                        InventoryEntryArrowDirection.Sell, type, InventoryEntryLocation.PlayerNewItems);
                    index++;
                }

                for (int i = index; i < m_PlayerListEntries.Count; i++)
                {
                    m_PlayerListEntries[i].gameObject.SetActive(false);
                }
            }
        }

        private void ExpandEntriesList(List<InventoryListEntry> listEntries, Transform entryRoot, int count)
        {
            if (listEntries.Count < count)
            {
                int r = count - listEntries.Count;
                for (int i = 0; i < r; i++)
                {
                    InventoryListEntry listEntry = Instantiate(m_GigaEntryPrefab, entryRoot);
                    listEntries.Add(listEntry);
                }
            }
        }

        private static void CloneInventory(InventoryDictionary target, InventoryDictionary source)
        {
            target.Clear();
            JoinInventories(target, source);
        }

        private static void JoinInventories(InventoryDictionary target, InventoryDictionary source)
        {
            foreach (KeyValuePair<IItem, int> item in source)
            {
                target.AddItem(item.Key, item.Value);
            }
        }


        #region inventory panel controlls

        public void UpdateEntirePlayerPanel()
        {
            ConfigurePlayerListEntries(currentInterfaceType);
            DisplayWeightCurrencyChanges();
        }

        public void UpdateEntireTradePanel()
        {
            ConfigureTradeListEntries(currentInterfaceType);
        }

        private void DisplayWeightCurrencyChanges()
        {
            m_MoneyCounter.text = $"{m_PlayerEquipment.GetMoney()}";;
            if (currentInterfaceType != InventoryInterfaceType.OnlyPlayerInventory)
            {
                SetWeightTextColor(m_WeightCounter, tempWeightSum, m_PlayerEquipment.maxWeight, Color.white, Color.red);
                m_WeightCounter.text = $"{tempWeightSum}/{m_PlayerEquipment.maxWeight}";

                SetWeightTextColor(m_WeightDeltaCounter, tempWeightSum, m_PlayerEquipment.maxWeight, Color.white, Color.red);
                m_WeightDeltaCounter.text = (tempWeightSum - m_PlayerEquipment.currentWeight).ToString();

                if (currentInterfaceType == InventoryInterfaceType.Trading)
                {
                    m_MoneyDeltaObject.SetActive(true);
                    m_MoneyDeltaCounter.gameObject.SetActive(true);
                    m_MoneyDeltaCounter.text = (tempCurrencySum - m_PlayerEquipment.GetMoney()).ToString();
                    
                    SetWeightTextColor(m_MoneyDeltaCounter, m_PlayerEquipment.GetMoney()-tempCurrencySum,m_PlayerEquipment.GetMoney() , Color.white,
                        Color.red);
                }
                else
                {
                    m_MoneyDeltaCounter.gameObject.SetActive(false);
                    m_MoneyDeltaObject.SetActive(false);
                }
            }
            else
            {
                SetWeightTextColor(m_WeightCounter, m_PlayerEquipment.currentWeight, m_PlayerEquipment.maxWeight, Color.white,
                    Color.red);
                m_WeightCounter.text = $"{m_PlayerEquipment.currentWeight}/{m_PlayerEquipment.maxWeight}";
            }
        }

        private void SetWeightTextColor(TextMeshProUGUI text, int curWeight, int maxWeight, Color underColor, Color overColor)
        {
            if (curWeight > maxWeight)
            {
                text.color = overColor;
            }
            else
            {
                text.color = underColor;
            }
        }

        public void CreateTradePanel(BasicInventory tradeInventory, InventoryInterfaceType interfaceType,
            string tradeInventoryLabel)
        {
            m_CurrentTradeInventory = tradeInventory;
            currentInterfaceType = interfaceType;

            switch (interfaceType)
            {
                case InventoryInterfaceType.OnlyPlayerInventory:
                    break;
                case InventoryInterfaceType.Looting:
                    tempWeightSum = m_PlayerEquipment.currentWeight;
                    m_ApplyButton.text = "Loot";
                    break;
                case InventoryInterfaceType.Trading:
                    tempWeightSum = m_PlayerEquipment.currentWeight;
                    tempCurrencySum = m_PlayerEquipment.GetMoney();
                    m_ApplyButton.text = "Trade";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(interfaceType), interfaceType, null);
            }

            ResetTrade();

            m_TradePanel.SetActive(true);

            m_InventoryActionButtons.SetActive(true);
            m_TradeInformationPanel.SetActive(interfaceType is InventoryInterfaceType.Trading
                or InventoryInterfaceType.Looting);
            m_TradePanelLabel.text = tradeInventoryLabel;
        }

        public void CreatePlayerInventoryPanel()
        {
            currentInterfaceType = InventoryInterfaceType.OnlyPlayerInventory;

            UpdateEntirePlayerPanel();

            m_TradePanel.SetActive(false);
            m_InventoryActionButtons.SetActive(false);
            m_TradeInformationPanel.SetActive(false);
        }

        #endregion


        private void OnEnable()
        {
            if (!CanvasController.Instance.m_OpenedInterfaces.Contains(gameObject))
            {
                CanvasController.Instance.m_OpenedInterfaces.Push(gameObject);
            }
        }

        //Nwm czy to ma sens, ale dodaje metode ktora schowa interakcje z przedmiotami oraz opis przedmiotu w momencie wylaczenia interface ekwipunku mhm
        private void OnDisable()
        {
            itemDescriptionController.gameObject.SetActive(false);
            if (currentInterfaceType == InventoryInterfaceType.Trading)
            {
                CanvasController.Instance.dialogueCanvasController.gameObject.SetActive(true);
            }
        }
    }
}