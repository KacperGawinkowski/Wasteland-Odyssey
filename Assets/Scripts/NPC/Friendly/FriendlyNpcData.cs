using NPC;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using World.TownGeneration;

[Serializable]
public class FriendlyNpcData
{
	public int spawnPointId;
	public string npcName;
	[SerializeReference] public VillageData villageData;
	public HouseData houseData;
	public QuestGiverData questGiverData;
	public TraderData traderData;
	public NpcType npcType;

	public EquipmentSaveData equipmentSaveData;
}

[Flags]
public enum NpcType : byte
{
	other = 0,
	wanderer = 1,
	healer = 2,
	upgrader = 4,
	storylineNpc = 8,
	bombNpc = 16
}
