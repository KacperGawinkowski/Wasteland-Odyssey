using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;

public class QuestObjectiveTracker : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_StorylineQuestObjectiveText;
    [SerializeField] private GameObject m_SideQuestObjectiveObject;
    [SerializeField] private TextMeshProUGUI m_SideQuestObjectiveText;


    public void Start()
    {
        SetObjectives();
    }

    public void SetObjectives()
    {
        m_SideQuestObjectiveText.text = "";

        m_StorylineQuestObjectiveText.text = StorylineQuestIndex.GetQuest(SaveSystem.saveContent.questLog.storylineQuestsFinished).GetQuestObjective();
        SetLocationObjective();
    }

    private void SetLocationObjective()
    {
        Location currentLocation = PlayerController.Instance.playerControllerGlobal.currentLocation;

        if (PlayerController.Instance.globalMapPlayer.activeSelf)
        {
            List<Quest> questList = SaveSystem.saveContent.questLog.quests.Where(x => x.questStatus == QuestStatus.Completed).ToList();
            m_SideQuestObjectiveObject.SetActive(questList.Count > 0);
            SetCompletedQuestsText(questList);
        }
        else
        {
            List<Quest> questList = SaveSystem.saveContent.questLog.quests.Where(x => x.questStatus != QuestStatus.Failed).ToList();
            SetLocationObjectiveText(questList,currentLocation);
        }
    }

    private void SetLocationObjectiveText(List<Quest> questList, Location playerLocation)
    {
        if (playerLocation is VillageData village)
        {
            List<Quest> completedQuestsInVillage = questList.
                Where(x => x.friendlyNpcData.villageData == village).
                Where(x => x.questStatus == QuestStatus.Completed).ToList();

            if (completedQuestsInVillage.Count > 0)
            {
                m_SideQuestObjectiveObject.SetActive(true);
                foreach (Quest quest in completedQuestsInVillage)
                {
                    SetObjectiveText(quest);
                }
            }
            else
            {
                m_SideQuestObjectiveObject.SetActive(false);
            }
            
        }
        else if (playerLocation is Quest)
        {
            Quest questInCurrentLocation = questList.FirstOrDefault(x => x == playerLocation);
            if (questInCurrentLocation != null)
            {
                m_SideQuestObjectiveObject.SetActive(true);
                SetObjectiveText(questInCurrentLocation);
            }
        }
    }

    private void SetObjectiveText(Quest quest)
    {
        if (!m_SideQuestObjectiveText.text.Contains(quest.GetObjectiveText()))
        {
            m_SideQuestObjectiveText.text += quest.GetObjectiveText();
        }
    }
    
    private void SetCompletedQuestsText(List<Quest> questList)
    {
        foreach (Quest quest in questList)
        {
            SetObjectiveText(quest);
        }
    }
}
