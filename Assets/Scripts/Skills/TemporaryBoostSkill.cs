using Player;

namespace Skills
{
    public abstract class TemporaryBoostSkill : ActivatedSkill
    {
        public float duration;

        public abstract void ActivateSkill(PlayerControllerWSAD player);
        public abstract void DeactivateSkill(PlayerControllerWSAD player);
    }
}
