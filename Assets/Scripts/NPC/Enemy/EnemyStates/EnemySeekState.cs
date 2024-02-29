using Player;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using Utils;
using Utils.StateMachine;

public class EnemySeekState : IStateMachineState<EnemyNpc>
{
    public static readonly EnemySeekState Instance = new();

    private EnemySeekState()
    {
    }

    public void OnEnter(EnemyNpc enemy)
    {
        enemy.lastPlayerLocation = PlayerController.Instance.playerControllerLocal.transform.position;
        enemy.agent.SetDestination(enemy.lastPlayerLocation);

        enemy.agent.isStopped = false;
        enemy.agent.stoppingDistance = 0f;
        enemy.baseMoveSpeed = 4f;
        enemy.agent.speed = enemy.baseMoveSpeed * (1 - enemy.moveSpeedDecrease);

        enemy.seekTimeRemaining = enemy.seekTime;
    }

    public IStateMachineState<EnemyNpc> OnFixedUpdate(EnemyNpc enemy)
    {
        if (enemy.playerIsSeen)
        {
            return EnemyApproachState.Instance;
        }
        else
        {
            if (enemy.agent.remainingDistance <= 1f)
            {
                enemy.seekTimeRemaining -= Time.deltaTime;
                if (enemy.seekTimeRemaining <= 0)
                {
                    enemy.playerWasSeen = false;
                    enemy.DisplayTextStatus("???", new Color(255, 140, 0), 1);
                    return EnemyIdleState.Instance;
                }
            }
            return Instance;
        }
    }

    public void OnExit(EnemyNpc monoBehaviour)
    {

    }
}