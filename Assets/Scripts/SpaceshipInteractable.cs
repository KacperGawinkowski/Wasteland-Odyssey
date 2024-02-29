using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HealthSystem;
using Interactions;
using UnityEngine;

public class SpaceshipInteractable : Interactable
{
    public List<EnemyNpc> enemiesAroundTheShip = new List<EnemyNpc>();

    public override void Interact()
    {
        Debug.Log("Game won ez");
        SaveSystem.DeleteSave(SaveSystem.currentSaveName);
        GameLoader.Instance.LoadWinScene();
        //GameLoader.Instance.LoadMainMenu();
    }

    public override bool CanInteract()
    {
        return enemiesAroundTheShip.Count(x => x.GetComponent<HealthController>().currentHp.head > 0) <= 0;
    }
}