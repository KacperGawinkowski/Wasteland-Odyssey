using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using InventorySystem.Items;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GunListEntry : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private IItem m_Item;
    
    [SerializeField] private GameObject m_Selection;
    [SerializeField] private TextMeshProUGUI m_ItemName;
    [SerializeField] private TextMeshProUGUI m_ItemLvl;
    [SerializeField] private Image m_ItemIcon;
    [SerializeField] private GameObject m_EquippedIcon;
    [SerializeField] private TextMeshProUGUI m_EquippedSlot;


    public Button UpgradeItemButton;
    public void SetData(IItem item)
    {
        m_Item = item;
        m_ItemName.text = m_Item.ItemName;
        if (m_Item is UniqueItemInstance instance)
        {
            m_ItemLvl.text = $"ILVL: {instance.ItemLvl}";
            if (PlayerController.Instance.equipmentController.CheckIfItemIsEquipped(item))
            {
                m_EquippedIcon.SetActive(true);
                if (instance is WeaponInstance weaponInstance)
                {
                    m_EquippedSlot.text = $"EQ{(weaponInstance.GetWeaponTypeInt() + 1)}";
                }
            }
        }
        m_ItemIcon.sprite = m_Item.Icon;
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
