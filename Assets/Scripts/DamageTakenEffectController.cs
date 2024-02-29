using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class DamageTakenEffectController : MonoBehaviour
{
    [SerializeField] private float m_EffectTimerDuration = 1f;
    private float m_EffectTimer;
    private bool m_IsEffectActive;
    [SerializeField, HideInInspector] private Image m_Image;
    private Color m_DefaultColor;

    private void Start()
    {
        m_DefaultColor = m_Image.color;
    }

    private void Update()
    {
        if (m_IsEffectActive)
        {
            m_EffectTimer -= Time.deltaTime;
            m_Image.color = Color.Lerp(new Color(m_DefaultColor.r, m_DefaultColor.g, m_DefaultColor.b, 0), m_DefaultColor, m_EffectTimer / m_EffectTimerDuration);


            if (m_EffectTimer <= 0f)
            {
                gameObject.SetActive(false);
                m_IsEffectActive = false;
            }
        }
    }

    public void EnableDamageTakenEffect()
    {
        if (!m_IsEffectActive)
        {
            gameObject.SetActive(true);
            m_IsEffectActive = true;
            m_EffectTimer = m_EffectTimerDuration;
        }
        else
        {
            m_EffectTimer = m_EffectTimerDuration;
        }
    }

    private void OnValidate()
    {
        if (m_Image == null) m_Image = GetComponent<Image>();
    }
}