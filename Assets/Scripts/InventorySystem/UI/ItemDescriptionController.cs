using System;
using System.Collections.Generic;
using System.Linq;
using InventorySystem.Items;
using Skills;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace InventorySystem.UI
{
    public class ItemDescriptionController : MonoBehaviour
    {
        [FormerlySerializedAs("m_itemName")] [SerializeField] private TextMeshProUGUI m_ItemName;
        [FormerlySerializedAs("m_itemIcon")] [SerializeField] private Image m_ItemIcon;
        [FormerlySerializedAs("m_itemAmountObject")] [SerializeField] private GameObject m_ItemAmountObject;
        [FormerlySerializedAs("m_itemAmount")] [SerializeField] private TextMeshProUGUI m_ItemAmount;
        [FormerlySerializedAs("m_itemWeight")] [SerializeField] private TextMeshProUGUI m_ItemWeight;
        [FormerlySerializedAs("m_itemLvlObject")] [SerializeField] private GameObject m_ItemLvlObject;
        [FormerlySerializedAs("m_itemLvl")] [SerializeField] private TextMeshProUGUI m_ItemLvl;
        [FormerlySerializedAs("m_itemBuyPrice")] [SerializeField] private TextMeshProUGUI m_ItemBuyPrice;
        [FormerlySerializedAs("m_itemSellPrice")] [SerializeField] private TextMeshProUGUI m_ItemSellPrice;

        [FormerlySerializedAs("m_itemDescriptionPanel")] [SerializeField] private GameObject m_ItemDescriptionPanel;
        [SerializeField] private TextMeshProUGUI m_ItemDescriptionText;
        
        [FormerlySerializedAs("m_itemStatsPanel")] [SerializeField] private GameObject m_ItemStatsPanel;

        [SerializeField] private GameObject m_DamageObject;
        [SerializeField] private TextMeshProUGUI m_DamageValue;
        [SerializeField] private GameObject m_MagazineObject;
        [SerializeField] private TextMeshProUGUI m_MagazineValue;
        [SerializeField] private Image m_MagazineIcon;
        [SerializeField] private GameObject m_AttackSpeedObject;
        [SerializeField] private TextMeshProUGUI m_AttackSpeedValue;
        [SerializeField] private GameObject m_AccuracyObject;
        [SerializeField] private TextMeshProUGUI m_AccuracyValue;
        [SerializeField] private GameObject m_ArmorPointsObject;
        [SerializeField] private TextMeshProUGUI m_ArmorPointsValue;

        [SerializeField] private GameObject m_SkillsObject;
        [SerializeField] private Button m_SkillQButton;
        [SerializeField] private Button m_SkillEButton;
        
        [SerializeField] private GameObject m_SkillDescriptionObject;
        [SerializeField] private TextMeshProUGUI m_SkillName;
        [SerializeField] private TextMeshProUGUI m_SkillDescription;

        public void ShowDescription(IItem item, int itemQuantity)
        {
            m_ItemName.text = item.ItemName;
            m_ItemIcon.sprite = item.Icon;
            m_ItemWeight.text = $"Weight: {item.Weight}";
            m_ItemBuyPrice.text = $"Buy price: {item.BuyPrice}";
            m_ItemSellPrice.text = $"Sell price: {item.SellPrice}";

            HideAllAdditionalProperties();

            if (item is UniqueItemInstance uniqueItem)
            {
                ShowUniqueItem(uniqueItem);
            }
            else if(item is StackableItem stackableItem)
            {
                ShowStackableItemProperties(stackableItem, itemQuantity);
            }
        }

        private void HideAllAdditionalProperties()
        {
            m_ItemLvlObject.SetActive(false);
            m_ItemDescriptionPanel.SetActive(false);
            m_ItemStatsPanel.SetActive(false);

            m_ItemAmount.text = "";
            
            m_DamageObject.SetActive(false);
            m_MagazineObject.SetActive(false);
            m_AttackSpeedObject.SetActive(false);
            m_AccuracyObject.SetActive(false);
            m_ArmorPointsObject.SetActive(false);
            
            m_SkillsObject.SetActive(false);
            m_SkillDescriptionObject.SetActive(false);
            m_SkillEButton.gameObject.SetActive(false);
            m_SkillQButton.gameObject.SetActive(false);
        }

        private void ShowStackableItemProperties(StackableItem item, int itemQuantity)
        {
            m_ItemAmountObject.SetActive(true);
            m_ItemAmount.text = $"Amount: {itemQuantity} ";

            m_ItemDescriptionPanel.SetActive(true);
            
            m_ItemDescriptionText.text = item.Description;
            LayoutRebuilder.ForceRebuildLayoutImmediate(m_ItemDescriptionPanel.GetComponent<RectTransform>());
            
            //m_ItemDescriptionText.ForceMeshUpdate(true);
        }
        
        private void ShowUniqueItem(UniqueItemInstance item)
        {
            m_ItemLvlObject.SetActive(true);
            m_ItemLvl.text = $"ILVL: {item.ItemLvl}";
            
            m_ItemAmountObject.SetActive(false);
            
            m_ItemStatsPanel.SetActive(true);

            if (item is WeaponInstance weapon)
            {
                m_DamageObject.SetActive(true);
                
                if (weapon.GetWeapon().ammunitionType == AmmunitionType.SHOTGUN)
                {
                    m_DamageValue.text = $"Damage: {weapon.damage} ( Max: {weapon.damage * 4})";
                }
                else
                {
                    m_DamageValue.text = $"Damage: {weapon.damage}";
                }
                
                m_MagazineObject.SetActive(true);
                m_MagazineValue.text = $"Capacity: {weapon.GetWeaponMagazineCapacity()}";
                m_MagazineIcon.sprite = weapon.GetWeapon().ammunition.Icon;
                
                m_AttackSpeedObject.SetActive(true);
                m_AttackSpeedValue.text = $"Attack Speed: {weapon.GetWeapon().attackSpeed}";
                
                m_AccuracyObject.SetActive(true);
                
                m_AccuracyValue.text = $"Accuracy: {weapon.accuracy.ToString("0.00")}";

                ShowSkills(weapon);
            }
            else if (item is ArmorInstance armor)
            {
                m_ArmorPointsObject.SetActive(true);
                m_ArmorPointsValue.text = $"Armor: {armor.armorPoints}";
            }
        }

        private void ShowSkills(UniqueItemInstance instance)
        {
            if (instance is WeaponInstance weaponInstance)
            {
                int numberOfSkill = weaponInstance.skillSlots.Count(x => x != null);
                switch (numberOfSkill)
                {
                    case 1:
                        if (weaponInstance.skillSlots[0] != null)
                        {
                            ShowQSkill(weaponInstance.skillSlots[0].skill);
                        }
                        else
                        {
                            ShowESkill(weaponInstance.skillSlots[1].skill);
                        }
                        break;
                    
                    case 2:
                        ShowQSkill(weaponInstance.skillSlots[0].skill);
                        ShowESkill(weaponInstance.skillSlots[1].skill);
                        break;
                    
                    default:
                        m_SkillsObject.SetActive(false);
                        m_SkillDescriptionObject.SetActive(false);
                        break;
                }
            }
            else if (instance is ArmorInstance armorInstance)
            {
                
            }
        }

        private void ShowQSkill(ActivatedSkill skill)
        {
            m_SkillsObject.SetActive(true);
            m_SkillQButton.gameObject.SetActive(true);
            if (skill != null)
            {
                m_SkillQButton.image.sprite = skill.Icon;
                m_SkillQButton.onClick.AddListener(() =>
                {
                    ShowSkillDescription(skill);
                });
            }
        }

        private void ShowESkill(ActivatedSkill skill)
        {
            m_SkillsObject.SetActive(true);
            m_SkillEButton.gameObject.SetActive(true);
            if (skill != null)
            {
                m_SkillEButton.image.sprite = skill.Icon;
                m_SkillEButton.onClick.AddListener(() =>
                {
                    ShowSkillDescription(skill);
                });
            }
        }

        private void ShowSkillDescription(ActivatedSkill skill)
        {
            m_SkillDescriptionObject.SetActive(true);
            m_SkillName.text = skill.ItemName;
            m_SkillDescription.text = skill.Description;
            
            LayoutRebuilder.ForceRebuildLayoutImmediate(m_SkillDescriptionObject.GetComponent<RectTransform>());
        }
        
        private void OnEnable()
        {
            if (!CanvasController.Instance.m_OpenedInterfaces.Contains(gameObject))
            {
                CanvasController.Instance.m_OpenedInterfaces.Push(gameObject);
            }
        }

        private void OnValidate()
        {
            //Debug.Log(gameObject.name + " " + transform.parent.gameObject.name);
            if (!m_ItemLvl) m_ItemLvl = m_ItemLvlObject.GetComponentInChildren<TextMeshProUGUI>();
            if (!m_ItemAmount) m_ItemAmount = m_ItemAmountObject.GetComponentInChildren<TextMeshProUGUI>();
            if (!m_ItemDescriptionText) m_ItemDescriptionText = m_ItemDescriptionPanel.GetComponentInChildren<TextMeshProUGUI>();
            if (!m_DamageValue) m_DamageValue = m_DamageObject.GetComponentInChildren<TextMeshProUGUI>();
            if (!m_MagazineValue) m_MagazineValue = m_MagazineObject.GetComponentInChildren<TextMeshProUGUI>();
            if (!m_MagazineIcon) m_MagazineIcon = m_MagazineObject.GetComponentInChildren<Image>();
            if (!m_AttackSpeedValue) m_AttackSpeedValue = m_AttackSpeedObject.GetComponentInChildren<TextMeshProUGUI>();
            if (!m_AccuracyValue) m_AccuracyValue = m_AccuracyObject.GetComponentInChildren<TextMeshProUGUI>();
            if (!m_ArmorPointsValue) m_ArmorPointsValue = m_ArmorPointsObject.GetComponentInChildren<TextMeshProUGUI>();
        }
    }
}