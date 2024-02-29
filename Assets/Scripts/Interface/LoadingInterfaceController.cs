using TMPro;
using UnityEngine;

namespace Interface
{
    public class LoadingInterfaceController : MonoBehaviour
    {
        [SerializeField] private LoadingCircleController m_LoadingCircleController;
        [SerializeField] private TextMeshProUGUI m_Text;

        public void StartCircle(float time, string text = null)
        {
            m_LoadingCircleController.totalTime = time;
            m_LoadingCircleController.passedTime = 0;
            m_LoadingCircleController.image.fillAmount = 0;

            m_LoadingCircleController.gameObject.SetActive(true);
            if (!string.IsNullOrEmpty(text))
            {
                m_Text.gameObject.SetActive(true);
                m_Text.text = text;
            }

        }

        public void HideCircle()
        {
            m_LoadingCircleController.gameObject.SetActive(false);
            m_Text.gameObject.SetActive(false);
        }
    }
}
