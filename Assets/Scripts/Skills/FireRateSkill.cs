using Player;
using UnityEngine;

namespace Skills
{
    [CreateAssetMenu(fileName = nameof(FireRateSkill), menuName = "Skills/Active/" + nameof(FireRateSkill))]
    public class FireRateSkill : TemporaryBoostSkill
    {
        [SerializeField] private float m_FireRateBoots = 6;

        public override void ActivateSkill(PlayerControllerWSAD player)
        {
            player.fireRateBoost += m_FireRateBoots;
        }

        public override void DeactivateSkill(PlayerControllerWSAD player)
        {
            player.fireRateBoost -= m_FireRateBoots;
        }
    }
}