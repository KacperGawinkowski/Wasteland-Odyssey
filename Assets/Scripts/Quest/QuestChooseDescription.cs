using System;
using System.Collections;
using System.Collections.Generic;
using InventorySystem.Items;
using InventorySystem.UI;
using NPC;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class QuestChooseDescription : MonoBehaviour
{
    [SerializeField] private Button m_PositiveButton;
    [SerializeField] private TextMeshProUGUI m_ButtonText;
    [SerializeField] private Image m_ButtonImage;
    [SerializeField] private Color m_CompleteColor;
    [SerializeField] private Color m_TakeColor;
    [SerializeField] private Color m_AbortColor;
    
    [SerializeField] private TextMeshProUGUI m_QuestDescription;

    [SerializeField] private Image m_RewardIcon;
    [SerializeField] private TextMeshProUGUI m_RewardItemName;

    public GameObject childZero;

    public void PrepareQuestChooseDescription(Quest quest, QuestGiverController npc)
    {
        //m_QuestTitle.text = "Side Quest";
        m_QuestDescription.text = quest.GetQuestDescription();
        SetButtonsAndStatus(quest, npc);
        SetReward(quest);
    }

    private void SetButtonsAndStatus(Quest quest, QuestGiverController questGiverController)
    {
        if (SaveSystem.saveContent.questLog.ContainsQuest(quest))
        {
            if (quest.questStatus == QuestStatus.Completed)
            {
                m_ButtonImage.color = m_CompleteColor;
                //m_QuestCompletedIcon.SetActive(true);

                m_ButtonText.text = "Complete";
                m_PositiveButton.onClick.RemoveAllListeners();
                m_PositiveButton.onClick.AddListener(() =>
                {
                    SaveSystem.saveContent.questLog.RemoveQuest(quest);
                    questGiverController.questsToGive.Remove(quest);
                    SaveSystem.saveContent.questLog.questsFinished++;
                    PlayerController.Instance.equipmentController.AddItem((IItem)ItemIndex.GetById(quest.reward.itemId), quest.reward.amount);
                    childZero.gameObject.SetActive(false);
                });
            }
            else if (quest.questStatus == QuestStatus.InProgress)
            {
                m_ButtonImage.color = m_AbortColor;
                //m_QuestCompletedIcon.SetActive(false);

                m_ButtonText.text = "Abandon";
                m_PositiveButton.onClick.RemoveAllListeners();
                m_PositiveButton.onClick.AddListener(() =>
                {
                    SaveSystem.saveContent.questLog.RemoveQuest(quest);
                    PrepareQuestChooseDescription(quest, questGiverController);
                });
            }
            else if (quest.questStatus == QuestStatus.Failed)
            {
                gameObject.SetActive(false);
            }
        }
        else
        {
            //m_QuestCompletedIcon.SetActive(false);

            m_ButtonImage.color = m_TakeColor;

            m_ButtonText.text = "Accept";
            m_PositiveButton.onClick.RemoveAllListeners();
            m_PositiveButton.onClick.AddListener(() =>
            {
                SaveSystem.saveContent.questLog.AddQuest(quest);
                quest.questStatus = QuestStatus.InProgress;
                PrepareQuestChooseDescription(quest, questGiverController);
            });
        }
    }

    private void SetReward(Quest quest)
    {
        m_RewardIcon.sprite = ItemIndex.GetById(quest.reward.itemId).itemIcon;
        //m_RewardItemName.text = ItemIndex.GetById(quest.reward.itemId).itemName;
        if (quest.reward.amount > 1)
        {
            m_RewardItemName.text = $"x {quest.reward.amount}";
        }

        // m_RewardButton.onClick.RemoveAllListeners();
        // m_RewardButton.onClick.AddListener(() =>
        // {
        //     m_RewardItemDescriptionPanel.SetActive(true);
        //     itemDescriptionController.ShowDescription((IItem)ItemIndex.GetById(quest.reward.itemId), quest.reward.amount);
        // });
    }
}