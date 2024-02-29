using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using World;

public class RandomEventInterfaceController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_RandomEventEnemyLabel;
    [SerializeField] private TextMeshProUGUI m_RandomEventDescription;
    [SerializeField] private Button m_EnterButton;
    [SerializeField] private Button m_SkipButton;
    
    public void ShowInterface(RandomEventData randomEventData)
    {
        gameObject.SetActive(true);
        m_EnterButton.onClick.RemoveAllListeners();
        m_SkipButton.onClick.RemoveAllListeners();
        Time.timeScale = 0f;

        RandomEventPreset randomEventPreset = randomEventData.randomEventPreset;

        m_RandomEventEnemyLabel.text = randomEventPreset.actorName;
        // if (randomEventPreset.enemiesHaveToBeKilled)
        // {
        //     if (randomEventData.numberOfEnemies > 1)
        //     {
        //         m_RandomEventEnemyLabel.text = "Group of Enemies";
        //     }
        //     else
        //     {
        //         m_RandomEventEnemyLabel.text = "Enemy";
        //     }
        // }
        // else
        // {
        //     
        // }

        m_RandomEventDescription.text = randomEventPreset.eventDescription;
        m_SkipButton.gameObject.SetActive(randomEventPreset.canBeSkipped);

        m_EnterButton.GetComponentInChildren<TextMeshProUGUI>().text = randomEventPreset.fightText;
        m_SkipButton.GetComponentInChildren<TextMeshProUGUI>().text = randomEventPreset.skipText;
        
        if (randomEventPreset.canBeSkipped)
        {
            m_SkipButton.gameObject.SetActive(true);
            
            if (randomEventPreset.skipPrice > 0)
            {
                m_SkipButton.GetComponentInChildren<TextMeshProUGUI>().text = "Pay " + randomEventPreset.skipPrice;
            }

            m_SkipButton.interactable = PlayerController.Instance.equipmentController.GetMoney() >= randomEventPreset.skipPrice;
            m_SkipButton.onClick.AddListener(() =>
            {
                Time.timeScale = 1f;
                PlayerController.Instance.randomEncounterChance = -0.1f;
                PlayerController.Instance.playerControllerGlobal.agent.forceStop = false;
                PlayerController.Instance.equipmentController.SetMoney(PlayerController.Instance.equipmentController.GetMoney() - randomEventPreset.skipPrice);
                
                
                m_SkipButton.gameObject.SetActive(false);
                gameObject.SetActive(false);
            });
            
        }
        
        m_EnterButton.onClick.AddListener(() =>
        {
            Time.timeScale = 1f;
            PlayerController.Instance.randomEncounterChance = -0.1f;
            PlayerController.Instance.playerControllerGlobal.agent.forceStop = false;
            PlayerController.Instance.globalMapPlayer.SetActive(false);
            PlayerController.Instance.playerControllerLocal.locationBoundary.hasToKillEnemies = randomEventData.randomEventPreset.enemiesHaveToBeKilled;

            TimeController.Instance.timeSpeedMultiplier = 1;
            GameLoader.Instance.LoadRandomEvent();
            gameObject.SetActive(false);
        });
    }
}
