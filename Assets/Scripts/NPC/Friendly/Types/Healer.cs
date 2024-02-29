using System;
using System.Collections;
using System.Collections.Generic;
using HealthSystem;
using Interactions;
using InventorySystem;
using NPC.Friendly;
using Player;
using UnityEngine;

public class Healer : MonoBehaviour, INpcDialogueAction
{
    private HealthController m_PlayerHealthController;
    private EquipmentController m_PlayerEquipmentController;
    public string interactionText = "Hello, would you like to be healed for a small price of 100 coins?";
    private int m_HealCost = 100;
    private void Start()
    {
        m_PlayerHealthController = PlayerController.Instance.healthController;
        m_PlayerEquipmentController = PlayerController.Instance.equipmentController;
    }

    public void AddDialogueActions()
    {
        // CanvasController.Instance.dialogueCanvasController.SetHealOption(this);


        DialogueAction dialogueAction = new()
        {
            action = () =>
            {
                PlayerController.Instance.equipmentController.SetMoney(
                    PlayerController.Instance.equipmentController.GetMoney() - m_HealCost);
                PlayerController.Instance.healthController.HealAll();
            },
            interactable = () => PlayerController.Instance.equipmentController.GetMoney() >= m_HealCost,
            name = "Heal"
        };

        CanvasController.Instance.dialogueCanvasController.AddButton(dialogueAction);
    }
}