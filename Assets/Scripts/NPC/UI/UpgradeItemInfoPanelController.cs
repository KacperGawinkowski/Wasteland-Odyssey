using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using InventorySystem;
using InventorySystem.Items;
using Player;
using Skills;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeItemInfoPanelController : MonoBehaviour
{
    private UniqueItemInstance m_Instance;

    [SerializeField] private Image m_ItemIcon;
    [SerializeField] private TextMeshProUGUI m_ItemName;
    [SerializeField] private TextMeshProUGUI m_ItemLvl;
    
    [SerializeField] private GameObject m_DamageObject;
    [SerializeField] private TextMeshProUGUI m_DamageValue;
    [SerializeField] private GameObject m_MagazineObject;
    [SerializeField] private TextMeshProUGUI m_MagazineValue;
    [SerializeField] private GameObject m_AttackSpeedObject;
    [SerializeField] private TextMeshProUGUI m_AttackSpeedValue;
    [SerializeField] private GameObject m_AccuracyObject;
    [SerializeField] private TextMeshProUGUI m_AccuracyValue;
    
    [SerializeField] private GameObject m_ArmorPointsObject;
    [SerializeField] private TextMeshProUGUI m_ArmorPointsValue;

    [SerializeField] private Button m_UpgradeButton;
    [SerializeField] private TextMeshProUGUI m_UpgradeCostText;
    [SerializeField] private Button m_DestroyButton;

    [SerializeField] private GameObject m_WeaponSkillsPanel;
    
    [SerializeField] private Button m_SkillQButton;
    [SerializeField] private Button m_SkillEButton;

    [SerializeField] private GameObject m_SkillDescriptionObject;
    [SerializeField] private TextMeshProUGUI m_SkillName;
    [SerializeField] private TextMeshProUGUI m_SkillDescription;

    [SerializeField] private Sprite m_EmptySkillSlotIcon;
    
    public void SetData(UniqueItemInstance item)
    {
        if (item != null)
        {
            ClearButtonListeners();
            m_Instance = item;
            
            m_ItemIcon.sprite = m_Instance.Icon;
            m_ItemName.text = m_Instance.ItemName;
            m_ItemLvl.text = $"ILVL: {item.ItemLvl}";
            //DisplayProperItemDescriptionFields(m_Instance);
            SetItemDescription(m_Instance);

            SetButtonListeners();
        }
    }

    private void SetItemDescription(UniqueItemInstance instance)
    {
        m_SkillQButton.image.sprite = m_EmptySkillSlotIcon;
        m_SkillEButton.image.sprite = m_EmptySkillSlotIcon;

        bool isItemWeapon = instance is WeaponInstance;
        
        m_DamageObject.SetActive(isItemWeapon);
        m_MagazineObject.SetActive(isItemWeapon);
        m_AttackSpeedObject.SetActive(isItemWeapon);
        m_AccuracyObject.SetActive(isItemWeapon);
        m_ArmorPointsObject.SetActive(!isItemWeapon);
        m_WeaponSkillsPanel.SetActive(isItemWeapon);
        m_SkillQButton.gameObject.SetActive(isItemWeapon);
        m_SkillEButton.gameObject.SetActive(isItemWeapon);
        
        if (instance is WeaponInstance weapon)
        {
            m_DamageValue.text = $"Damage: {weapon.damage}";
            m_MagazineValue.text = $"Capacity: {weapon.GetWeaponMagazineCapacity()}";
            m_AttackSpeedValue.text = $"Attack Speed: {weapon.GetWeapon().attackSpeed}";
            m_AccuracyValue.text = $"Accuracy: {weapon.accuracy.ToString("0.00")}";
            
            if(weapon.skillSlots.Count(x => x != null) > 0)
            {
                m_DestroyButton.gameObject.SetActive(true);
                m_SkillQButton.image.sprite = weapon.skillSlots[0]?.skill != null ? weapon.skillSlots[0].skill.Icon : m_EmptySkillSlotIcon;
                m_SkillEButton.image.sprite = weapon.skillSlots[1]?.skill != null ? weapon.skillSlots[1].skill.Icon : m_EmptySkillSlotIcon;
            }
            else
            {
                m_DestroyButton.gameObject.SetActive(false);
                m_DestroyButton.gameObject.SetActive(true);
            }
        }
        else if (instance is ArmorInstance armor)
        {
            m_ArmorPointsValue.text = $"Armor: {armor.armorPoints}";
        }
    }

    private void ClearButtonListeners()
    {
        m_UpgradeButton.onClick.RemoveAllListeners();
        m_DestroyButton.onClick.RemoveAllListeners();
        m_SkillQButton.onClick.RemoveAllListeners();
        m_SkillEButton.onClick.RemoveAllListeners();
    }

    private void SetButtonListeners()
    {
        SetUpgradeButtonListener();
        SetDestroyButtonListener();
        SetSkillButtonListeners();
    }

    private void SetUpgradeButtonListener()
    {
        int currentIlvL = m_Instance.ItemLvl;
        int costOfUpgrade = m_Instance.GetUpgradeCost();

        if (m_Instance.ItemLvl >= UniqueItem.s_MaxItemLevel)
        {
            m_UpgradeCostText.text = "Max Ilvl";
            m_UpgradeButton.interactable = false;
            return;
        }
        
        m_UpgradeCostText.text = $"Upgrade ${costOfUpgrade}";
        if (PlayerController.Instance.equipmentController.GetMoney() >= costOfUpgrade)
        {
            m_UpgradeButton.interactable = true;
            m_UpgradeButton.onClick.AddListener(() =>
            {
                if (PlayerController.Instance.equipmentController.GetMoney() >= costOfUpgrade)
                {
                    m_Instance.SetItemLvl(currentIlvL+1);
                    PlayerController.Instance.equipmentController.SetMoney(PlayerController.Instance.equipmentController.GetMoney() - costOfUpgrade);
                    
                    CanvasController.Instance.upgradePanelController.ShowItemInstancesInEquipment();
                    SetData(m_Instance);
                }
            });
        }
        else
        {
            m_UpgradeButton.interactable = false;
        }
    }

    private void SetDestroyButtonListener()
    {
        m_DestroyButton.onClick.AddListener(() =>
        {
            SkillSlot[] skillSlots = new SkillSlot[2];
            
            if (m_Instance is WeaponInstance weaponInstance)
            {
                skillSlots = weaponInstance.skillSlots;
            }

            PlayerController.Instance.equipmentController.RemoveItem(m_Instance,1);
            PlayerController.Instance.equipmentController.HideWeapon();
            foreach (SkillSlot skillSlot in skillSlots)
            {
                if (skillSlot != null)
                {
                    PlayerController.Instance.equipmentController.AddItem(skillSlot.skill,1);
                }
            }
            CanvasController.Instance.upgradePanelController.ClearItemListEntries();
            CanvasController.Instance.upgradePanelController.ClearSkillsListEntries();
            CanvasController.Instance.upgradePanelController.ShowItemInstancesInEquipment();
            gameObject.SetActive(false);
        });
    }

    private void SetSkillButtonListeners()
    {
        m_SkillQButton.onClick.AddListener(() =>
        {
            CanvasController.Instance.upgradePanelController.ShowSkillsInEquipment(m_Instance, 0);

            HideSkillDescriptions();
            ShowSkillDescription(0);
        });
        m_SkillEButton.onClick.AddListener(() =>
        {
            CanvasController.Instance.upgradePanelController.ShowSkillsInEquipment(m_Instance, 1);

            HideSkillDescriptions();
            ShowSkillDescription(1);
        });
    }

    public void ShowSkillDescription(int slot)
    {
        if (m_Instance is WeaponInstance weaponInstance)
        {
            if (weaponInstance.skillSlots[slot]?.skill != null)
            {
                m_SkillDescriptionObject.SetActive(true);
                m_SkillName.text = weaponInstance.skillSlots[slot].skill.ItemName;
                m_SkillDescription.text = weaponInstance.skillSlots[slot].skill.Description;

                if (slot == 0)
                {
                    m_SkillQButton.image.sprite = weaponInstance.skillSlots[slot].skill.Icon;
                    m_SkillQButton.image.color = Color.white;
                }
                else
                {
                    m_SkillEButton.image.sprite = weaponInstance.skillSlots[slot].skill.Icon;
                    m_SkillEButton.image.color = Color.white;
                }
            }
            else
            {
                if (slot == 0)
                {
                    m_SkillQButton.image.color = Color.white;
                }
                else
                {
                    m_SkillEButton.image.color = Color.white;
                }
            }
        }
    }

    public void HideSkillDescriptions()
    {
        m_SkillDescriptionObject.SetActive(false);
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
        CanvasController.Instance.upgradePanelController.m_SkillsPanel.SetActive(false);
    }

    private void OnValidate()
    {
        //Debug.Log(gameObject.name + " " + transform.parent.gameObject.name);
        if (!m_DamageValue) m_DamageValue = m_DamageObject.GetComponentInChildren<TextMeshProUGUI>();
        if (!m_MagazineValue) m_MagazineValue = m_MagazineObject.GetComponentInChildren<TextMeshProUGUI>();
        if (!m_AttackSpeedValue) m_AttackSpeedValue = m_AttackSpeedObject.GetComponentInChildren<TextMeshProUGUI>();
        if (!m_AccuracyValue) m_AccuracyValue = m_AccuracyObject.GetComponentInChildren<TextMeshProUGUI>();
        if (!m_ArmorPointsValue) m_ArmorPointsValue = m_ArmorPointsObject.GetComponentInChildren<TextMeshProUGUI>();
    }
}
