using System;
using System.Linq;
using InventorySystem;
using InventorySystem.Items;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(BasicInventory))]
public class LootGenerator : MonoBehaviour
{
    [SerializeField] private LootPreset m_LootPreset;
    [SerializeField] private BasicInventory m_Inventory;
    [Range(0.0f, 1f)]
    [SerializeField] private float m_ChanceToAppear;

    public void ChestRandomizeEnabled()
    {
        float randomNumber = Random.Range(0f, 1f);
        if (randomNumber > m_ChanceToAppear)
        {
            gameObject.SetActive(false);
        }

        if (!m_LootPreset)
        {
            Debug.LogError("No loot preset!");
        }
    }

    public void ForceSpawnChestWithLoot(int lootValue)
    {
        gameObject.SetActive(true);
        GenerateLoot(lootValue);
    }
    
    public void GenerateLoot(int lootValue)
    {
        BaseItem[] items = m_LootPreset.items.ToArray();
        for (int i = 0; i < items.Length; i++)
        {
            int newIndex = Random.Range(0, items.Length);
            (items[i], items[newIndex]) = (items[newIndex], items[i]);
        }

        Debug.Log(gameObject.name);
        while (true)
        {
            foreach (BaseItem item in m_LootPreset.items)
            {
                float rand = Random.value;
                // Debug.Log($"{item.name} - {rand}");
                if (rand < item.occurChance)
                {
                    int amount;
                    switch (item)
                    {
                        case Ammunition:
                            amount = Random.Range(3, 99);
                            break;
                        case Currency:
                            amount = Random.Range(1, lootValue);
                            break;
                        default:
                            amount = 1;
                            break;
                    }

                    AddItem(item, amount);
                    lootValue -= item.buyPrice * amount;
                    //Debug.Log(lootValue);
                    if (lootValue <= 0)
                    {
                        return;
                    }
                }
            }
        }
    }

    public void AddItem(BaseItem item, int amount)
    {
        switch (item)
        {
            case UniqueItem uniqueItem:
                int ilvl = Random.Range(1 + 2 * SaveSystem.saveContent.questLog.questsFinished + 6 * SaveSystem.saveContent.questLog.storylineQuestsFinished, 10 + 3 * SaveSystem.saveContent.questLog.questsFinished + 10 * SaveSystem.saveContent.questLog.storylineQuestsFinished);
                ilvl = Math.Clamp(ilvl,1, Constants.s_MaxItemLevel);
                m_Inventory.AddItem(uniqueItem.CreateInstance(ilvl), amount);
                break;
            case StackableItem stackableItem:
                m_Inventory.AddItem(stackableItem, amount);
                break;
            default:
                throw new Exception("The item is sus");
        }
    }

    public void AddItem(IItem item, int amount)
    {
        m_Inventory.AddItem(item, amount);
    }

    public int GetTotalLootValue()
    {
        int value = 0;
        foreach (var item in m_Inventory.InventoryDictionary)
        {
            value += item.Key.BuyPrice * item.Value;
        }

        return value;
    }

    private void OnValidate()
    {
        m_Inventory = GetComponent<BasicInventory>();
    }
}