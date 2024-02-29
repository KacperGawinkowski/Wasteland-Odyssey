using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStarter : MonoBehaviour
{
    //List<string> saveFiles = new();

    //public void Start()
    //{
    //    Debug.Log($"{Application.persistentDataPath}");

    //    DirectoryInfo dir = new(Application.persistentDataPath);
    //    FileInfo[] fileInfos = dir.GetFiles($"*.{SaveSystem.fileExtension}");
    //    foreach (FileInfo item in fileInfos)
    //    {
    //        print(item.Name);
    //    }
    //}

    //public void NewGame()
    //{
    //    //TODO TOODOO  BROOODOOO
    //    GameLoader.Instance.LoadPlayer();
    //}

    //public void LoadGame(string fileName)
    //{
    //    //TODO TOODOO  BROOODOOO
    //    SaveSystem.Load(fileName);
    //    GameLoader.Instance.LoadPlayer();
    //}

    private void Start()
    {
        Time.timeScale = 1;
    }

    public void StartGame()
    {
        SaveSystem.Load("save");

        if (SaveSystem.saveContent.questLog.storylineQuestsFinished > 0)
        {
            GameLoader.Instance.UnloadScene();
        }
        else
        {
            GameLoader.Instance.LoadStartingQuest();
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}