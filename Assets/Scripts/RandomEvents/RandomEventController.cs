using System;
using System.Collections;
using System.Collections.Generic;
using InventorySystem.Items;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomEventController : MonoBehaviour
{
	[SerializeField] private RandomEventPresetController[] m_RandomEventPresets;

	private RandomEventPresetController m_ChosenRandomEvent;
	// Start is called before the first frame update

	private void Start()
	{
		PrepareRandomEventLocation(PlayerController.Instance.randomEventData);
	}

	public void PrepareRandomEventLocation(RandomEventData eventData)
	{
		ChoosePreset(eventData);

		if (m_ChosenRandomEvent.enemySpawnPoints != null)
		{
			SpawnEnemies(eventData.numberOfEnemies, eventData.randomEnemyValue);
		}


		if (m_ChosenRandomEvent.lootGeneratorController != null)
		{
			m_ChosenRandomEvent.lootGeneratorController.GenerateLoots(eventData.encounterDifficulty * 3);
		}
	}

	private void ChoosePreset(RandomEventData eventData)
	{
		try
		{
			m_ChosenRandomEvent = m_RandomEventPresets[eventData.randomEventPreset.randomEventPresetIndex];
		}
		catch (Exception)
		{
			m_ChosenRandomEvent = m_RandomEventPresets[0];
		}

		m_ChosenRandomEvent.gameObject.SetActive(true);

		if (m_ChosenRandomEvent.enemySpawnPoints)
		{
			PlayerController.Instance.PrepareForHostileLocation(m_ChosenRandomEvent.spawnPoint.transform.position);
		}
		else
		{
			PlayerController.Instance.PrepareForFriendlyLocation(m_ChosenRandomEvent.spawnPoint.transform.position);
		}
	}

	private void SpawnEnemies(int count, int value)
	{
		if (count > 0)
		{
			int spawnedEnemies = 0;
			int valueForEnemy = value / count;
			while (spawnedEnemies != count)
			{
				int rand = Random.Range(0, m_ChosenRandomEvent.enemySpawnPoints.spawnPoints.Length);
				EnemyNpc enemy = m_ChosenRandomEvent.enemySpawnPoints.spawnPoints[rand].SpawnEnemy();
				if (enemy is not null)
				{
					spawnedEnemies++;
					enemy.FitEnemy(valueForEnemy);
				}
			}
		}
	}

	private void OnValidate()
	{
		m_RandomEventPresets = GetComponentsInChildren<RandomEventPresetController>(true);
	}
}
