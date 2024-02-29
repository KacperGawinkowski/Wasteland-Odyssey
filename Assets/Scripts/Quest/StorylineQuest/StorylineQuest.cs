using System;
using NPC;
using NPC.Friendly;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class StorylineQuest : Quest
{
    public int storylineQuestIndex;

    public StorylineQuest()
    {
    }

    public StorylineQuest(int index, FriendlyNpcData friendlyNpcData, Vector2Int mapCoordinates) : base(friendlyNpcData, mapCoordinates)
    {
        StorylineQuestIndex.Initialize();
        storylineQuestIndex = index;
    }

    public override void CheckIfQuestCompleted()
    {
        if (storylineQuestIndex == 0)
        {
            questStatus = QuestStatus.Completed;
        }
        else if (storylineQuestIndex == 1)
        {
            if (PlayerController.Instance.playerControllerGlobal.currentLocation is VillageData)
            {
                questStatus = QuestStatus.Completed;
            }
        }
        else if (storylineQuestIndex == 2)
        {
            if (PlayerController.Instance.equipmentController.GetMoney() >= StorylineQuestIndex.GetQuest(storylineQuestIndex).amountOfMoneyNeeded)
            {
                questStatus = QuestStatus.Completed;
            }
        }
        else
        {
            //TODO trzeci quest na statek ez? zbyteczne ez
        }
    }

    public bool IsQuestObjectiveFulfilled()
    {
        if (storylineQuestIndex < 2)
        {
            return true;
        }

        if (storylineQuestIndex == 2)
        {
            if (PlayerController.Instance.equipmentController.GetMoney() >= StorylineQuestIndex.GetQuest(storylineQuestIndex).amountOfMoneyNeeded)
            {
                return true;
            }
        }

        return false;
    }

    public override string GetQuestDescription()
    {
        string description = StorylineQuestIndex.GetQuest(storylineQuestIndex).storylineQuestDescription;
        try
        {
            description = description.Replace("<npcName>", Constants.s_StorylineNPCName);
            description = description.Replace("<amountOfMoney>", StorylineQuestIndex.GetQuest(storylineQuestIndex).amountOfMoneyNeeded.ToString());
        }
        catch (Exception)
        {
            // ignored
        }

        return description;
    }

    public override string GetObjectiveText()
    {
        return "";
    }
}