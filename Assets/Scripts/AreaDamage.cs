using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HealthSystem;
using UnityEngine;

public class AreaDamage : MonoBehaviour
{
	[SerializeField] private bool m_TargetPlayer;
	[SerializeField] private bool m_TargetEnemies;

	[SerializeField] private int m_Damage;
	[SerializeField] private float m_DamageTime;
	[SerializeField] private float m_LiveTime;

	private List<HealthController> m_DamageList;
	private List<float> m_DamageTimers;

	private void Start()
	{
		m_DamageList = new List<HealthController>();
		m_DamageTimers = new List<float>();
		Destroy(gameObject, m_LiveTime);
	}

	private void FixedUpdate()
	{
		for (int i = 0; i < m_DamageList.Count; i++)
		{
			HealthController item = m_DamageList[i];
			m_DamageTimers[i] -= Time.deltaTime;
			while (m_DamageTimers[i] <= 0)
			{
				m_DamageTimers[i] += m_DamageTime;
				item.ApplyDamage(m_Damage);
			}
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if ((m_TargetEnemies && other.gameObject.CompareTag("Enemy")))
		{
			HealthController hc = other.gameObject.GetComponent<HealthController>();
			m_DamageList.Add(hc);
			m_DamageTimers.Add(m_DamageTime);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if ((m_TargetEnemies && other.gameObject.CompareTag("Enemy")))
		{
			HealthController hc = other.gameObject.GetComponent<HealthController>();
			int i = m_DamageList.FindIndex((x) => x == hc);
			m_DamageList.RemoveAt(i);
			m_DamageTimers.RemoveAt(i);
		}
	}
}