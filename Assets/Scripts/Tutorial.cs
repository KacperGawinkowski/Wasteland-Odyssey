using System;
using UnityEngine;

[Serializable]
public class Tutorial
{
    public string tutorialText;
    public string positiveButtonText;

    public bool isItOnlyTextTutorial;
    
    public GameObject objectToInteract;

    /*[HideInInspector]*/ public bool isFinished = false;


}
