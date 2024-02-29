using Player;
using UnityEngine;
using Utils.StateMachine;

public class EnemyAttackState : IStateMachineState<EnemyNpc>
{
    public static readonly EnemyAttackState Instance = new();

    private EnemyAttackState()
    {
    }


    public void OnEnter(EnemyNpc enemy)
    {
        enemy.agent.isStopped = false;
        enemy.attackApproachDistance = enemy.equipmentController.currentlyHeldWeapon.GetWeapon().effectiveRange - Random.Range(0, enemy.approachMaxRandomModifier);
        enemy.agent.stoppingDistance = enemy.attackApproachDistance;

        enemy.baseMoveSpeed = 3f;
        enemy.agent.speed = enemy.baseMoveSpeed * (1 - enemy.moveSpeedDecrease);
    }

    public IStateMachineState<EnemyNpc> OnFixedUpdate(EnemyNpc enemy)
    {
        if (enemy.playerIsSeen)
        {
            PlayerControllerWSAD player = PlayerController.Instance.playerControllerLocal;

            Vector3 playerPosition = player.transform.position;
            
            enemy.transform.LookAt(new Vector3(playerPosition.x, enemy.transform.position.y, playerPosition.z));
            
            enemy.agent.SetDestination(playerPosition);

            float distance = Vector3.Distance(enemy.transform.position, playerPosition);
            if (enemy.equipmentController.currentlyHeldWeapon.GetWeapon().effectiveRange + enemy.shootRangeTolerance < distance)
            {
                return EnemyApproachState.Instance;
            }
            
            enemy.Shoot();

            return this;
        }
        else
        {
            return EnemyApproachState.Instance;
        }
    }

    public void OnExit(EnemyNpc enemy)
    {
        // throw new System.NotImplementedException();
    }
}