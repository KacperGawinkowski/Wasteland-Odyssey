using System;
using System.Collections;
using System.Collections.Generic;
using NPC;
using UnityEngine;
using UnityEngine.Serialization;

public class QuestChooseController : MonoBehaviour
{
    [SerializeField] private QuestChooseDescription[] m_QuestChooseDescriptions;

    public void ShowQuests(QuestGiverController questGiverController)
    {
        foreach (QuestChooseDescription questChooseDescription in m_QuestChooseDescriptions)
        {
            questChooseDescription.childZero.SetActive(false);
        }

        for (int i = 0; i < questGiverController.questsToGive.Count; i++)
        {
            m_QuestChooseDescriptions[i].childZero.SetActive(true);
            m_QuestChooseDescriptions[i].PrepareQuestChooseDescription(questGiverController.questsToGive[i], questGiverController);
        }
    }

    private void OnValidate()
    {
        if (m_QuestChooseDescriptions.Length == 0)
        {
            m_QuestChooseDescriptions = GetComponentsInChildren<QuestChooseDescription>();
        }
    }
}