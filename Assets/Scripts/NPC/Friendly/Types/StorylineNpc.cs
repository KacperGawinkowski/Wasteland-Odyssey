using NPC.Friendly;
using UnityEngine;

namespace NPC
{
    public class StorylineNpc : MonoBehaviour, INpcDialogueAction
    {
        public FriendlyNpcController npc;

        public bool isBombNpc = false;

        private void Start()
        {
            npc = GetComponent<FriendlyNpcController>();
            FindDialogueLine();
        }

        public void SetNextStorylineQuest()
        {
            StorylineQuestController.SetStorylineQuest(npc);
        }

        private void FindDialogueLine()
        {
            if (isBombNpc)
            {
                if (StorylineQuestController.s_StorylineQuest.questStatus == QuestStatus.Completed)
                {
                    npc.dialogueText = StorylineQuestIndex.GetQuest(0).storylineQuestNpcCompleteDialogue;
                }
                else
                {
                    npc.dialogueText = StorylineQuestIndex.GetQuest(0).storylineQuestNpcTakeDialogue;
                }
            }
            else
            {
                if (StorylineQuestController.s_StorylineQuest.questStatus == QuestStatus.Completed)
                {
                    npc.dialogueText = StorylineQuestIndex
                        .GetQuest(StorylineQuestController.s_StorylineQuest.storylineQuestIndex)
                        .storylineQuestNpcCompleteDialogue;
                }
                else
                {
                    npc.dialogueText = StorylineQuestIndex
                        .GetQuest(StorylineQuestController.s_StorylineQuest.storylineQuestIndex)
                        .storylineQuestNpcTakeDialogue;
                }
            }
        }

        public void AddDialogueActions()
        {
            // if (isBombNpc && SaveSystem.saveContent.questLog.storylineQuestsFinished > 0)
            // {
            //     return;
            // }

            // CanvasController.Instance.storylineDialogueCanvasController.PrepareDialogue(this);

            ///////

            DialogueAction dialogueAction = null;
            if (isBombNpc)
            {
                if (StorylineQuestController.s_StorylineQuest.IsQuestObjectiveFulfilled())
                {
                    CanvasController.Instance.dialogueCanvasController.ConfigureExitButton(false);
                    dialogueAction = new DialogueAction
                    {
                        name = StorylineQuestIndex.GetQuest(0).storylineQuestButtonLabel,
                        action = () =>
                        {
                            StorylineQuestController.s_StorylineQuest.CheckIfQuestCompleted();
                            if (SaveSystem.saveContent.questLog.storylineQuestsFinished == 0)
                            {
                                SaveSystem.saveContent.questLog.storylineQuestsFinished++;
                                SetNextStorylineQuest();
                            }

                            CanvasController.Instance.dialogueCanvasController.gameObject.SetActive(false);
                        }
                    };
                }
            }
            else if (SaveSystem.saveContent.questLog.storylineQuestsFinished < 3)
            {
                dialogueAction = new DialogueAction
                {
                    name = StorylineQuestIndex.GetQuest(StorylineQuestController.s_StorylineQuest.storylineQuestIndex).storylineQuestButtonLabel,
                    action = () =>
                    {
                        PlayerController.Instance.equipmentController.SetMoney(
                            PlayerController.Instance.equipmentController.GetMoney() - StorylineQuestIndex.GetQuest(StorylineQuestController.s_StorylineQuest.storylineQuestIndex).amountOfMoneyNeeded
                        );
                        StorylineQuestController.s_StorylineQuest.CheckIfQuestCompleted();
                        if (SaveSystem.saveContent.questLog.storylineQuestsFinished < StorylineQuestIndex.s_NumberOfQuests - 1)
                        {
                            SaveSystem.saveContent.questLog.storylineQuestsFinished++;
                            SetNextStorylineQuest();
                            if (SaveSystem.saveContent.questLog.storylineQuestsFinished != 1)
                            {
                                FindDialogueLine();
                                CanvasController.Instance.dialogueCanvasController.PrepareNpcDialog(npc);
                            }
                            else
                            {
                                CanvasController.Instance.storylineDialogueCanvasController.gameObject.SetActive(false);
                            }
                        }
                    },
                    interactable = () => PlayerController.Instance.equipmentController.GetMoney() >= StorylineQuestIndex.GetQuest(StorylineQuestController.s_StorylineQuest.storylineQuestIndex).amountOfMoneyNeeded
                };
            }

            if (dialogueAction != null)
            {
                CanvasController.Instance.dialogueCanvasController.AddButton(dialogueAction);
            }
        }
    }
}