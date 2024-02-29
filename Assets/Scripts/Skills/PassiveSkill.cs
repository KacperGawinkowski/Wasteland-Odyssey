using UnityEngine;

namespace Skills
{
    [CreateAssetMenu(fileName = nameof(PassiveSkill), menuName = "Skills/" + nameof(PassiveSkill))]
    public class PassiveSkill : Skill
    {
        public float movement;
        public float accuracy;
        
    }
}
