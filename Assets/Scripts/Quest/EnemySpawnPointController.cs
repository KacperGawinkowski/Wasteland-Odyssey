using UnityEngine;

public class EnemySpawnPointController : MonoBehaviour
{
    [SerializeField] private EnemyIdleType m_IdleType;

    [SerializeField] private EnemyNpc m_EnemyPrefab;
    private bool m_EnemySpawned;

    public EnemyNpc SpawnEnemy()
    {
        if (m_EnemySpawned) return null;

        EnemyNpc spawnedEnemy = Instantiate(m_EnemyPrefab, transform.position, Quaternion.identity);
        spawnedEnemy.idleType = m_IdleType;
        // tutaj trzeba dodać jakieś waypointy do patrolowania sus

        m_EnemySpawned = true;
        return spawnedEnemy;
    }
}
