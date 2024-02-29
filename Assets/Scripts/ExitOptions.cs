using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitOptions : MonoBehaviour
{
    public GameObject m_OptionsMenuPanel;
    public GameObject m_ControlsMenuPanel;

    public void ExitToDesktop()
    {
        Application.Quit();
    }

    public void ExitToMainMenu()
    {
        Save();
        GameLoader.Instance.LoadMainMenu();
    }

    private void OnApplicationQuit()
    {
        Save();
    }

    private static void Save()
    {
        if (PlayerController.Instance.playerControllerGlobal.currentLocation is Quest quest)
        {
            if (PlayerController.Instance.playerControllerLocal.isActiveAndEnabled)
            {
                quest.questStatus = QuestStatus.Failed;
                GlobalMapController.Instance.RemoveLocation(quest);
            }
        }

        SaveSystem.Save(SaveSystem.currentSaveName);
    }

    private void OnEnable()
    {
        if (!CanvasController.Instance.m_OpenedInterfaces.Contains(gameObject))
        {
            CanvasController.Instance.m_OpenedInterfaces.Push(gameObject);
        }
    }
}