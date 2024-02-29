using Skills;
using UnityEngine;

namespace HealthSystem
{
    [CreateAssetMenu(fileName = nameof(BodyPartPreset), menuName = "HealthSystem/" + nameof(BodyPartPreset), order = 1)]
    public class BodyPartPreset : ScriptableObject
    {
        public int maxHealth;
        public int weight;

        public PassiveSkill debuff;
    }
}
