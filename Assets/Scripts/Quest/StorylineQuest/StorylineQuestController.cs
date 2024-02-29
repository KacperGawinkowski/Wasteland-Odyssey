using System;
using NPC.Friendly;
using UnityEngine;
using Random = UnityEngine.Random;

public static class StorylineQuestController
{
    public static StorylineQuest s_StorylineQuest;

    // public static void SetBombasticStorylineQuest()
    // {
    //     if (SaveSystem.saveContent.questLog.storylineQuestsFinished == 0)
    //     {
    //         if (!SaveSystem.saveContent.questLog.ContainsStorylineQuest(s_StorylineQuest))
    //         {
    //             s_StorylineQuest = new StorylineQuest(SaveSystem.saveContent.questLog.storylineQuestsFinished, null,new Vector2Int(-100,-100));
    //             SaveSystem.saveContent.questLog.AddStorylineQuest(s_StorylineQuest);
    //             s_StorylineQuest.questStatus = QuestStatus.InProgress;
    //         }
    //     }
    // }
    
    public static void SetFirstStorylineQuest()
    {
        if (SaveSystem.saveContent.questLog.storylineQuestsFinished == 0)
        {
            if (!SaveSystem.saveContent.questLog.ContainsStorylineQuest(s_StorylineQuest))
            {
                s_StorylineQuest = new StorylineQuest(SaveSystem.saveContent.questLog.storylineQuestsFinished, null,new Vector2Int(-100,-100));
                SaveSystem.saveContent.questLog.AddStorylineQuest(s_StorylineQuest);
                s_StorylineQuest.questStatus = QuestStatus.InProgress;
            }
        }
    }

    public static void SetStorylineQuest(FriendlyNpcController friendlyNpcController)
    {
        if (s_StorylineQuest.storylineQuestIndex <= 1)
        {
            if (friendlyNpcController.friendlyNpcData == null)
            {
                s_StorylineQuest = new StorylineQuest(SaveSystem.saveContent.questLog.storylineQuestsFinished, null, new Vector2Int(-420,-420));
            }
            else
            {
                s_StorylineQuest = new StorylineQuest(SaveSystem.saveContent.questLog.storylineQuestsFinished, friendlyNpcController.friendlyNpcData, new Vector2Int(friendlyNpcController.friendlyNpcData.villageData.positionX,friendlyNpcController.friendlyNpcData.villageData.positionY));
            }
            
            SaveSystem.saveContent.questLog.AddStorylineQuest(s_StorylineQuest);
            s_StorylineQuest.questStatus = QuestStatus.InProgress;
                
        }
        else
        {
            s_StorylineQuest.enemyCount = Random.Range(12, Mathf.Clamp(SaveSystem.saveContent.questLog.questsFinished / 2 + 1, 12, 25));
            s_StorylineQuest.enemyValue = Random.Range((SaveSystem.saveContent.questLog.questsFinished + 1) * 2000, (SaveSystem.saveContent.questLog.questsFinished + 2) * 5000);
            
            int x;
            int y;
            do
            {
                x = Random.Range(0, GlobalMapController.Instance.m_SizeX);
                y = Random.Range(0, GlobalMapController.Instance. m_SizeY);
            } while (GlobalMapController.Instance.IsPositionADesert(new Vector2Int(x,y)) == false || GlobalMapController.Instance.IsPositionALocation(new Vector2Int(x,y)) == false);
        
            Vector2Int mapCoordinates = new Vector2Int(x,y);
            
            s_StorylineQuest = new StorylineQuest(SaveSystem.saveContent.questLog.storylineQuestsFinished, friendlyNpcController.friendlyNpcData, mapCoordinates);

            
            s_StorylineQuest.questStatus = QuestStatus.InProgress;
            s_StorylineQuest.questLocation = Resources.Load<QuestLocation>("QuestLocations/Story/LastQuest");
            SaveSystem.saveContent.questLog.AddStorylineQuest(s_StorylineQuest);
            Debug.Log($"Emre Ertan {s_StorylineQuest.questLocation}");
        }
    }
}