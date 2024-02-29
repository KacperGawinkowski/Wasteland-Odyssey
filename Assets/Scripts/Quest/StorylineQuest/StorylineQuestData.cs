using System;
using NPC;
using UnityEngine;

[CreateAssetMenu(fileName = "StorylineQuest", menuName = "Quests/Storyline Quest")]
public class StorylineQuestData : ScriptableObject
{
    public int storylineQuestIndex;
    [TextArea] public string storylineQuestNpcTakeDialogue;
    [TextArea] public string storylineQuestNpcCompleteDialogue;
    [TextArea] public string storylineQuestDescription;
    [TextArea] public string storylineShortenedQuestObjective;

    public string storylineQuestButtonLabel;

    public int amountOfMoneyNeeded;

    public string GetQuestObjective()
    {
        string description = StorylineQuestIndex.GetQuest(storylineQuestIndex).storylineShortenedQuestObjective;
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
}