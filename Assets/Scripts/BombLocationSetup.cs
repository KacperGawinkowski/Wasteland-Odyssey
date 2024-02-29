using System;
using System.Collections;
using System.Collections.Generic;
using InventorySystem;
using UnityEngine;

public class BombLocationSetup : MonoBehaviour
{
    [SerializeField] private GameObject m_PlayerSpawnPoint;

    [SerializeField] private EnemyNpc m_Mech;
    void Start()
    {
        // Debug.Log($"bomb location start: {Time.realtimeSinceStartup}");
        PlayerController.Instance.PrepareForFriendlyLocation(m_PlayerSpawnPoint.transform.position);
        CanvasController.Instance.clockInterface.UpdateLocationName("Home");
        
        m_Mech.FitEnemy(10);
        m_Mech.equipmentController.weaponObject.transform.GetChild(0).gameObject.SetActive(false);
        PlayerController.Instance.healthController.ApplyDamage(10);
    }
}
