using UnityEngine;

namespace HealthSystem
{
    [CreateAssetMenu(fileName = "HealthPreset", menuName = "ScriptableObjects/HealthPreset", order = 1)]
    public class HealthPreset : ScriptableObject
    {
        public BodyPartPreset[] bodyParts;
    }
}
