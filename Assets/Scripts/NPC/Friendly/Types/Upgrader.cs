using System;
using NPC.Friendly;
using UnityEngine;

namespace NPC
{
    public class Upgrader : MonoBehaviour, INpcDialogueAction
    {
        public void AddDialogueActions()
        {
            // CanvasController.Instance.dialogueCanvasController.SetUpgradeOption(this);

            DialogueAction dialogueAction = new()
            {
                action = () =>
                {
                    CanvasController.Instance.upgradePanel.SetActive(true);
                    CanvasController.Instance.questChoosePanel.SetActive(false);
                    CanvasController.Instance.inventoryPanel.SetActive(false);
                    CanvasController.Instance.upgradePanelController.ShowItemInstancesInEquipment();
                },
                interactable = () => true,
                name = "Upgrade"
            };

            CanvasController.Instance.dialogueCanvasController.AddButton(dialogueAction);
        }
    }
}