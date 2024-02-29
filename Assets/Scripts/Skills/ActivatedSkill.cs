using System;

namespace Skills
{
    [Serializable]
    public abstract class ActivatedSkill : Skill
    {
        public float cooldown;
    }
}