using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunMoonMenuController : MonoBehaviour
{
    [SerializeField] private Light m_Sun;

    private void Update()
    {
        DateTime now = DateTime.Now;
        float timeInSeconds = (now.Hour * 60 + now.Minute) * 60 + now.Second;
        m_Sun.transform.localRotation = Quaternion.Euler(new Vector3((timeInSeconds / (24f * 60f * 60f)) * 360f - 90f, 30f, 0));
    }
}