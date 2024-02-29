using System;
using System.Collections;
using System.Runtime.InteropServices;
using NPC;
using NPC.Friendly;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StorylineDialogueCanvasController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_NpcName;
    [SerializeField] private TextMeshProUGUI m_DialogText;
    [SerializeField] private GameObject m_PositiveButton;
    [SerializeField] private Button positiveButton;

    [SerializeField] private GameObject m_NegativeButton;

    private FriendlyNpcController npc;

    public void PrepareDialogue(StorylineNpc questNpc)
    {
        gameObject.SetActive(true);
        ClearListeners();
        npc = questNpc.npc;
        m_NpcName.text = npc.npcName;

        StopAllCoroutines();


        if (questNpc.isBombNpc)
        {
            if (StorylineQuestController.s_StorylineQuest.questStatus == QuestStatus.Completed)
            {
                StartCoroutine(TypeText(m_DialogText, StorylineQuestIndex.GetQuest(0).storylineQuestNpcCompleteDialogue));
                //m_DialogText.text = StorylineQuestIndex.GetQuest(0).storylineQuestNpcCompleteDialogue;
            }
            else
            {
                StartCoroutine(TypeText(m_DialogText, StorylineQuestIndex.GetQuest(0).storylineQuestNpcTakeDialogue));
                //m_DialogText.text = StorylineQuestIndex.GetQuest(0).storylineQuestNpcTakeDialogue;
            }
        }
        else
        {
            if (StorylineQuestController.s_StorylineQuest.questStatus == QuestStatus.Completed)
            {
                StartCoroutine(TypeText(m_DialogText, StorylineQuestIndex.GetQuest(StorylineQuestController.s_StorylineQuest.storylineQuestIndex).storylineQuestNpcCompleteDialogue));
                //m_DialogText.text = StorylineQuestIndex.GetQuest(StorylineQuestController.s_StorylineQuest.storylineQuestIndex).storylineQuestNpcCompleteDialogue;
            }
            else
            {
                StartCoroutine(TypeText(m_DialogText, StorylineQuestIndex.GetQuest(StorylineQuestController.s_StorylineQuest.storylineQuestIndex).storylineQuestNpcTakeDialogue));
                //m_DialogText.text = StorylineQuestIndex.GetQuest(StorylineQuestController.s_StorylineQuest.storylineQuestIndex).storylineQuestNpcTakeDialogue;
            }
        }

        SetStorylineQuestOption(questNpc);
    }

    public IEnumerator TypeText(TextMeshProUGUI tmpText, string textToType)
    {
        tmpText.text = "";
        foreach (char letter in textToType)
        {
            tmpText.text += letter;
            yield return null;
        }
    }


    public void SetStorylineQuestOption(StorylineNpc questNpc)
    {
        if (StorylineQuestController.s_StorylineQuest.IsQuestObjectiveFulfilled())
        {
            //sus w przypadku rozmowy z bombą nie powinno być innej opcji dialogowej niz "Ok" wiec wyłączam "BYWAJ" button xd
            m_NegativeButton.SetActive(!questNpc.isBombNpc /*SaveSystem.saveContent.questLog.storylineQuestsFinished != 0*/);

            m_PositiveButton.SetActive(true);
            if (questNpc.isBombNpc)
            {
                positiveButton.GetComponentInChildren<TextMeshProUGUI>().text = StorylineQuestIndex.GetQuest(0).storylineQuestButtonLabel;
            }
            else
            {
                positiveButton.GetComponentInChildren<TextMeshProUGUI>().text = StorylineQuestIndex.GetQuest(StorylineQuestController.s_StorylineQuest.storylineQuestIndex).storylineQuestButtonLabel;
            }
        }


        positiveButton.onClick.AddListener(() =>
        {
            if (questNpc.isBombNpc)
            {
                StorylineQuestController.s_StorylineQuest.CheckIfQuestCompleted();
                if (SaveSystem.saveContent.questLog.storylineQuestsFinished == 0)
                {
                    SaveSystem.saveContent.questLog.storylineQuestsFinished++;
                    questNpc.SetNextStorylineQuest();
                }

                CanvasController.Instance.storylineDialogueCanvasController.gameObject.SetActive(false);
            }
            else
            {
                PlayerController.Instance.equipmentController.SetMoney(PlayerController.Instance.equipmentController.GetMoney() -
                                                                       StorylineQuestIndex.GetQuest(StorylineQuestController.s_StorylineQuest.storylineQuestIndex).amountOfMoneyNeeded);
                StorylineQuestController.s_StorylineQuest.CheckIfQuestCompleted();
                if (SaveSystem.saveContent.questLog.storylineQuestsFinished < StorylineQuestIndex.s_NumberOfQuests - 1)
                {
                    SaveSystem.saveContent.questLog.storylineQuestsFinished++;
                    questNpc.SetNextStorylineQuest();
                    if (SaveSystem.saveContent.questLog.storylineQuestsFinished != 1)
                    {
                        PrepareDialogue(questNpc);
                    }
                    else
                    {
                        CanvasController.Instance.storylineDialogueCanvasController.gameObject.SetActive(false);
                    }
                }
            }
        });
    }

    private void ClearListeners()
    {
        positiveButton.onClick.RemoveAllListeners();
        m_PositiveButton.SetActive(false);
    }

    // private void OnValidate()
    // {
    //     positiveButton = m_PositiveButton.GetComponent<Button>();
    // }
}