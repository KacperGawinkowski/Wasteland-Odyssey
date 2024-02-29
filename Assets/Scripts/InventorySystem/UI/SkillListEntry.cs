using System;
using System.Collections;
using System.Collections.Generic;
using InventorySystem;
using InventorySystem.Items;
using UnityEngine.EventSystems;
using Player;
using Skills;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillListEntry : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject m_Selection;
    [SerializeField] private Image m_SkillIcon;
    [SerializeField] private TextMeshProUGUI m_SkillName;

    public Button SetSkillInSlotButton;

    private ActivatedSkill m_Skill;
    private UniqueItemInstance m_ItemInstance;
    public void SetData(ActivatedSkill skill, int slot, UniqueItemInstance item)
    {
        m_ItemInstance = item;
        m_Skill = skill;
        m_SkillIcon.sprite = skill.Icon;
        m_SkillName.text = skill.itemName;

        EquipmentController playerEquipment = PlayerController.Instance.equipmentController;
        
        SetSkillInSlotButton.interactable = playerEquipment.GetMoney() >= 1000;
        
        SetSkillInSlotButton.onClick.AddListener(() =>
        {
            if (m_ItemInstance is WeaponInstance weapon)
            {
                weapon.SetSkillSlot(slot,m_Skill);
                playerEquipment.RemoveItem(m_Skill,1);
                playerEquipment.SetMoney(playerEquipment.GetMoney()-1000);
                CanvasController.Instance.upgradePanelController.ClearSkillsListEntries();
                CanvasController.Instance.upgradePanelController.ShowSkillsInEquipment(m_ItemInstance,slot);
                
                CanvasController.Instance.upgradePanelController.upgradeItemInfoPanelController.SetData(weapon);
                CanvasController.Instance.upgradePanelController.upgradeItemInfoPanelController.ShowSkillDescription(slot);
                CanvasController.Instance.upgradePanelController.m_MoneyText.text = playerEquipment.GetMoney().ToString();
            }
        });
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        m_Selection.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        m_Selection.SetActive(false);
    }

    private void OnDisable()
    {
        m_Selection.SetActive(false);
    }
}
