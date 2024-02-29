using UnityEngine;

namespace Utils.StateMachine
{
    public struct StateMachine<T> where T : MonoBehaviour
    {
        private IStateMachineState<T> m_CurrentState;
        private T m_MonoBehaviour;

        public void FixedUpdate()
        {
            if (m_CurrentState != null)
            {
                IStateMachineState<T> nextState = m_CurrentState.OnFixedUpdate(m_MonoBehaviour);
                if (nextState != m_CurrentState)
                {
                    m_CurrentState.OnExit(m_MonoBehaviour);
                    m_CurrentState = nextState;
                    m_CurrentState.OnEnter(m_MonoBehaviour);
                }
            }
        }

        // public void Update()
        // {
        //     if (m_CurrentState != null)
        //     {
        //         IStateMachineState<T> nextState = m_CurrentState.OnUpdate(m_MonoBehaviour);
        //         if (nextState != m_CurrentState)
        //         {
        //             m_CurrentState.OnExit(m_MonoBehaviour);
        //             m_CurrentState = nextState;
        //             m_CurrentState.OnEnter(m_MonoBehaviour);
        //         }
        //     }
        // }

        public void Start(T monoBehaviour, IStateMachineState<T> startingState)
        {
            m_MonoBehaviour = monoBehaviour;
            m_CurrentState = startingState;

            m_CurrentState.OnEnter(m_MonoBehaviour);
        }

        public void Stop()
        {
            m_CurrentState.OnExit(m_MonoBehaviour);
        }
    }
}