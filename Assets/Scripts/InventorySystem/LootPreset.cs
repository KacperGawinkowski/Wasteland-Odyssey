using System.Collections;
using System.Collections.Generic;
using InventorySystem.Items;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(LootPreset), menuName = "InventorySystem/LootPreset/" + nameof(LootPreset))]
public class LootPreset : ScriptableObject
{
    public BaseItem[] items;
}
