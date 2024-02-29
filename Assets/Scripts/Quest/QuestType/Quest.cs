using System;
using InventorySystem.Items;
using NPC.Friendly;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public abstract class Quest : Location
{
    public QuestLocation questLocation;

    public int questDifficulty;
    public int enemyCount;
    public int enemyValue = 0;

    public FriendlyNpcData friendlyNpcData;
    public QuestReward reward;

    public QuestStatus questStatus;

    protected Quest()
    {
    }

    protected Quest(QuestLocation questLocation, int questDifficulty, int enemyCount, int sharedEnemyValue, FriendlyNpcData friendlyNpcData, Vector2Int mapCoordinates) : base(mapCoordinates)
    {
        this.questLocation = questLocation;
        this.questDifficulty = questDifficulty;
        this.enemyCount = enemyCount;
        this.friendlyNpcData = friendlyNpcData;
        enemyValue = sharedEnemyValue;
        reward = RandomizeQuestReward(questDifficulty);
    }

    public Quest(FriendlyNpcData friendlyNpcData, Vector2Int mapCoordinates) : base(mapCoordinates)
    {
        this.friendlyNpcData = friendlyNpcData;
    }

    public static Quest CreateRandomQuest(FriendlyNpcController npc)
    {
        int rand = Random.Range(0, 2);
        QuestLocation questLocation = QuestLocation.QuestLocations[Random.Range(0, QuestLocation.QuestLocations.Length)];
        int enemyCount = Random.Range(questLocation.minEnemiesCount, Mathf.Clamp(SaveSystem.saveContent.questLog.questsFinished / 2 + 1, questLocation.minEnemiesCount, questLocation.maxEnemiesCount));

        int randomEnemyValue = Random.Range((SaveSystem.saveContent.questLog.questsFinished + 1) * 500, (SaveSystem.saveContent.questLog.questsFinished + 2) * 500);

        int x;
        int y;
        do
        {
            x = Random.Range(0, GlobalMapController.Instance.m_SizeX);
            y = Random.Range(0, GlobalMapController.Instance.m_SizeY);
        } while (GlobalMapController.Instance.IsPositionADesert(new Vector2Int(x, y)) == false || GlobalMapController.Instance.IsPositionALocation(new Vector2Int(x, y)) == false);

        Vector2Int mapCoordinates = new Vector2Int(x, y);

        switch (rand)
        {
            case 0:
                return new KillQuest(questLocation, SaveSystem.saveContent.questLog.GetRandomQuestDifficulty(), enemyCount, randomEnemyValue, npc.friendlyNpcData, mapCoordinates);
            case 1:
                return new LootQuest(questLocation, SaveSystem.saveContent.questLog.GetRandomQuestDifficulty(), enemyCount, (int)(randomEnemyValue * 1.5f), npc.friendlyNpcData, mapCoordinates);
            default:
                return null;
        }
    }

    public abstract void CheckIfQuestCompleted();
    public abstract string GetQuestDescription();

    private static QuestReward RandomizeQuestReward(int questRewardValue)
    {
        float random = Random.Range(0f, 1f);

        if (random <= 1f) // (random <= 0.5f) money reward
        {
            QuestReward reward = new()
            {
                itemId = "Money",
                amount = questRewardValue
            };
            return reward;
        }
        else //item reward
        {
            return default;
            //Do zrobienia w przyszlosci moze, ale raczej nie ez
        }
    }

    public abstract string GetObjectiveText();
}

[Serializable]
public struct QuestReward
{
    public string itemId;
    public int amount;
}

public enum QuestStatus : byte
{
    InProgress,
    Completed,
    Failed
}