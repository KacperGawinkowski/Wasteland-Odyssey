using System;
using System.Collections;
using System.Collections.Generic;
using InventorySystem.UI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class InventoryAmountPicker : MonoBehaviour
{
    //[SerializeField] private TMP_InputField m_InputField;

    [SerializeField] private TextMeshProUGUI m_AmountLabel;
    [SerializeField] private Slider m_Slider;
    [SerializeField] private InventoryListEntry m_ThisListEntry;

    [NonSerialized] public Action<int> action;
    private int m_Amount = 1;

    public void CheckIfAmountIsValid()
    {
        // if (int.TryParse(m_InputField.text, out int number))
        // {
        //     if (number < 0)
        //     {
        //         m_InputField.text = "1";
        //         number = 1;
        //     }
        //     
        //     if (number > m_ThisListEntry.itemQuantity)
        //     {
        //         m_InputField.text = m_ThisListEntry.itemQuantity.ToString();
        //         number = m_ThisListEntry.itemQuantity;
        //     }
        //
        //     m_Amount = number;
        // }
    }

    public void InvokeAction()
    {
        action?.Invoke(m_Amount);
    }

    public void ToggleAmountPicker(bool value)
    {
        m_Slider.minValue = 1;
        m_Slider.maxValue = m_ThisListEntry.itemQuantity;
        m_Slider.value = 1;
        //m_InputField.text = "";
        gameObject.SetActive(value);
        //m_Amount = 1;
    }

    public void SliderChanged()
    {
        m_AmountLabel.text = m_Slider.value.ToString();
    }
    
    public void IsMouseReleased(bool value)
    {
        if (value)
        {
            action?.Invoke((int)m_Slider.value);
            m_Slider.value = 1;
            m_AmountLabel.text = "1";
            m_Slider.maxValue = m_ThisListEntry.itemQuantity;
        }
    }

}