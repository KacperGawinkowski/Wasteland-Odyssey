using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace InventorySystem.Items
{
    [CreateAssetMenu(fileName = nameof(Armor), menuName = "InventorySystem/Items/" + nameof(Armor))]
    public class Armor : UniqueItem
    {
        [Header("Armor Options")]
        public ArmorSlot armorSlot;
        public int baseArmorPoints;
        public SkinnedMeshRenderer skinnedMeshRenderer;
        public MeshFilter meshFilter;
        public MeshRenderer meshRenderer;

        public override UniqueItemInstance CreateInstance(int itemLvl)
        {
            ArmorInstance instance = new();
            instance.item = this;
            instance.SetItemLvl(itemLvl);
            return instance;
        }
    }

    [System.Serializable]
    public class ArmorInstance : UniqueItemInstance
    {
        public int armorPoints;

        public override void SetItemLvl(int iLvl)
        {
            if (iLvl > UniqueItem.s_MaxItemLevel)
            {
                Debug.LogError($"Cannot upgrade item above {UniqueItem.s_MaxItemLevel}");
                return;
            }
            
            itemLvl = iLvl;
            armorPoints = (int)(GetArmor().baseArmorPoints + iLvl * 0.72f);
            
            sellPrice = GetSellPrice(iLvl);
            buyPrice = sellPrice * 2;
        }

        public override int GetUpgradeCost()
        {
            return 10 + Math.Abs(GetSellPrice(itemLvl) - GetSellPrice(itemLvl+1));
        }

        public int GetArmorTypeInt()
        {
            return (int)((Armor)item).armorSlot;
        }

        public Armor GetArmor()
        {
            return (Armor)item;
        }
    }

    public enum ArmorSlot
    {
        HEAD,
        TORSO,
        LEGS,
    }
}