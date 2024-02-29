using System.Collections;
using Player;
using UnityEngine;
using Utils;

namespace Skills
{
    [CreateAssetMenu(fileName = nameof(SpeedSkill), menuName = "Skills/Active/" + nameof(SpeedSkill))]
    public class SpeedSkill : TemporaryBoostSkill
    {
        [SerializeField] private float m_SpeedBoost = 6;

        public override void ActivateSkill(PlayerControllerWSAD player)
        {
            player.moveSpeedBoost += m_SpeedBoost;
        }

        public override void DeactivateSkill(PlayerControllerWSAD player)
        {
            player.moveSpeedBoost -= m_SpeedBoost;
        }
    }
}