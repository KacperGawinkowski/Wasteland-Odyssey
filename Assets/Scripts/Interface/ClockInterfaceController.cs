using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClockInterfaceController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_LocatioNane;
    [SerializeField] private TextMeshProUGUI m_DayText;
    [SerializeField] private Transform m_IconsContainer;

    private int m_CurrentDay;

    public void UpdateLocationName(string name)
    {
        m_LocatioNane.text = name;
    }

    public void SetDay(int day)
    {
        if (m_CurrentDay != day)
        {
            m_CurrentDay = day;
            m_DayText.text = day.ToString();
        }
    }

    public void SetHour(float hour)
    {
        m_IconsContainer.rotation = Quaternion.Euler(0, 0, (hour / 24) * -360);
    }
}
