using System;
using NPC;
using NPC.Friendly;
using UnityEngine;

namespace Interactions
{
    public class Talkable : Interactable
    {
        public FriendlyNpcController npc;

        public override void Interact()
        {
            base.Interact();

            // if (npc.friendlyNpcData != null && npc.friendlyNpcData?.npcType != NpcType.storylineNpc)
            // if (npc.friendlyNpcData is not { npcType: NpcType.storylineNpc })
            // {
            CanvasController.Instance.dialogueCanvasController.ConfigureExitButton(true);
            CanvasController.Instance.dialogueCanvasController.gameObject.SetActive(true);
            CanvasController.Instance.dialogueCanvasController.PrepareNpcDialog(npc);
            
            // }

            
        }


        private void OnValidate()
        {
            npc = GetComponent<FriendlyNpcController>();
        }
    }
}