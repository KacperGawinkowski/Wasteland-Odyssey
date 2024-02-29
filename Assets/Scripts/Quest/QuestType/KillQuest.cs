using System;
using System.Collections;
using System.Collections.Generic;
using NPC.Friendly;
using UnityEngine;

[Serializable]
public class KillQuest : Quest
{
    public KillQuest()
    {
    }

    public KillQuest(QuestLocation questLocation, int questDifficulty, int enemyCount, int enemyValue, FriendlyNpcData friendlyNpcData, Vector2Int mapCoordinates) :
        base(questLocation, questDifficulty, enemyCount, enemyValue, friendlyNpcData, mapCoordinates)
    { }

    public override void CheckIfQuestCompleted()
    {
        if (EnemyNpc.aliveEnemies.Count <= 0)
        {
            questStatus = QuestStatus.Completed;
            CanvasController.Instance.questObjectiveTracker.SetObjectives();
        }
    }

    public override string GetQuestDescription()
    {
        return $"Go to the {questLocation.locationName} and kill {enemyCount} bandits";
    }

    public override string GetObjectiveText()
    {
        return questStatus switch
        {
            QuestStatus.Completed => $"Return to {friendlyNpcData.npcName} in {friendlyNpcData.villageData.villageName}\n",
            QuestStatus.InProgress => $"Killed {enemyCount - EnemyNpc.aliveEnemies.Count} / {enemyCount}\n",
            _ => "\n"
        };
    }
}
