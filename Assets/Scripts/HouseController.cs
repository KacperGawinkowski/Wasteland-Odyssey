using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using InventorySystem;
using NPC;
using NPC.Friendly;
using TMPro;
using UnityEngine;
using World.TownGeneration;
using Random = UnityEngine.Random;

public class HouseController : MonoBehaviour
{
    [SerializeField] private GameObject m_WandererPointListObject; // ?
    [SerializeField] public Transform[] m_WandererPoints;
    [SerializeField] public NpcSpawnPoint[] npcSpawnPoints;

    public void GenerateNPCs(VillageData villageData, HouseData houseData, List<FriendlyNpcData> important, List<FriendlyNpcData> fillers, float chanceForStorylineNpc)
    {
        for (int i = 0; i < npcSpawnPoints.Length; i++)
        {
            NpcSpawnPoint spawnPoint = npcSpawnPoints[i];
            FriendlyNpcData npcData = new()
            {
                spawnPointId = i,
                npcName = NameGenerator.GenerateName(),
                villageData = villageData,
                houseData = houseData,
                npcType = NpcType.other
            };

            bool isImportant = false;

            if (Random.value < spawnPoint.chanceToBecomeTrader)
            {
                npcData.traderData = new TraderData()
                {
                    traderType = spawnPoint.traderType,
                    itemSaveData = null
                };
                isImportant = true;
            }
            else if (Random.value < spawnPoint.chanceToBecomeHealer)
            {
                npcData.npcType |= NpcType.healer;
                isImportant = true;
            }
            else if (Random.value < spawnPoint.chanceToBecomeUpgrader)
            {
                npcData.npcType |= NpcType.upgrader;
                isImportant = true;
            }

            if (isImportant)
            {
                important.Add(npcData);
            }
            else
            {
                npcData.npcType |= TryToSpawnStorylineNpc(npcData, chanceForStorylineNpc);
                fillers.Add(npcData);
            }
        }
    }

    private NpcType TryToSpawnStorylineNpc(FriendlyNpcData npcData, float chanceForStorylineNpc)
    {
        if (SaveSystem.saveContent.storylineNpcSpawned) return NpcType.wanderer;

        float randomNumber = Random.value;
        if (randomNumber < chanceForStorylineNpc)
        {
            npcData.npcType |= NpcType.storylineNpc;
            npcData.npcName = Constants.s_StorylineNPCName;
            SaveSystem.saveContent.storylineNpcSpawned = true;
            return NpcType.storylineNpc;
        }
        else
        {
            return NpcType.wanderer;
        }
    }

    private void OnValidate()
    {
        if (m_WandererPointListObject != null)
        {
            List<Transform> susList = new List<Transform>();
            foreach (Transform t in m_WandererPointListObject.transform)
            {
                susList.Add(t);
            }

            m_WandererPoints = susList.ToArray();
        }

        npcSpawnPoints = GetComponentsInChildren<NpcSpawnPoint>();
    }
}