using UnityEngine;
using UnityEngine.UI;

namespace Interface
{
    public class LoadingCircleController : MonoBehaviour
    {
        [SerializeField] private LoadingInterfaceController m_Controller;
        public Image image;

        [System.NonSerialized] public float totalTime;
        [System.NonSerialized] public float passedTime;

        // Update is called once per frame
        void Update()
        {
            image.fillAmount = passedTime / totalTime;

            passedTime += Time.deltaTime;

            if (passedTime > totalTime)
            {
                m_Controller.HideCircle();
            }
        }
    }
}
