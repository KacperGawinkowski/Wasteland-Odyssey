using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoofTriggerController : MonoBehaviour
{
    [SerializeField] private RoofTrigger[] m_Roofs;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            for (int i = 0; i < m_Roofs.Length; i++)
            {
                try
                {
                    m_Roofs[i].PlayerEnter();
                }
                catch (Exception)
                {
                    Debug.LogError($"RoofTrigger Exception [Index {i}]");
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            for (int i = 0; i < m_Roofs.Length; i++)
            {
                try
                {
                    m_Roofs[i].PlayerExit();
                }
                catch (Exception)
                {
                    Debug.LogError($"RoofTrigger Exception [Index {i}]");
                }
            }
        }
    }
}