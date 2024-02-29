using System;
using Player;
using Skills;
using TMPro;
using UnityEngine;

namespace HealthSystem.UI
{
    public class AdvancedHealthInterfaceUIController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_BodyPartDebuffs;
        [SerializeField] private HealthController playerHealthController;

        public void SetBodyPartDebuffsText()
        {
            m_BodyPartDebuffs.text = "";
            
            float moveSpeedDecrease = 0;
            float accuracyDecrease = 0;
            foreach (PassiveSkill debuff in playerHealthController.debuffs)
            {
                moveSpeedDecrease += debuff.movement;
                accuracyDecrease += debuff.accuracy;
            }

            if (accuracyDecrease != 0)
            {
                m_BodyPartDebuffs.text = $"{playerHealthController.bodyPartPresets.leftArm.debuff.description} - {accuracyDecrease * 100}% \n";
            }
            
            if (moveSpeedDecrease != 0)
            {
                m_BodyPartDebuffs.text += $"{playerHealthController.bodyPartPresets.leftLeg.debuff.description} - {moveSpeedDecrease*100}%";
            }
            

            
            // m_BodyPartDebuffs.text ="";
            // foreach (BodyPart bodyPart in playerHealthController.bodyParts)
            // {
            //     if (bodyPart.health <= 0)
            //     {
            //         if (bodyPart.bodyPart.debuff != null)
            //         {
            //             if (m_BodyPartDebuffs.text.Contains(bodyPart.bodyPart.debuff.itemName))
            //             {
            //                 string str = $"{bodyPart.bodyPart.debuff.description} - {} \n {bodyPart.bodyPart.debuff.description} - {}";
            //                 m_BodyPartDebuffs.text = m_BodyPartDebuffs.text.Replace(bodyPart.bodyPart.debuff.itemName,str);
            //             }
            //             else
            //             {
            //                 m_BodyPartDebuffs.text += bodyPart.bodyPart.debuff.itemName + "\n";
            //             }
            //         }
            //     }
            // }
        }
        
        private void OnEnable()
        {
            if (!CanvasController.Instance.m_OpenedInterfaces.Contains(gameObject))
            {
                CanvasController.Instance.m_OpenedInterfaces.Push(gameObject);
            }
        }
    }
}
