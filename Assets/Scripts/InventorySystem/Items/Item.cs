using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.Serialization;

namespace InventorySystem.Items
{
    public interface IItem
    {
        public string ItemName { get; }
        public string Description { get; }
        public int Weight { get; }
        public int SellPrice { get; }

        public int BuyPrice { get; }
        public Sprite Icon { get; }
    }

    public abstract class BaseItem : ScriptableObject
    {
        [Header("Core")]
        public string itemName;
        [TextArea(1, 5)] public string description;
        public int weight;
        public float occurChance = 0.3f;

        [Header("Prices")]
        public int sellPrice;
        public int buyPrice;

        [Header("Graphics")]
        public Sprite itemIcon;
    }

    public abstract class StackableItem : BaseItem, IItem
    {
        public string ItemName => itemName;
        public int Weight => weight;
        public string Description => description;
        public int SellPrice => sellPrice;
        public int BuyPrice => buyPrice;
        public Sprite Icon => itemIcon;
    }

    public abstract class UniqueItem : BaseItem
    {
        public const int s_MaxItemLevel = 111;
        public abstract UniqueItemInstance CreateInstance(int itemLvl);
    }

    [Serializable]
    public abstract class UniqueItemInstance : IItem
    {
        [NonSerialized] public UniqueItem item;

        public string ItemName => item.itemName;
        public int Weight => item.weight;
        public string Description => item.description;
        public int SellPrice => sellPrice; //item.sellPrice;
        public int BuyPrice => buyPrice; //item.buyPrice;
        public Sprite Icon => item.itemIcon;


        public int ItemLvl => itemLvl;
        [SerializeField] protected int itemLvl;
        protected int sellPrice;
        protected int buyPrice;

        public abstract void SetItemLvl(int iLvl);
        public abstract int GetUpgradeCost();

        protected int GetSellPrice(int iLvl)
        {
            return (int)((0.009 * item.sellPrice) * iLvl * iLvl + item.sellPrice);
        }
    }
}