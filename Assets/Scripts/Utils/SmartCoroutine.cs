using System.Collections;
using UnityEngine;

namespace Utils
{
    public struct SmartCoroutine
    {
        private Coroutine m_Coroutine;
        private MonoBehaviour m_MonoBehaviour;

        public void SetMonoBehavior(MonoBehaviour monoBehaviour)
        {
            m_MonoBehaviour = monoBehaviour;
        }

        public void Start(IEnumerator enumerator)
        {
            if (m_Coroutine != null)
            {
                m_MonoBehaviour.StopCoroutine(m_Coroutine);
            }

            m_Coroutine = m_MonoBehaviour.StartCoroutine(enumerator);
        }

        public void Stop()
        {
            if (m_Coroutine != null)
            {
                m_MonoBehaviour.StopCoroutine(m_Coroutine);
                m_Coroutine = null;
            }
        }
    }
}