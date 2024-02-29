using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialCanvasController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_TutorialText;
    
    public Button nextTutorialButton;
    [HideInInspector] public TutorialManager tutorialManager;
    
    // private bool m_TutorialEnabled;
    private Tutorial m_CurrentTutorial;
    

    public void ShowCurrentTutorial()
    {
        gameObject.SetActive(true);
        // m_TutorialEnabled = true;
        m_CurrentTutorial = tutorialManager.GetCurrentTutorial();
        StopAllCoroutines();

        if (m_CurrentTutorial != null)
        {
            StartCoroutine(TypeText(m_CurrentTutorial.tutorialText));
            nextTutorialButton.GetComponentInChildren<TextMeshProUGUI>().text = m_CurrentTutorial.positiveButtonText;
            nextTutorialButton.interactable = m_CurrentTutorial.isItOnlyTextTutorial;
        }
        else
        {
            StartCoroutine(TypeText("Tutorial has been finished"));
            nextTutorialButton.GetComponentInChildren<TextMeshProUGUI>().text = "Ok";
            nextTutorialButton.interactable = true;
        }
    }

    private IEnumerator TypeText(string textToType)
    {
        m_TutorialText.text = "";
        foreach (char letter in textToType.ToCharArray())
        {
            m_TutorialText.text += letter;
            yield return null;
        }
    }


    public void SetNextTutorialButtonAction()
    {
        if (m_CurrentTutorial != null)
        {
            m_CurrentTutorial.isFinished = true;
            ShowCurrentTutorial();
        }
        else
        {
            CloseTutorial();
        }
    }

    public void CloseTutorial()
    {
        // m_TutorialEnabled = false;
        gameObject.SetActive(false);
    }
}
