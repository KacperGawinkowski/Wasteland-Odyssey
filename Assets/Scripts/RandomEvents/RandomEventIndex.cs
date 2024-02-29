using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RandomEventIndex
{
    public static bool s_IsInitialized;
    
    private static RandomEventPreset[] s_AllRandomEvents;

    public static void Initialize()
    {
        if (s_IsInitialized == false)
        {
            s_AllRandomEvents = Resources.LoadAll<RandomEventPreset>("RandomEventPresets");

            s_IsInitialized = true;
        }
    }

    public static RandomEventPreset GetRandomEventPreset()
    {
        return s_AllRandomEvents[Random.Range(0, s_AllRandomEvents.Length)];
    }
}
