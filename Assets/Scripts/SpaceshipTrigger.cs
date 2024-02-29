using System.Collections;
using System.Collections.Generic;
using HealthSystem;
using UnityEngine;

public class SpaceshipTrigger : MonoBehaviour
{
    [SerializeField] private SpaceshipInteractable m_SpaceshipInteractable;
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<EnemyNpc>())
        {
            m_SpaceshipInteractable.enemiesAroundTheShip.Add(other.GetComponent<EnemyNpc>());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            m_SpaceshipInteractable.enemiesAroundTheShip.Remove(other.GetComponent<EnemyNpc>());
        }
    }
}
