using UnityEngine;

public class EnemySpawnPointList : MonoBehaviour
{
    public EnemySpawnPointController[] spawnPoints;

    private void OnValidate()
    {
        spawnPoints = GetComponentsInChildren<EnemySpawnPointController>();
    }
}
