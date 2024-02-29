using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class QuestLog
{
    public List<Quest> quests = new();
    public int questsFinished = 0;
    
    public List<StorylineQuest> storylineQuests = new();
    public int storylineQuestsFinished = 0;
    
    public int GetRandomQuestDifficulty()
    {
        return Random.Range((1 + questsFinished) * 100, ((1 + questsFinished) * 100) * 3);
    }

    public void AddQuest(Quest quest)
    {
        //dodac quest do mapy globalnej
        quests.Add(quest);
        GlobalMapController.Instance.AddLocation(quest);
        CanvasController.Instance.questObjectiveTracker.SetObjectives();
    }

    public void RemoveQuest(Quest quest)
    {
        //usunac quest z mapy globalnej
        quests.Remove(quest);
        GlobalMapController.Instance.RemoveLocation(quest);
        CanvasController.Instance.questObjectiveTracker.SetObjectives();
    }

    public bool ContainsQuest(Quest quest)
    {
        return quests.Contains(quest);
    }
    
    public void AddStorylineQuest(StorylineQuest quest)
    {
        //dodac quest do mapy globalnej
        storylineQuests.Add(quest);
        if (quest.questLocation != null)
        {
            Debug.Log("Add storyline location");
            GlobalMapController.Instance.AddLocation(quest);
        }
        CanvasController.Instance.questObjectiveTracker.SetObjectives();
    }

    public void RemoveStorylineQuest(StorylineQuest quest)
    {
        //usunac quest z mapy globalnej
        storylineQuests.Remove(quest);
        if (quest.questLocation != null)
        {
            GlobalMapController.Instance.RemoveLocation(quest);
        }
        CanvasController.Instance.questObjectiveTracker.SetObjectives();
    }

    public bool ContainsStorylineQuest(StorylineQuest quest)
    {
        return storylineQuests.Contains(quest);
    }
}
