using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "RandomEventPreset", menuName = "Quests/RandomEvent", order = 2)]
public class RandomEventPreset : ScriptableObject
{
    public int randomEventPresetIndex;
    public int minNumberOfEnemies;
    public int maxNumberOfEnemies;
    public bool canBeSkipped;
    public bool enemiesHaveToBeKilled;
    [TextArea] public string eventDescription;
    public int skipPrice;

    public string fightText;
    public string skipText;
    public string actorName;

}