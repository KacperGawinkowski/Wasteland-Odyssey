using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;
    
    private string[] m_TutorialTexts;
    [SerializeField] private List<Tutorial> m_Tutorials = new();

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
    }
    
    void Start()
    {
        CanvasController.Instance.tutorialCanvasController.tutorialManager = this;
        Debug.Log($"{m_Tutorials.Count} tutorials loaded.   {m_Tutorials.Count(x => x.isFinished)} tutorials finished.");
        CanvasController.Instance.tutorialCanvasController.ShowCurrentTutorial();
    }
    
    public Tutorial GetCurrentTutorial()
    {
        if (m_Tutorials.Count(x => x.isFinished) < m_Tutorials.Count)
            return m_Tutorials[m_Tutorials.Count(x => x.isFinished)];
        else
            return null;
    }

    private void OnValidate()
    {
        m_TutorialTexts = Resources.Load<TextAsset>("Tutorials").text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

        while(m_Tutorials.Count < m_TutorialTexts.Length)
        {
            m_Tutorials.Add(new Tutorial());
            Debug.LogError("Tutorials in TutorialManager (object in World component in StartingLocation Scene) has to be adjusted!");
        }

        for (int i = 0; i < m_Tutorials.Count; i++)
        {
            m_Tutorials[i].tutorialText = m_TutorialTexts[i];
        }
    }
}
