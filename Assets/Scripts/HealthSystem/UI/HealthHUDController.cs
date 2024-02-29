using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace HealthSystem.UI
{
    public class HealthHUDController : MonoBehaviour
    {
        [SerializeField] private GameObject healthHUD;
        [SerializeField] private Gradient healthGradient;

        // [SerializeField] private Image[] m_HeadImage;
        // [SerializeField] private Image[] m_StomachImage;
        // [SerializeField] private Image[] m_LeftArmImage;
        // [SerializeField] private Image[] m_RightArmImage;
        // [SerializeField] private Image[] m_LeftLegImage;
        // [SerializeField] private Image[] m_RightLegImage;

        [SerializeField] private CharacterHealthSkeleton<Image[]> m_BodyPartsImages;

        // private Image[][] m_BodyPartsImages;

        // private void Awake()
        // {
        //     // m_BodyPartsImages = new Image[][] { m_HeadImage, m_StomachImage, m_LeftArmImage, m_RightArmImage, m_LeftLegImage, m_RightLegImage };
        // }

        public void ActivateHUD(bool val)
        {
            healthHUD.SetActive(val);
        }

        public void UpdateHealthHUD(HealthController healthController)
        {
            ActivateHUD(healthController.currentHp.Any(bodyPart => bodyPart.Item2 < healthController.bodyPartPresets[bodyPart.Item1].maxHealth));

            foreach (CharacterBodyPart bodyPart in (CharacterBodyPart[])Enum.GetValues(typeof(CharacterBodyPart)))
            {
                float healthPercentage = (float)healthController.currentHp[bodyPart] / (float)healthController.bodyPartPresets[bodyPart].maxHealth;
                foreach (Image image in m_BodyPartsImages[bodyPart])
                {
                    SetBodyPartColorGUI(image, healthPercentage);
                }
            }
        }

        private void SetBodyPartColorGUI(Image image, float healthPercentage)
        {
            image.color = healthGradient.Evaluate(healthPercentage);
        }
    }
}