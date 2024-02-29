using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomEventPresetController : MonoBehaviour
{
    public Transform spawnPoint;
    public EnemySpawnPointList enemySpawnPoints;
    public LootGeneratorController lootGeneratorController;
}

public class RandomEventData
{
    public RandomEventPreset randomEventPreset;
    public int numberOfEnemies;
    public int encounterDifficulty;
    public int randomEnemyValue;
    public RandomEventData()
    {
        if (!RandomEventIndex.s_IsInitialized)
        {
            RandomEventIndex.Initialize();
        }

        randomEventPreset = RandomEventIndex.GetRandomEventPreset();
        
        // randomEnemyValue = Random.Range((SaveSystem.saveContent.questLog.questsFinished + 1) * 1000, (SaveSystem.saveContent.questLog.questsFinished + 2) * 1000);
        randomEnemyValue = Random.Range((SaveSystem.saveContent.questLog.questsFinished + 1) * 200, (SaveSystem.saveContent.questLog.questsFinished + 2) * 200);
        numberOfEnemies = Random.Range(randomEventPreset.minNumberOfEnemies, Mathf.Clamp(SaveSystem.saveContent.questLog.questsFinished / 2 + 1, 1, randomEventPreset.maxNumberOfEnemies));
        encounterDifficulty = SaveSystem.saveContent.questLog.GetRandomQuestDifficulty();
    }
}
