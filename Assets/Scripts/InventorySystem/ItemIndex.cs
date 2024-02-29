using System;
using InventorySystem.Items;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public static class ItemIndex
{
    // item id, item
    private static Dictionary<string, BaseItem> s_ItemIndex = new();
    private static Dictionary<Type, List<BaseItem>> s_IndexByType = new();

    private static Dictionary<ArmorSlot, List<Armor>> s_ArmorsBySlot = new();

    private static bool s_IsInitialized;

    private static BaseItem[] s_AllItems;

    public static Currency currencyItem;

    public static void Initialize()
    {
        s_AllItems = Resources.LoadAll<BaseItem>("Items");
        s_ItemIndex.Clear();
        s_IndexByType.Clear();

        foreach (BaseItem item in s_AllItems)
        {
            s_ItemIndex[item.itemName] = item;

            if (!s_IndexByType.ContainsKey(item.GetType()))
            {
                s_IndexByType[item.GetType()] = new List<BaseItem>();
            }

            s_IndexByType[item.GetType()].Add(item);

            if (item is Armor armor)
            {
                if (!s_ArmorsBySlot.ContainsKey(armor.armorSlot))
                {
                    s_ArmorsBySlot[armor.armorSlot] = new List<Armor>();
                }

                s_ArmorsBySlot[armor.armorSlot].Add(armor);
            }

            if (item is Currency currency)
            {
                currencyItem = currency;
            }
        }

        s_IsInitialized = true;
    }

    public static BaseItem GetById(string id)
    {
        if (!s_IsInitialized) Initialize();

        return s_ItemIndex[id];
    }

    public static BaseItem[] GetByType<T>() where T : BaseItem
    {
        if (!s_IsInitialized) Initialize();
        
        return s_IndexByType[typeof(T)].ToArray();
    }

    public static List<Armor> GetArmorBySlot(ArmorSlot slot)
    {
        return s_ArmorsBySlot[slot];
    }

    public static BaseItem[] GetAllItems()
    {
        return s_AllItems;
    }
}