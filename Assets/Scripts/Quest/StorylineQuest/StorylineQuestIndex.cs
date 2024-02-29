using System;
using System.Collections.Generic;
using InventorySystem.Items;
using UnityEngine;

public static class StorylineQuestIndex
{
    private static bool s_IsInitialized = false;

    public static int s_NumberOfQuests;
    
    private static StorylineQuestData[] s_storylineIndex;
    
    public static void Initialize()
    {
        if (s_IsInitialized == false)
        {
            s_storylineIndex = Resources.LoadAll<StorylineQuestData>("StoryLineQuests");
            s_NumberOfQuests = s_storylineIndex.Length;
            s_IsInitialized = true;
        }
    }

    public static StorylineQuestData GetQuest(int questIndex)
    {
        if (!s_IsInitialized) Initialize();
        if (questIndex > s_storylineIndex.Length) return null;
        
        return s_storylineIndex[questIndex];
    }
}
