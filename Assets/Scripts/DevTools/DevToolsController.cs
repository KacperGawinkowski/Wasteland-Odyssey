using System;
using System.Diagnostics;
using InventorySystem.Items;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using World;
using Debug = UnityEngine.Debug;

namespace DevTools
{
    public class DevToolsController : MonoBehaviour
    {
        [SerializeField] private GameObject m_ToolsContainer;
        [SerializeField] private TMP_InputField m_Input;
        [SerializeField] private TMP_Text m_Output;

        private TimeController timeController;

        private float m_PreviousTimeScale;

        // Start is called before the first frame update
        void Start()
        {
            timeController = FindObjectOfType<TimeController>();
        }

        private void Update()
        {
            if (Keyboard.current.backquoteKey.wasPressedThisFrame)
            {
                if (m_ToolsContainer.activeSelf)
                {
                    m_ToolsContainer.SetActive(false);
                    Time.timeScale = m_PreviousTimeScale;
                    InputManager.input.PlayerLocal.Enable();

                    //PlayerController.Instance.playerControllerLocal.GetComponent<PlayerInput>().enabled = true;
                }
                else
                {
                    m_ToolsContainer.SetActive(true);
                    m_PreviousTimeScale = Time.timeScale;
                    InputManager.input.PlayerLocal.Disable();
                    //PlayerController.Instance.playerControllerLocal.GetComponent<PlayerInput>().enabled = false;
                    Time.timeScale = 0;
                }
            }
        }

        //public void ChangeTimeSpeed() //used in OnValueChanged in TMP_InputField
        //{
        //    if (float.TryParse(m_Input.text, out float speed))
        //    {
        //        timeController.timeSpeedMultiplier = speed;
        //    }

        //}

        public void ProcessCommand(string command)
        {
            string[] args = command.Split(' ');
            try
            {
                switch (args[0])
                {
                    case "timeset":
                        timeController.SetTime(int.Parse(args[1]));
                        m_Output.text += "time changed\n";
                        break;

                    case "give":
                        GiveCommand(args);
                        break;

                    case "items":
                        ItemsCommand(args);
                        break;

                    case "goto":
                        GotoCommand(args);
                        break;

                    // case "addquest":
                    //     GetQuestCommand(args);
                    //     break;

                    case "completequest":
                        CompleteQuest(args);
                        break;

                    case "gamespeed":
                        GameSpeed(args);
                        break;

                    case "clear":
                        m_Output.text = "";
                        break;

                    case "save":
                        SaveSystem.Save("save");
                        break;

                    case "queststatus":
                        m_Output.text += "Completed: " + SaveSystem.saveContent.questLog.storylineQuestsFinished + " storyline quests and " + SaveSystem.saveContent.questLog.questsFinished + " sidequests.\n";
                        break;

                    case "god":
                        PlayerController.Instance.healthController.godMode = !PlayerController.Instance.healthController.godMode;
                        m_Output.text += $"god mode {(PlayerController.Instance.healthController.godMode ? "enabled" : "disabled")}\n";
                        break;

                    case "showsave":
                        Debug.Log(Application.persistentDataPath);
#if PLATFORM_STANDALONE_WIN
                        Process.Start("explorer.exe", Application.persistentDataPath.Replace('/', '\\'));
#endif
                        break;

                    case "help":
                        m_Output.text += "timeset <hour> (sets the time in game)\n";
                        m_Output.text += "give <itemName> <itemAmount> <?itemLvl> (sets the time in game)\n";
                        m_Output.text += "items (lists all item names)\n";
                        m_Output.text += "goto <quest / village> <questIndex / villageIndex> (teleports player to location)\n";
                        // m_Output.text += "addquest <number of quests> (adds n quests)\n";
                        m_Output.text += "completequest <quest id> (completes the specified)\n";
                        m_Output.text += "gamespeed <float speed> (changes the game speed [1 is normal])\n";

                        m_Output.text += "queststatus (displays information about the number of completed side & storyline quests)\n";

                        m_Output.text += "god (enables/disables god mode)\n";
                        m_Output.text += "save (saves the game)\n";
                        m_Output.text += "showsave (opens save location)\n";
                        m_Output.text += "clear (clears the console)\n";
                        break;

                    default:
                        break;
                }
            }
            catch (Exception e)
            {
                m_Output.text += "Incorrect usage of " + args[0] + " command\n\n";
                Debug.LogException(e);
            }
        }

        #region commands

        private void GiveCommand(string[] args)
        {
            BaseItem item = ItemIndex.GetById(args[1]);
            if (item is UniqueItem uniqueItem)
            {
                UniqueItemInstance itemInstance = uniqueItem.CreateInstance(int.Parse(args[3]));
                PlayerController.Instance.equipmentController.AddItem(itemInstance, int.Parse(args[2]));
            }
            else
            {
                PlayerController.Instance.equipmentController.AddItem((IItem)ItemIndex.GetById(args[1]), int.Parse(args[2]));
            }

            m_Output.text += item.itemName + " added to inventory\n";
        }

        private void ItemsCommand(string[] args)
        {
            foreach (BaseItem baseItem in ItemIndex.GetAllItems())
            {
                string itemType = baseItem.GetType().ToString();
                m_Output.text += itemType.Split(".")[itemType.Split(".").Length - 1] + " " + baseItem.itemName + "\n";
            }
        }

        private void GotoCommand(string[] args)
        {
            if (args[1].ToLower() == "quest")
            {
                GameLoader.Instance.LoadQuest(SaveSystem.saveContent.questLog.quests[int.Parse(args[2])]);
                m_Output.text += "teleported to " + SaveSystem.saveContent.questLog.quests[int.Parse(args[2])].questLocation.locationName;
            }
            else if (args[1].ToLower() == "village")
            {
                GameLoader.Instance.LoadVillage();
                m_Output.text += "teleported to village";
            }
        }

        private void GetQuestCommand(string[] args)
        {
            for (int i = 0; i < int.Parse(args[1]); i++)
            {
                Quest quest = Quest.CreateRandomQuest(null);
                SaveSystem.saveContent.questLog.AddQuest(quest);
            }

            m_Output.text += "Created new random quest";
        }

        private void CompleteQuest(string[] args)
        {
            SaveSystem.saveContent.questLog.quests[int.Parse(args[1])].questStatus = QuestStatus.Completed;
            GlobalMapController.Instance.RemoveLocation(SaveSystem.saveContent.questLog.quests[int.Parse(args[1])]);
            m_Output.text += "Completed quest " + args[1];
        }

        private void GameSpeed(string[] args)
        {
            Time.timeScale = int.Parse(args[1]);
            m_Output.text += "Game speed time set to " + args[1];
        }

        #endregion
    }
}