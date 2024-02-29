using System;
using System.Collections;
using UnityEngine;
using Utils.StateMachine;
using Random = UnityEngine.Random;

public class EnemyIdleState : IStateMachineState<EnemyNpc>
{
    public static readonly EnemyIdleState Instance = new();

    private EnemyIdleState()
    {
    }

    public void OnEnter(EnemyNpc enemy)
    {
        enemy.agent.isStopped = false;
        enemy.baseMoveSpeed = 2f;
        enemy.agent.speed = enemy.baseMoveSpeed * (1 - enemy.moveSpeedDecrease);
        enemy.agent.stoppingDistance = 0f;

        switch (enemy.idleType)
        {
            case EnemyIdleType.Standing:
                enemy.agent.SetDestination(enemy.spawnPosition);
                break;
            case EnemyIdleType.RandomWanderer:
                enemy.stateMachineCoroutine.Start(RandomWandererCoroutine(enemy));
                break;
            case EnemyIdleType.Patrol:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public IStateMachineState<EnemyNpc> OnFixedUpdate(EnemyNpc enemy)
    {
        if (enemy.playerIsSeen)
        {
            enemy.DisplayTextStatus("!!!", new Color(255, 140, 0), 1);
            return EnemyApproachState.Instance;
        }
        else
        {
            return this;
        }
    }

    public void OnExit(EnemyNpc enemy)
    {
        switch (enemy.idleType)
        {
            case EnemyIdleType.Standing:
                break;
            case EnemyIdleType.RandomWanderer:
                enemy.stateMachineCoroutine.Stop();
                break;
            case EnemyIdleType.Patrol:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private static IEnumerator RandomWandererCoroutine(EnemyNpc enemy)
    {
        while (true)
        {
            Vector2 destination = Random.insideUnitCircle * enemy.patrolRadius;
            enemy.agent.SetDestination(new Vector3(destination.x + enemy.spawnPosition.x, enemy.transform.position.y, destination.y + enemy.spawnPosition.z));
            float randomTime = Random.Range(4f, 10f);
            yield return new WaitForSeconds(randomTime);
        }
    }
}