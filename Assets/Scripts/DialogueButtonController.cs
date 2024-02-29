using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class DialogueButtonController : MonoBehaviour
{
    [SerializeField] private Button m_Button;

    [FormerlySerializedAs("m_Text")] [SerializeField]
    public TextMeshProUGUI text;

    public void SetAction(DialogueAction dialogueAction)
    {
        text.text = dialogueAction.name;

        m_Button.interactable = dialogueAction.interactable == null || dialogueAction.interactable.Invoke();

        m_Button.onClick.RemoveAllListeners();
        m_Button.onClick.AddListener(() =>
        {
            dialogueAction.action.Invoke();
            m_Button.interactable = dialogueAction.interactable == null || dialogueAction.interactable.Invoke();
        });
    }
}