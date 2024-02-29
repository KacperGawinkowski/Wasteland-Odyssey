using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class ObjectNameTagController : MonoBehaviour
{
    [SerializeField] private TextMeshPro m_TextMesh;
    private bool m_IsTimeBound;

    private void Update()
    {
        if (PlayerController.Instance)
        {
            Camera c = PlayerController.Instance.playerControllerLocal.localCamera;
            m_TextMesh.transform.LookAt(c.transform.position);
            m_TextMesh.transform.Rotate(0, 180, 0);

            if (m_IsTimeBound)
            {
                m_TextMesh.transform.position += new Vector3(0, Random.Range(1, 6), 0) * Time.deltaTime;
            }
        }
    }

    private void SetColor(Color color)
    {
        m_TextMesh.color = color;
    }

    private void SetText(string text)
    {
        m_TextMesh.text = text;
    }

    public void SetHitForDuration(string text, Color color, int seconds)
    {
        SetText(text);
        SetColor(color);
        m_IsTimeBound = true;
        Destroy(gameObject, seconds);
    }

    public void ShowNpcName(string text, Color color)
    {
        SetText(text);
        SetColor(color);
    }
}