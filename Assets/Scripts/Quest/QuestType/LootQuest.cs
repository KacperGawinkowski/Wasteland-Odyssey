using InventorySystem.Items;
using Player;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NPC.Friendly;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

[Serializable]
public class LootQuest : Quest
{
    public string questItemId;

    public LootQuest()
    {
    }

    public LootQuest(QuestLocation questLocation, int questDifficulty, int enemyCount, int enemyValue, FriendlyNpcData friendlyNpcData, Vector2Int mapCoordinates) :
        base(questLocation, questDifficulty, enemyCount, enemyValue, friendlyNpcData, mapCoordinates)
    {
        RandomizeItemToLoot();
    }

    public void RandomizeItemToLoot()
    {
        BaseItem[] possibleItems = ItemIndex.GetByType<QuestItem>();

        possibleItems = possibleItems.Where(item => ((QuestItem)item).location.Contains(questLocation)).ToArray();

        questItemId = ((QuestItem)possibleItems[Random.Range(0, possibleItems.Length - 1)]).ItemName;
    }

    public override string GetQuestDescription()
    {
        return $"Go to the {questLocation.locationName} and bring {questItemId}. The site might be guarded by around {enemyCount} bandits";
    }

    public override void CheckIfQuestCompleted()
    {
        if (PlayerController.Instance.equipmentController.InventoryDictionary.GetAmount((QuestItem)ItemIndex.GetById(questItemId)) > 0)
        {
            questStatus = QuestStatus.Completed;
            CanvasController.Instance.questObjectiveTracker.SetObjectives();
        }
    }

    public override string GetObjectiveText()
    {
        return questStatus switch
        {
            QuestStatus.Completed => $"Return to {friendlyNpcData.npcName} in {friendlyNpcData.villageData.villageName}\n",
            QuestStatus.InProgress => $"Find the {ItemIndex.GetById(questItemId).itemName}\n",
            _ => "\n"
        };
    }
}