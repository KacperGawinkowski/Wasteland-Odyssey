using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using InventorySystem.Items;
using UnityEngine;
using Random = UnityEngine.Random;

public class LootGeneratorController : MonoBehaviour
{
    [SerializeField] private LootGenerator[] m_Chests;
    private List<LootGenerator> m_ActiveChests;

    public void GenerateLoots(int lootValue)
    {
        m_ActiveChests = new List<LootGenerator>();
        int activeChests = 0;
        foreach (LootGenerator chest in m_Chests)
        {
            chest.ChestRandomizeEnabled();
            if (chest.gameObject.activeSelf)
            {
                m_ActiveChests.Add(chest);
                activeChests++;
            }
        }

        if (activeChests > 0)
        {
            int valueInOneChest = lootValue / activeChests;
            foreach (LootGenerator activeChest in m_ActiveChests)
            {
                activeChest.GenerateLoot(valueInOneChest);
            }
        }
        else
        {
            int chestToForceSpawnIndex = Random.Range(0, m_Chests.Length);
            m_ActiveChests.Add(m_Chests[chestToForceSpawnIndex]);
            m_Chests[chestToForceSpawnIndex].ForceSpawnChestWithLoot(lootValue);
        }
    }

    public void PutItemInRandomActiveChest(IItem item)
    {
        int randomActiveChestIndex = Random.Range(0, m_ActiveChests.Count);
        m_ActiveChests[randomActiveChestIndex].AddItem(item, 1);
    }

    [ContextMenu("LoadChests")]
    private void LoadChests()
    {
        m_Chests = GetComponentsInChildren<LootGenerator>();
    }

    private void OnValidate()
    {
        LoadChests();
    }
}