using System;
using InventorySystem.Items;
using System.Collections.Generic;
using Player;
using Unity.AI.Navigation;
using UnityEngine;
using Random = UnityEngine.Random;

public class QuestController : MonoBehaviour
{
    // [SerializeField] private NavMeshSurface m_NavMeshSurface;

    private void Start()
    {
        Quest quest = (Quest)PlayerController.Instance.playerControllerGlobal.currentLocation;

        if (quest != null)
        {
            int variant = Random.Range(0, quest.questLocation.questLocationVariants.Length);
            Instantiate(quest.questLocation.questLocationVariants[variant], transform);
        }
    }

    // private void Start()
    // {
    //     
    //     
    //     Quest quest = (Quest)PlayerController.Instance.playerControllerGlobal.currentLocation;
    //
    //     if (quest is StorylineQuest)
    //     {
    //         quest.enemyCount = 25;
    //         quest.enemyValue = Random.Range((SaveSystem.saveContent.questLog.questsFinished + 1) * 2000 + (1000 * quest.enemyCount) * 5, (SaveSystem.saveContent.questLog.questsFinished + 2) * 5000 + (1000 * quest.enemyCount) * 7);
    //     }
    //     else
    //     {
    //         Debug.LogError("Entered quest location without active quest!");
    //     }
    //
    //     // PlayerController.Instance.playerControllerLocal.transform.position = m_PlayerSpawnPoint.transform.position;
    //     PlayerController.Instance.PrepareForHostileLocation(m_PlayerSpawnPoint.transform.position);
    // }


    // private void SpawnEnemies(int count, int value)
    // {
    //     int spawnedEnemies = 0;
    //     int valueForEnemy = value / count;
    //     while (spawnedEnemies != count)
    //     {
    //         int rand = Random.Range(0, m_SpawnPoints.spawnPoints.Length);
    //         EnemyNpc enemy = m_SpawnPoints.spawnPoints[rand].SpawnEnemy();
    //         if (enemy is not null)
    //         {
    //             spawnedEnemies++;
    //             enemy.FitEnemy(valueForEnemy);
    //         }
    //     }
    // }
}