using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class QuestLocationController : MonoBehaviour
{
    public EnemySpawnPointList spawnPoints;
    public Transform playerSpawnPoint;
    public LootGeneratorController lootGeneratorController;

    private void Start()
    {
        Quest quest = (Quest)PlayerController.Instance.playerControllerGlobal.currentLocation;

        if (quest is StorylineQuest)
        {
            quest.enemyCount = 25;
            quest.enemyValue = Random.Range((SaveSystem.saveContent.questLog.questsFinished + 1) * 2000 + (1000 * quest.enemyCount) * 5, (SaveSystem.saveContent.questLog.questsFinished + 2) * 5000 + (1000 * quest.enemyCount) * 7);
        }

        if (quest != null)
        {
            SpawnEnemies(quest.enemyCount, quest.enemyValue);
            lootGeneratorController.GenerateLoots(quest.questDifficulty * 5);

            switch (quest)
            {
                case LootQuest lootQuest:
                    lootGeneratorController.PutItemInRandomActiveChest((QuestItem)ItemIndex.GetById(lootQuest.questItemId));
                    break;
            }
        }
        else
        {
            Debug.LogError("Entered quest location without active quest!");
        }

        PlayerController.Instance.PrepareForHostileLocation(playerSpawnPoint.transform.position);
    }

    private void SpawnEnemies(int count, int value)
    {
        int spawnedEnemies = 0;
        int valueForEnemy = value / count;
        while (spawnedEnemies != count)
        {
            int rand = Random.Range(0, spawnPoints.spawnPoints.Length);
            EnemyNpc enemy = spawnPoints.spawnPoints[rand].SpawnEnemy();
            if (enemy is not null)
            {
                spawnedEnemies++;
                enemy.FitEnemy(valueForEnemy);
            }
        }
    }

    private void OnValidate()
    {
        if (spawnPoints == null) spawnPoints = GetComponentInChildren<EnemySpawnPointList>();
        if (lootGeneratorController == null) lootGeneratorController = GetComponentInChildren<LootGeneratorController>();
    }
}