using System;
using System.Collections.Generic;
using System.Linq;
using NPC.Friendly;
using UnityEngine;
using World;

namespace NPC
{
    public class QuestGiverController : MonoBehaviour, INpcDialogueAction, ISaveData<QuestGiverData>
    {
        public List<Quest> questsToGive;
        public FriendlyNpcController npc;

        private QuestGiverData m_QuestGiverData;

        private int m_QuestRefreshFrequencyInDays = 7;

        private void Start()
        {
            if (questsToGive == null)
            {
                questsToGive = new List<Quest>();
            }
            if (m_QuestGiverData.onWhichDayWasQuestGenerated <= TimeController.Instance.dayCounter - m_QuestRefreshFrequencyInDays || questsToGive.Count == 0)
            {
                for (int i = questsToGive.Count; i < 3; i++)
                {
                    questsToGive.Add(Quest.CreateRandomQuest(npc));
                }
                
                m_QuestGiverData.onWhichDayWasQuestGenerated = TimeController.Instance.dayCounter;
            }
        }

        public void AddDialogueActions()
        {
            // CanvasController.Instance.dialogueCanvasController.SetQuestOption(this);
            
            DialogueAction dialogueAction = new()
            {
                action = () =>
                {
                    CanvasController.Instance.questChoosePanel.SetActive(true);
                    CanvasController.Instance.inventoryPanel.SetActive(false);
                    CanvasController.Instance.upgradePanel.SetActive(false);
                    CanvasController.Instance.questChooseController.ShowQuests(this);
                },
                interactable = () => true,
                name = "Quest"
            };

            CanvasController.Instance.dialogueCanvasController.AddButton(dialogueAction);
        }

        public void SetData(QuestGiverData data)
        {
            m_QuestGiverData = data;

            questsToGive = m_QuestGiverData.questsToGive;
        }

        public QuestGiverData GetData()
        {
            return m_QuestGiverData;
        }
    }

    [Serializable]
    public class QuestGiverData
    {
        public List<Quest> questsToGive;
        public int onWhichDayWasQuestGenerated;
    }
}