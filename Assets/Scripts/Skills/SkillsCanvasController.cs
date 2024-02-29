using System.Collections;
using System.Collections.Generic;
using InventorySystem.Items;
using Player;
using UnityEngine;
using UnityEngine.UI;

public class SkillsCanvasController : MonoBehaviour
{
    [SerializeField] private Image m_SkillQ;
    [SerializeField] private Image m_SkillQCooldown;
    [SerializeField] private Image m_SkillE;
    [SerializeField] private Image m_SkillECooldown;

    private PlayerControllerWSAD m_PlayerController;

    public void SetSkillsInterface(WeaponInstance instance)
    {
        if (instance?.skillSlots[0]?.skill != null)
        {
            m_SkillQ.gameObject.SetActive(true);
            m_SkillQCooldown.gameObject.SetActive(true);
            m_SkillQ.sprite = instance.skillSlots[0].skill.Icon;
            m_SkillQCooldown.sprite = instance.skillSlots[0].skill.Icon;
        }
        else
        {
            m_SkillQ.gameObject.SetActive(false);
            m_SkillQCooldown.gameObject.SetActive(false);
        }
        
        if (instance?.skillSlots[1]?.skill != null)
        {
            m_SkillE.gameObject.SetActive(true);
            m_SkillECooldown.gameObject.SetActive(true);
            m_SkillE.sprite = instance.skillSlots[1].skill.Icon;
            m_SkillECooldown.sprite = instance.skillSlots[1].skill.Icon;
        }
        else
        {
            m_SkillE.gameObject.SetActive(false);
            m_SkillECooldown.gameObject.SetActive(false);
        }
    }

    public void SetCooldown(int id,float percentage)
    {
        switch (id)
        {
            case 0:
                m_SkillQCooldown.fillAmount = 1-percentage;
                break;
            default:
                m_SkillECooldown.fillAmount = 1-percentage;
                break;
        }
    }
    
}
