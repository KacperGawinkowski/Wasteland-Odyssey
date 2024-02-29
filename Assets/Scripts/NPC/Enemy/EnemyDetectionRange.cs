using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils.StateMachine;

public class EnemyDetectionRange : MonoBehaviour
{
    [SerializeField] private EnemyNpc m_EnemyNpc;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            m_EnemyNpc.playerInDetectionRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("PlayerTriggerRange"))
        {
            m_EnemyNpc.playerInDetectionRange = false;
        }
    }
}