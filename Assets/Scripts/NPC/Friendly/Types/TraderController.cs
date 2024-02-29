using System;
using System.Collections.Generic;
using System.Linq;
using InventorySystem;
using InventorySystem.Items;
using NPC.Friendly;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using World;
using Random = UnityEngine.Random;

namespace NPC
{
	public class TraderController : BasicInventory, INpcDialogueAction, ISaveData<TraderData>
	{
		private TraderData m_TraderData;

		public TraderType traderType;
		public int numberOfItemsToSell = 20;
		public float percentageOfFullyRandomItems = 0.25f;

		private void Start()
		{
			SaveSystem.OnUpdateSaveContent += UpdateSave;
		}

		public void PrepareItemsToSell(TraderType traderType)
		{
			InventoryDictionary ??= new InventoryDictionary();

			if (!(InventoryDictionary.UniqueItemCount() == 0 || m_TraderData.onWhichDayWasEquipmentGenerated < TimeController.Instance.dayCounter))
			{
				return;
			}

			if (m_TraderData != null)
			{
				m_TraderData.onWhichDayWasEquipmentGenerated = TimeController.Instance.dayCounter;
			}

			this.traderType = traderType;

			InventoryDictionary.Clear();

			switch (this.traderType)
			{
				case TraderType.Random:
					AddRandomItemsFromLootPreset(ItemIndex.GetAllItems(), numberOfItemsToSell, 15);
					break;
				case TraderType.Trash:
					AddRandomItemsFromLootPreset(ItemIndex.GetByType<Trash>(), (int)(numberOfItemsToSell * (1 - percentageOfFullyRandomItems)), 30);
					AddRandomItemsFromLootPreset(ItemIndex.GetAllItems(), (int)(numberOfItemsToSell * (percentageOfFullyRandomItems)), 15);
					break;
				case TraderType.Armor:
					AddRandomItemsFromLootPreset(ItemIndex.GetByType<Armor>(), (int)(numberOfItemsToSell * (1 - percentageOfFullyRandomItems)));
					AddRandomItemsFromLootPreset(ItemIndex.GetAllItems(), (int)(numberOfItemsToSell * (percentageOfFullyRandomItems)), 15);
					break;
				case TraderType.Weapon:
					AddRandomItemsFromLootPreset(ItemIndex.GetByType<Weapon>(), (int)(numberOfItemsToSell * (1 - percentageOfFullyRandomItems)));
					AddRandomItemsFromLootPreset(ItemIndex.GetAllItems(), (int)(numberOfItemsToSell * (percentageOfFullyRandomItems)), 15);
					break;
				case TraderType.Ammo:
					AddRandomItemsFromLootPreset(ItemIndex.GetByType<Ammunition>(), (int)(numberOfItemsToSell * (1 - percentageOfFullyRandomItems)), 120);
					AddRandomItemsFromLootPreset(ItemIndex.GetAllItems(), (int)(numberOfItemsToSell * (percentageOfFullyRandomItems)), 15);
					break;
				case TraderType.Medicaments:
					AddRandomItemsFromLootPreset(ItemIndex.GetByType<Medicament>(), (int)(numberOfItemsToSell * (1 - percentageOfFullyRandomItems)), 30);
					AddRandomItemsFromLootPreset(ItemIndex.GetAllItems(), (int)(numberOfItemsToSell * (percentageOfFullyRandomItems)), 15);
					break;
			}

		}

		private void AddRandomItemsFromLootPreset(BaseItem[] items, int numberOfItems, int maxAmountOfTheSameItem = 1)
		{
			List<BaseItem> result = items.ToList().OrderBy(arg => Guid.NewGuid()).Take(numberOfItems).Where(x => x.GetType() != typeof(QuestItem) && x.GetType() != typeof(Currency)).ToList();
			foreach (BaseItem item in result)
			{
				if (item is UniqueItem uniqueItem)
				{
					int ilvl = Random.Range(1 + 2 * SaveSystem.saveContent.questLog.questsFinished + 5 * SaveSystem.saveContent.questLog.storylineQuestsFinished, 10 + 3 * SaveSystem.saveContent.questLog.questsFinished + 10 * SaveSystem.saveContent.questLog.storylineQuestsFinished);
					ilvl = Math.Clamp(ilvl,1, Constants.s_MaxItemLevel);
					UniqueItemInstance itemInstance = uniqueItem.CreateInstance(ilvl);
					
					InventoryDictionary.AddItem(itemInstance, 1);
				}
				else
				{
					InventoryDictionary.AddItem((IItem)item, Random.Range(1, maxAmountOfTheSameItem));
				}
			}
		}

		public void AddDialogueActions()
		{
			// CanvasController.Instance.dialogueCanvasController.SetTradeOption(this);
			
			DialogueAction dialogueAction = new()
			{
				action = () =>
				{
					CanvasController.Instance.inventoryPanel.SetActive(true);
					CanvasController.Instance.questChoosePanel.SetActive(false);
					CanvasController.Instance.upgradePanel.SetActive(false);
					CanvasController.Instance.inventoryInterface.CreateTradePanel(this, InventoryInterfaceType.Trading, "Trade");
					CanvasController.Instance.m_DialoguePanel.SetActive(false);
				},
				interactable = () => true,
				name = "Trade"
			};

			CanvasController.Instance.dialogueCanvasController.AddButton(dialogueAction);
		}

		public void SetData(TraderData data)
		{
			m_TraderData = data;

			traderType = data.traderType;

			InventoryDictionary = new InventoryDictionary();

			if (data.itemSaveData != null)
			{
				foreach (ItemSaveData item in data.itemSaveData)
				{
					BaseItem baseItem = ItemIndex.GetById(item.itemId);

					if (baseItem is StackableItem stackableItem)
					{
						InventoryDictionary.AddItem(stackableItem, item.count);
					}

					if (baseItem is UniqueItem uniqueItem)
					{
						for (int i = 0; i < item.count; i++)
						{
							item.uniqueItemInstance.item = uniqueItem;
							InventoryDictionary.AddItem(item.uniqueItemInstance, 1);
						}
					}
				}
			}
		}

		private void OnDestroy()
		{
			UpdateSave();
			SaveSystem.OnUpdateSaveContent -= UpdateSave;
		}

		public TraderData GetData()
		{
			TraderData traderData = new()
			{
				traderType = traderType,
				itemSaveData = new ItemSaveData[InventoryDictionary.UniqueItemCount()]
			};

			int i = 0;
			foreach (KeyValuePair<IItem, int> item in InventoryDictionary)
			{
				traderData.itemSaveData[i] = new ItemSaveData
				{
					itemId = item.Key.ItemName,
					count = item.Value,
					uniqueItemInstance = (item.Key is UniqueItemInstance ii ? ii : null)
				};
				i++;
			}
			return traderData;
		}

		private void UpdateSave()
		{
			if (m_TraderData == null)
			{
				return;
			}

			m_TraderData.itemSaveData = new ItemSaveData[InventoryDictionary.UniqueItemCount()];

			int i = 0;
			foreach (KeyValuePair<IItem, int> item in InventoryDictionary)
			{
				m_TraderData.itemSaveData[i] = new ItemSaveData
				{
					itemId = item.Key.ItemName,
					count = item.Value,
					uniqueItemInstance = (item.Key is UniqueItemInstance ii ? ii : null)
				};
				i++;
			}
		}
	}

	[Serializable]
	public class TraderData
	{
		public ItemSaveData[] itemSaveData;
		public TraderType traderType;
		public int onWhichDayWasEquipmentGenerated;
	}


	public enum TraderType : byte
	{
		Random,
		Trash,
		Weapon,
		Armor,
		Ammo,
		Medicaments
	}
}