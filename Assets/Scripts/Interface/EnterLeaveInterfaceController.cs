using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnterLeaveInterfaceController : MonoBehaviour
{
    [SerializeField] private GameObject m_EnterButton;
    [SerializeField] private GameObject m_LeaveButton;
    
    [SerializeField] private TextMeshProUGUI m_LocationName;
    [SerializeField] private TextMeshProUGUI m_Description;

    public void ShowEnterLocationInterface(Location location)
    {
        Clear();
        gameObject.SetActive(true);
        m_EnterButton.SetActive(true);
        if(location is VillageData village) m_LocationName.text = village.villageName;
        else if(location is Quest quest) m_LocationName.text = quest.questLocation.locationName;
        StopAllCoroutines();
        StartCoroutine(TypeText(m_Description, "Do you want to enter?"));
        //m_Description.text = "Do you want to enter?";
    }

    public void ShowLeaveLocationInterface(Location location)
    {
        Clear();
        gameObject.SetActive(true);
        m_LeaveButton.SetActive(true);
        if(location is VillageData village) m_LocationName.text = village.villageName;
        else if(location is Quest quest) m_LocationName.text = quest.questLocation.locationName;
        else if (location is null && SaveSystem.saveContent.questLog.storylineQuestsFinished == 1) m_LocationName.text = "Your house";
        StopAllCoroutines();
        StartCoroutine(TypeText(m_Description, "Do you want to leave?"));
        //m_Description.text = "Do you want to leave?";
    }
    
    private IEnumerator TypeText(TextMeshProUGUI TmpText, string textToType)
    {
        TmpText.text = "";
        foreach (char letter in textToType.ToCharArray())
        {
            TmpText.text += letter;
            yield return null;
        }
    }

    public void Clear()
    {
        gameObject.SetActive(false);
        m_EnterButton.SetActive(false);
        m_LeaveButton.SetActive(false);
        m_LocationName.text = "";
        m_Description.text = "";
    }
}
