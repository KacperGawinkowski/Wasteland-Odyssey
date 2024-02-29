using Player;
using UnityEngine;

namespace Skills
{
    [CreateAssetMenu(fileName = nameof(AreaSkill), menuName = "Skills/" + nameof(AreaSkill))]
    public class AreaSkill : ActivatedSkill
    {
        public float duration;
        public GameObject effectToSpawn;
        public GameObject areaSelector;
    }
}