using NPC;
using NPC.Friendly;
using UnityEngine;
using UnityEngine.Serialization;

public class NpcSpawnPoint : MonoBehaviour
{
    [SerializeField] public float chanceToBecomeTrader;
    [SerializeField] public TraderType traderType;
    [SerializeField] public float chanceToBecomeHealer;
    [SerializeField] public float chanceToBecomeUpgrader;

    [FormerlySerializedAs("canBeStroylineQuestNpc")] [SerializeField] public bool canBeStorylineQuestNpc;
}
