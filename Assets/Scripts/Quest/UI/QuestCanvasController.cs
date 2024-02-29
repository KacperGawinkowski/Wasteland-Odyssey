using System;
using System.Collections;
using System.Collections.Generic;
using InventorySystem.Items;
using InventorySystem.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using World;

public class QuestCanvasController : MonoBehaviour
{
    [FormerlySerializedAs("m_QuestList")] public GameObject questList;
    [SerializeField] private GameObject m_QuestInfo;

    [SerializeField] private TextMeshProUGUI m_QuestTitle;
    [SerializeField] private TextMeshProUGUI m_QuestTimeToFinish;
    [SerializeField] private GameObject m_QuestCompletedIcon;
    [SerializeField] private GameObject m_QuestFailedIcon;
    [SerializeField] private TextMeshProUGUI m_QuestNpc;
    [SerializeField] private TextMeshProUGUI m_QuestDescription;

    [SerializeField] private Image m_RewardIcon;
    [SerializeField] private TextMeshProUGUI m_RewardItemName;

    [SerializeField] private GameObject m_RewardObject;
    [SerializeField] private Button m_AbandonButton;

    [SerializeField] private GameObject m_ButtonsContainer;

    //[SerializeField] private GameObject m_RewardItemDescriptionPanel;
    //public ItemDescriptionController itemDescriptionController;

    [SerializeField] private QuestButton m_QuestButtonPrefab;


    public void SetQuestInfo(Quest quest, QuestButton questButton, string questTitle)
    {
        //m_RewardButton.onClick.RemoveAllListeners();
        m_AbandonButton.onClick.RemoveAllListeners();
        m_QuestInfo.SetActive(true);
        m_ButtonsContainer.SetActive(true);

        m_QuestTimeToFinish.gameObject.SetActive(false);
        
        m_QuestTitle.text = questTitle;
        m_QuestCompletedIcon.SetActive(quest.questStatus == QuestStatus.Completed);
        m_QuestFailedIcon.SetActive(quest.questStatus == QuestStatus.Failed);

        if (quest.friendlyNpcData != null)
        {
            m_QuestNpc.text = $"{quest.friendlyNpcData.npcName} from {quest.friendlyNpcData.villageData.villageName}";
        }
        else
        {
            m_QuestNpc.text = "Quest wasnt not taken from NPC";
        }

        m_QuestDescription.text = quest.GetQuestDescription();

        if (quest.reward.itemId != null)
        {
            m_AbandonButton.gameObject.SetActive(true);
            m_RewardObject.SetActive(true);
            
            m_RewardIcon.sprite = ItemIndex.GetById(quest.reward.itemId).itemIcon;
            m_RewardItemName.text = ItemIndex.GetById(quest.reward.itemId).itemName;
            if (quest.reward.amount > 1)
            {
                m_RewardItemName.text += " x " + quest.reward.amount;
            }

            m_AbandonButton.onClick.AddListener(() =>
            {
                //m_RewardItemDescriptionPanel.SetActive(false);
                m_QuestInfo.SetActive(false);
                m_ButtonsContainer.SetActive(false);
                SaveSystem.saveContent.questLog.RemoveQuest(quest);
                Destroy(questButton.gameObject);
            });
        }
        else
        {
            m_AbandonButton.gameObject.SetActive(false);
            m_RewardObject.SetActive(false);
        }
    }
    
    public void SetQuestInfo(StorylineQuest quest, string questTitle)
    {
        TimeController.Instance.OnDayChanged.AddListener(UpdateDayCounter);
        //m_RewardButton.onClick.RemoveAllListeners();
        m_AbandonButton.onClick.RemoveAllListeners();
        m_QuestInfo.SetActive(true);
        m_ButtonsContainer.SetActive(true);

        m_QuestTimeToFinish.gameObject.SetActive(true);
        m_QuestTimeToFinish.text = "Days left : " + (100 - TimeController.Instance.dayCounter);
        
        m_QuestTitle.text = questTitle;
        if (quest != null)
        {
            m_QuestCompletedIcon.SetActive(quest.questStatus == QuestStatus.Completed);
            m_QuestFailedIcon.SetActive(quest.questStatus == QuestStatus.Failed);

            if (quest.friendlyNpcData != null)
            {
                m_QuestNpc.text = $"{quest.friendlyNpcData.npcName} from {quest.friendlyNpcData.villageData.villageName}";
            }
            else
            {
                m_QuestNpc.text = "";
            }

            m_QuestDescription.text = quest.GetQuestDescription();
        }
        else
        {
            m_QuestNpc.text = "";
            m_QuestDescription.text = "Go back to <npcName> from <villageName> to continue the storyline";
        }

        m_AbandonButton.gameObject.SetActive(false);
        m_RewardObject.SetActive(false);
    }

    private void UpdateDayCounter()
    {
        m_QuestTimeToFinish.text = "Days left : " + (100 - TimeController.Instance.dayCounter);
    }
    

    private void OnEnable()
    {
        if (!CanvasController.Instance.m_OpenedInterfaces.Contains(gameObject))
        {
            CanvasController.Instance.m_OpenedInterfaces.Push(gameObject);
        }
        
        QuestButton storylineQuestButton = Instantiate(m_QuestButtonPrefab, questList.transform);
        storylineQuestButton.storylineQuest = StorylineQuestController.s_StorylineQuest;
        storylineQuestButton.GetComponentInChildren<TextMeshProUGUI>().text = "Main Quest";
        storylineQuestButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            SetQuestInfo(StorylineQuestController.s_StorylineQuest, "Main Quest");
            if (PlayerController.Instance.globalMapPlayer.activeSelf)
            {
                if (StorylineQuestController.s_StorylineQuest?.storylineQuestIndex == 2)
                {
                    GlobalMapController.Instance.SetLocationAsSelected(StorylineQuestController.s_StorylineQuest);
                }
            }
        });
        
        
        foreach (Quest quest in SaveSystem.saveContent.questLog.quests)
        {
            if (quest is StorylineQuest) return;
            
            QuestButton questButton = Instantiate(m_QuestButtonPrefab, questList.transform);
            questButton.quest = quest;

            questButton.GetComponentInChildren<TextMeshProUGUI>().text = "Side Quest";

            questButton.GetComponent<Button>().onClick.AddListener(() =>
            {
                SetQuestInfo(questButton.quest, questButton,"Side Quest");
                if (PlayerController.Instance.globalMapPlayer.activeSelf)
                {
                    GlobalMapController.Instance.SetLocationAsSelected(questButton.quest);
                }
            });
        }
    }
    
    private void OnDisable()
    {
        TimeController.Instance.OnDayChanged.RemoveListener(UpdateDayCounter);
        foreach (Transform child in questList.transform)
        {
            Destroy(child.gameObject);
        }

        m_QuestInfo.SetActive(false);
        m_ButtonsContainer.SetActive(false);
        
        if (GlobalMapController.Instance != null)
        {
            GlobalMapController.Instance.SetLocationAsSelected(null);
            //GlobalMapController.Instance.SpawnTextAboveLocation(null);
        }

        GlobalMapController.Instance.DeselectLocations();


        //m_RewardItemDescriptionPanel.SetActive(false);
    }
}
