using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using TMPro;
using UnityEngine;
using World.TownGeneration;

public class LocationBoundary : MonoBehaviour
{
    public bool hasToKillEnemies = false;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("LocationBoundary"))
        {
            if ((hasToKillEnemies && EnemyNpc.aliveEnemies.Count > 0) || (SaveSystem.saveContent.questLog.storylineQuestsFinished == 0 && StorylineQuestController.s_StorylineQuest.IsQuestObjectiveFulfilled()))
            {
                return;
            }
            
            CanvasController.Instance.enterLeaveInterfaceController.ShowLeaveLocationInterface(PlayerController.Instance.playerControllerGlobal.currentLocation); //leaveLocationButton.gameObject.SetActive(true);
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("LocationBoundary"))
        {
            CanvasController.Instance.enterLeaveInterfaceController.Clear();
        }
    }
}
