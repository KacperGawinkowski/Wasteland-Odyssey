using System;
using System.Collections;
using System.Collections.Generic;
using HealthSystem.UI;
using Interface;
using InventorySystem;
using InventorySystem.Items;
using InventorySystem.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static PlayerWsadInput;

public class CanvasController : MonoBehaviour, ICanvasActions
{
    public static CanvasController Instance;

    [Header("Interface Panels")] [SerializeField]
    private GameObject m_HealthPanel;

    [SerializeField] private GameObject m_QuestPanel;
    public GameObject inventoryPanel;
    [SerializeField] private GameObject m_ClockPanel;
    [SerializeField] private GameObject m_AmmunitionPanel;
    public GameObject m_DialoguePanel;
    public GameObject questChoosePanel;
    public GameObject upgradePanel;
    
    public EnterLeaveInterfaceController enterLeaveInterfaceController;

    public LoadingInterfaceController loadingInterface;
    public ClockInterfaceController clockInterface;
    public InventoryCanvasController inventoryInterface;
    public CanvasAmmoController ammoInterface;
    public QuestCanvasController questCanvasController;
    public DialogueCanvasController dialogueCanvasController;
    public QuestChooseController questChooseController;
    public UpgradePanelController upgradePanelController;
    public SkillsCanvasController skillsCanvasController;
    public RandomEventInterfaceController randomEventInterfaceController;
    public StorylineDialogueCanvasController storylineDialogueCanvasController;
    public QuestObjectiveTracker questObjectiveTracker;
    public TutorialCanvasController tutorialCanvasController;
    public HealthHUDController healthHUDController;

    public GameObject hubertEffects;

    public  Stack<GameObject> m_OpenedInterfaces = new Stack<GameObject>();

    public GameObject menuPanel;

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        InputManager.input.Canvas.SetCallbacks(this);
    }

    private void OnEnable()
    {
        InputManager.input.Canvas.Enable();
    }

    private void OnDisable()
    {
        InputManager.input.Canvas.Disable();
    }

    private void OnDestroy()
    {
        InputManager.input.Canvas.RemoveCallbacks(this);
    }

    public void ToggleInventory()
    {
        m_HealthPanel.SetActive(false);
        m_QuestPanel.SetActive(false);
        m_DialoguePanel.SetActive(false);
        questChoosePanel.SetActive(false);
        inventoryPanel.SetActive(!inventoryPanel.activeSelf);
        upgradePanel.SetActive(false);
    }

    public void CreatePlayerInventoryPanel()
    {
        inventoryInterface.CreatePlayerInventoryPanel();
    }

    public void ToggleHealthPanel()
    {
        inventoryPanel.SetActive(false);
        m_QuestPanel.SetActive(false);
        questChoosePanel.SetActive(false);
        m_DialoguePanel.SetActive(false);
        m_HealthPanel.SetActive(!m_HealthPanel.activeSelf);
        upgradePanel.SetActive(false);
    }

    public void ToggleQuestPanel()
    {
        m_HealthPanel.SetActive(false);
        inventoryPanel.SetActive(false);
        m_DialoguePanel.SetActive(false);
        questChoosePanel.SetActive(false);
        m_QuestPanel.SetActive(!m_QuestPanel.activeSelf);
        upgradePanel.SetActive(false);
    }

    public void ToggleDialoguePanel()
    {
        m_DialoguePanel.SetActive(!m_DialoguePanel.activeSelf);
        m_HealthPanel.SetActive(false);
        inventoryPanel.SetActive(false);
        m_QuestPanel.SetActive(false);
        questChoosePanel.SetActive(false);
        upgradePanel.SetActive(false);
    }

    public void ToggleQuestChoosePanel()
    {
        questChoosePanel.SetActive(!questChoosePanel.activeSelf);
        m_HealthPanel.SetActive(false);
        inventoryPanel.SetActive(false);
        m_QuestPanel.SetActive(false);
        m_DialoguePanel.SetActive(false);
        upgradePanel.SetActive(false);
    }

    public void ToggleUpgradePanel()
    {
        upgradePanel.SetActive(!upgradePanel.activeSelf);
        questChoosePanel.SetActive(false);
        m_HealthPanel.SetActive(false);
        inventoryPanel.SetActive(false);
        m_QuestPanel.SetActive(false);
        m_DialoguePanel.SetActive(false);
    }


    public void SetActiveClock(bool value)
    {
        m_ClockPanel.SetActive(value);
        m_AmmunitionPanel.SetActive(value);
    }

    public void HideAllNPCPanels()
    {
        upgradePanel.SetActive(false);
        questChoosePanel.SetActive(false);
        m_DialoguePanel.SetActive(false);
    }

    public void OnOpenMenu(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            while (m_OpenedInterfaces.Count > 0 && m_OpenedInterfaces.Peek().gameObject.activeSelf == false)
            {
                m_OpenedInterfaces.Pop();
            }
            
            if (m_OpenedInterfaces.Count > 0)
            {
                GameObject uiObject = m_OpenedInterfaces.Peek().gameObject;
                if (uiObject != menuPanel.gameObject)
                {
                    uiObject.gameObject.SetActive(false);
                    m_OpenedInterfaces.Pop();
                }
                else
                {
                    ExitOptions exitOptions = menuPanel.GetComponent<ExitOptions>();
                    
                    if (exitOptions.m_ControlsMenuPanel.activeSelf)
                    {
                        exitOptions.m_ControlsMenuPanel.SetActive(false);
                        exitOptions.m_OptionsMenuPanel.SetActive(true);
                    }
                    else
                    {
                        menuPanel.SetActive(false);
                        Time.timeScale = 1;
                        m_OpenedInterfaces.Pop();
                    }
                }
            }
            else
            {
                menuPanel.SetActive(!menuPanel.activeSelf);
                Time.timeScale = menuPanel.activeSelf ? 0 : 1;
                //menuPanel.SetActive(!menuPanel.activeSelf);
            }
        }
    }

    public void OnShowInventory(InputAction.CallbackContext context)
    {
        if (!menuPanel.activeSelf)
        {
            ToggleInventory();
            inventoryInterface.CreatePlayerInventoryPanel();
        }
    }

    public void OnShowHealthInterface(InputAction.CallbackContext context)
    {
        if (!menuPanel.activeSelf)
        {
            ToggleHealthPanel();
        }
    }

    public void OnShowQuestInterface(InputAction.CallbackContext context)
    {
        if (!menuPanel.activeSelf)
        {
            ToggleQuestPanel();
        }
    }
}