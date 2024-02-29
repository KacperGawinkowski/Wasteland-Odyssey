using System;
using HealthSystem;
using UnityEngine;

[Serializable]
public class SaveContent
{
    public CharacterHealthSkeleton<int> playerHp;
    public EquipmentSaveData playerInventory;

    public VillageData[] villageDatas;

    public GlobalMap globalMap;
    public float perlinX;
    public float perlinY;

    public Vector2Int? playerGlobalMapPosition;

    public int? dayCounter;
    public float? dayTime;

    public QuestLog questLog = new();

    public bool storylineNpcSpawned;
    public float storylineNpcSpawnChance = 0.2f;
}