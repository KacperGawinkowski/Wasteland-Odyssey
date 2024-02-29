using System;
using System.Collections;
using System.Collections.Generic;
using InventorySystem;
using InventorySystem.Items;
using InventorySystem.UI;
using NPC;
using Player;
using Skills;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class UpgradePanelController : MonoBehaviour
{
    [FormerlySerializedAs("upgradeGunInfoPanelController")] public UpgradeItemInfoPanelController upgradeItemInfoPanelController;

    [SerializeField] private GameObject m_GunsPanel;
    [SerializeField] private GameObject m_GunListEntryPrefab;
    
    public GameObject m_SkillsPanel;
    [SerializeField] private GameObject m_SkillsPanelContent;
    [SerializeField] private GameObject m_SkillListEntryPrefab;
    public TextMeshProUGUI m_MoneyText;

    public void ShowItemInstancesInEquipment()
    {
        EquipmentController playerEquipment = PlayerController.Instance.equipmentController;
        m_MoneyText.text = playerEquipment.GetMoney().ToString();

        ClearItemListEntries();
        foreach (var item in playerEquipment.InventoryDictionary)
        {
            if (item.Key is WeaponInstance or ArmorInstance)
            {
                GunListEntry listEntry = Instantiate(m_GunListEntryPrefab, m_GunsPanel.transform).GetComponent<GunListEntry>();
                listEntry.UpgradeItemButton.onClick.AddListener(() =>
                {
                    upgradeItemInfoPanelController.gameObject.SetActive(true);
                    upgradeItemInfoPanelController.SetData((UniqueItemInstance)item.Key);
                    upgradeItemInfoPanelController.HideSkillDescriptions();
                    m_SkillsPanel.SetActive(false);
                    ClearSkillsListEntries();
                });
                listEntry.SetData((UniqueItemInstance)item.Key);
            }
        }
    }

    public void ClearItemListEntries()
    {
        foreach (Transform child in m_GunsPanel.transform)
        {
            Destroy(child.gameObject);
        }
    }
    public void ClearSkillsListEntries()
    {
        foreach (Transform child in m_SkillsPanelContent.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void ShowSkillsInEquipment(UniqueItemInstance itemInstance, int slot)
    {
        m_SkillsPanel.SetActive(true);
        EquipmentController playerEquipment = PlayerController.Instance.equipmentController;
        
        ClearSkillsListEntries();
        if (itemInstance is WeaponInstance weaponInstance)
        {
            foreach (var item in playerEquipment.InventoryDictionary)
            {
                if (item.Key is ActivatedSkill activatedSkill)
                {
                    if (weaponInstance.skillSlots[0]?.skill != activatedSkill && weaponInstance.skillSlots[1]?.skill != activatedSkill)
                    {
                        SkillListEntry listEntry = Instantiate(m_SkillListEntryPrefab, m_SkillsPanelContent.transform).GetComponent<SkillListEntry>();
                        listEntry.SetData(activatedSkill,slot, itemInstance);
                    }
                }
            }
        }
    }
    
    private void OnEnable()
    {
        if (!CanvasController.Instance.m_OpenedInterfaces.Contains(gameObject))
        {
            CanvasController.Instance.m_OpenedInterfaces.Push(gameObject);
        }
    }

    private void OnDisable()
    {
        m_SkillsPanel.SetActive(false);
        upgradeItemInfoPanelController.HideSkillDescriptions();
    }
}
