using System.Collections.Generic;
using InventorySystem.Items;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace InventorySystem.UI
{
    public class CanvasAmmoController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_AmmoCounterText;
        [SerializeField] private GameObject m_AmmoContainer;

        [SerializeField] private GameObject m_AmmoPrefab;

        private List<Image> m_BulletsIconsList = new List<Image>();

        private Color m_NormalBulletColor = new Color32(255, 255, 255, 255);
        private Color m_UsedBulletColor = new Color32(32, 32, 32, 255);

        public void SetAmmoCanvas(WeaponInstance weapon)
        {
            if (weapon != null)
            {
                m_AmmoCounterText.transform.parent.gameObject.SetActive(true);
                m_AmmoContainer.SetActive(true);
                int capacity = weapon.GetWeaponMagazineCapacity();

                m_AmmoCounterText.text = $"{weapon.currentAmmo} / {capacity}";

                for (int i = m_BulletsIconsList.Count; i < capacity; i++)
                {
                    m_BulletsIconsList.Add(Instantiate(m_AmmoPrefab, m_AmmoContainer.transform).GetComponent<Image>());
                }

                int bulletListSize = m_BulletsIconsList.Count;
                for (int i = 0; i < bulletListSize; i++)
                {
                    if (i < capacity)
                    {
                        m_BulletsIconsList[i].gameObject.SetActive(true);
                        m_BulletsIconsList[i].color = i < weapon.currentAmmo ? m_NormalBulletColor : m_UsedBulletColor;
                    }
                    else
                    {
                        m_BulletsIconsList[i].gameObject.SetActive(false);
                    }
                }

                float spacing = -1 * capacity * 1.45f;
                spacing = Mathf.Clamp(spacing, -40, -30);
                m_AmmoContainer.GetComponent<HorizontalLayoutGroup>().spacing = spacing;
            }
            else
            {
                m_AmmoCounterText.transform.parent.gameObject.SetActive(false);
                m_AmmoContainer.SetActive(false);
            }
        }

        public void ReloadAmmoCanvas(WeaponInstance weapon)
        {
            m_AmmoCounterText.text = weapon.currentAmmo + " / " + weapon.GetWeaponMagazineCapacity();
            for (int i = 0; i < weapon.currentAmmo; i++)
            {
                m_BulletsIconsList[i].color = m_NormalBulletColor;
            }
        }

        public void DecreaseAmmoCanvas(WeaponInstance weapon)
        {
            m_AmmoCounterText.text = weapon.currentAmmo + " / " + weapon.GetWeaponMagazineCapacity();
            m_BulletsIconsList[weapon.currentAmmo].color = m_UsedBulletColor;
        }
    }
}
