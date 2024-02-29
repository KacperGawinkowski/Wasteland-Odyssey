using Player;
using UnityEngine;
using Utils.StateMachine;

public class EnemyApproachState : IStateMachineState<EnemyNpc>
{
    public static readonly EnemyApproachState Instance = new();

    private EnemyApproachState()
    {
    }

    public void OnEnter(EnemyNpc enemy)
    {
        enemy.attackStateTriggerDistance = enemy.equipmentController.currentlyHeldWeapon.GetWeapon().effectiveRange - Random.Range(0, enemy.approachMaxRandomModifier);

        enemy.agent.isStopped = false;
        enemy.agent.stoppingDistance = 0f;
        enemy.baseMoveSpeed = 4f;
        enemy.agent.speed = enemy.baseMoveSpeed * (1 - enemy.moveSpeedDecrease);
        
    }

    public IStateMachineState<EnemyNpc> OnFixedUpdate(EnemyNpc enemy)
    {
        if (enemy.playerIsSeen)
        {
            PlayerControllerWSAD player = PlayerController.Instance.playerControllerLocal;
            enemy.agent.SetDestination(player.transform.position);

            if (enemy.attackStateTriggerDistance >= enemy.agent.remainingDistance)
            {
                return EnemyAttackState.Instance;
            }

            return this;
        }

        return EnemySeekState.Instance;
    }

    public void OnExit(EnemyNpc enemy)
    {
        // throw new System.NotImplementedException();
    }
}