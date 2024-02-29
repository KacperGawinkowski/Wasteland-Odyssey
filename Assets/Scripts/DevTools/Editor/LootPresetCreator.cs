using System;
using System.Collections.Generic;
using System.Linq;
using InventorySystem.Items;
using Skills;
using UnityEditor;
using UnityEngine;

public static class LootPresetCreator
{
    private static string nameBeginning = "LP_";
    private static string lootPresetsPath = "Assets/Resources/LootPresets/";
    
    [MenuItem("Tools/Loot Preset Merge")]
    public static void MergeLootPresets()
    {
        HashSet<BaseItem> allItems = new HashSet<BaseItem>();
        
        List<LootPreset> presets = Resources.LoadAll<LootPreset>("LootPresets").ToList();

        LootPreset allItemsPreset = presets.Find(x => x.name == "LP_AllItems");

        foreach (LootPreset lootPreset in presets)
        {
            foreach (BaseItem item in lootPreset.items)
            {
                allItems.Add(item);
            }
        }
        
        if (allItemsPreset == null || allItemsPreset.items.Length == 0)
        {
            Debug.Log("Creating new LP_AllItems!");
            CreateNewLootPreset("AllItems", allItems.ToArray());
        }
        else
        {
            presets.Remove(allItemsPreset);
            allItemsPreset.items = allItems.ToArray();
        }

        AssetDatabase.SaveAssets();
    }

    [MenuItem("Tools/Create Loot Presets")]
    public static void CreateLootPresets()
    {
        HashSet<Type> distinctItemCategories = new HashSet<Type>();
        Dictionary<Type, BaseItem[]> categorizedItems = new Dictionary<Type, BaseItem[]>();

        foreach (BaseItem item in Resources.LoadAll<BaseItem>("Items"))
        {
            distinctItemCategories.Add(item.GetType());
        }

        foreach (Type category in distinctItemCategories)
        {
            if (category == typeof(QuestItem))
            {
                continue;
            }
            BaseItem[] tempItems = Resources.LoadAll<BaseItem>("Items");
            List<BaseItem> tempCategorizedItems = new List<BaseItem>();
            for (int i = 0; i < tempItems.Length; i++)
            {
                if (tempItems[i].GetType() == category)
                {
                    tempCategorizedItems.Add(tempItems[i]);
                }
            }

            categorizedItems[category] = tempCategorizedItems.ToArray();
        }

        foreach (var items in categorizedItems)
        {
            CreateNewLootPreset(items.Key.Name,items.Value);
        }

        MergeLootPresets();
        AssetDatabase.SaveAssets();
    }

    private static void CreateNewLootPreset(string name, BaseItem[] items)
    {
        LootPreset preset = ScriptableObject.CreateInstance<LootPreset>();
        preset.name = name;
        preset.items = items;
            
        AssetDatabase.CreateAsset(preset,lootPresetsPath + nameBeginning + name + ".asset");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
