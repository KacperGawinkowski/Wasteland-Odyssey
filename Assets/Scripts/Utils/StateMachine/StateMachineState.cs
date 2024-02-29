using UnityEngine;

namespace Utils.StateMachine
{
    public interface IStateMachineState<in T> where T : MonoBehaviour
    {
        public void OnEnter(T monoBehaviour);
        public IStateMachineState<T> OnFixedUpdate(T monoBehaviour);
        // public IStateMachineState<T> OnUpdate(T monoBehaviour);
        public void OnExit(T monoBehaviour);
    }
}