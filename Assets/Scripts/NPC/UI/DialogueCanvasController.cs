using System;
using System.Collections;
using System.Collections.Generic;
using NPC;
using NPC.Friendly;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class DialogueCanvasController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_NpcName;
    [SerializeField] private TextMeshProUGUI m_DialogText;
    [SerializeField] private GameObject m_ButtonContainer;

    // [SerializeField] private GameObject m_QuestOption;
    // [SerializeField] private GameObject m_TradeOption;
    // [SerializeField] private GameObject m_UpgradeOption;
    // [SerializeField] private GameObject m_HealOption;
    //
    // [SerializeField] private Button questButton, tradeButton, upgradeButton, healButton;

    [SerializeField] private DialogueButtonController m_ButtonPrefab;
    [SerializeField] private DialogueButtonController m_ExitButton;
    [SerializeField] private List<DialogueButtonController> m_ButtonControllers;
    private int m_ActiveButtons;

    // private FriendlyNpcController npc;

    public void PrepareNpcDialog(FriendlyNpcController npc)
    {
        m_NpcName.text = npc.npcName;
        StopAllCoroutines();
        StartCoroutine(TypeTextCoroutine(m_DialogText, npc.dialogueText));

        m_ActiveButtons = 0;
        
        foreach (DialogueButtonController dialogueButtonController in m_ButtonControllers)
        {
            dialogueButtonController.gameObject.SetActive(false);
        }
        
        foreach (INpcDialogueAction dialogueAction in npc.dialogueActions)
        {
            dialogueAction.AddDialogueActions();
        }
    }

    public void ConfigureExitButton(bool visible)
    {
        m_ExitButton.gameObject.SetActive(visible);
        // m_ExitButton.text.text = text;
    }

    private IEnumerator TypeTextCoroutine(TextMeshProUGUI tmpText, string textToType)
    {
        tmpText.text = "";
        foreach (char letter in textToType)
        {
            tmpText.text += letter;
            yield return null;
        }
    }

    public void AddButton(DialogueAction dialogueAction)
    {
        if (m_ActiveButtons >= m_ButtonControllers.Count)
        {
            DialogueButtonController dialogueButtonController = Instantiate(m_ButtonPrefab, m_ButtonContainer.transform);
            m_ButtonControllers.Add(dialogueButtonController);
        }
        else
        {
            m_ButtonControllers[m_ActiveButtons].gameObject.SetActive(true);
        }
        
        m_ButtonControllers[m_ActiveButtons].SetAction(dialogueAction);
        m_ActiveButtons++;
    }

    // public void SetQuestOption(QuestGiverController questNpc)
    // {
    //     m_QuestOption.SetActive(true);
    //
    //     questButton.onClick.AddListener(() =>
    //     {
    //         CanvasController.Instance.questChoosePanel.SetActive(true);
    //         CanvasController.Instance.inventoryPanel.SetActive(false);
    //         CanvasController.Instance.upgradePanel.SetActive(false);
    //         CanvasController.Instance.questChooseController.ShowQuests(questNpc);
    //     });
    // }
    //
    // public void SetTradeOption(TraderController traderNpc)
    // {
    //     m_TradeOption.SetActive(true);
    //
    //     tradeButton.onClick.AddListener(() =>
    //     {
    //         CanvasController.Instance.inventoryPanel.SetActive(true);
    //         CanvasController.Instance.questChoosePanel.SetActive(false);
    //         CanvasController.Instance.upgradePanel.SetActive(false);
    //         CanvasController.Instance.inventoryInterface.CreateTradePanel(traderNpc, InventoryInterfaceType.Trading, "Trade");
    //         gameObject.SetActive(false);
    //     });
    // }
    //
    // public void SetUpgradeOption(Upgrader upgrader)
    // {
    //     m_UpgradeOption.SetActive(true);
    //
    //     upgradeButton.onClick.AddListener(() =>
    //     {
    //         CanvasController.Instance.upgradePanel.SetActive(true);
    //         CanvasController.Instance.questChoosePanel.SetActive(false);
    //         CanvasController.Instance.inventoryPanel.SetActive(false);
    //         CanvasController.Instance.upgradePanelController.ShowItemInstancesInEquipment();
    //     });
    // }
    //
    // public void SetHealOption(Healer healer)
    // {
    //     m_HealOption.SetActive(true);
    //     healButton.interactable = PlayerController.Instance.equipmentController.GetMoney() >= 1000;
    //
    //     healButton.onClick.AddListener(() =>
    //     {
    //         PlayerController.Instance.equipmentController.SetMoney(PlayerController.Instance.equipmentController.GetMoney() - 1000);
    //         PlayerController.Instance.healthController.HealAll();
    //         healButton.interactable = PlayerController.Instance.equipmentController.GetMoney() >= 1000;
    //     });
    //
    // }
    //
    // private void ClearListeners()
    // {
    //     healButton.onClick.RemoveAllListeners();
    //     questButton.onClick.RemoveAllListeners();
    //     tradeButton.onClick.RemoveAllListeners();
    //     upgradeButton.onClick.RemoveAllListeners();
    // }

    private void OnDisable()
    {
        CanvasController.Instance.upgradePanel.SetActive(false);
        CanvasController.Instance.questChoosePanel.SetActive(false);
    }

    // private void OnValidate()
    // {
    //     questButton = m_QuestOption.GetComponent<Button>();
    //     tradeButton = m_TradeOption.GetComponent<Button>();
    //     upgradeButton = m_UpgradeOption.GetComponent<Button>();
    //     healButton = m_HealOption.GetComponent<Button>();
    // }
}

public class DialogueAction
{
    public string name;
    public Action action;
    public Func<bool> interactable;
}