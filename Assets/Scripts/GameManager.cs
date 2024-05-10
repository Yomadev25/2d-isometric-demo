using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private int _maxEnemyPerScene;
    [SerializeField]
    private GameObject[] _enemyPrefabs;

    private void Awake()
    {
        EnemyManager.onDie += PrepareToSpawn;
    }

    private void OnDestroy()
    {
        EnemyManager.onDie -= PrepareToSpawn;
    }

    private void Start()
    {
        for (int i = 0; i < _maxEnemyPerScene; i++)
        {
            PrepareToSpawn();
        }
    }

    private void PrepareToSpawn()
    {
        Invoke(nameof(Spawn), 1f);
    }

    private void Spawn()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies.Length > _maxEnemyPerScene) return;

        Vector2 spawnPos = Vector2.zero;
        spawnPos.x = Random.Range(-3.5f, 3.5f);
        spawnPos.y = Random.Range(-1.8f, 1.8f);
        Instantiate(_enemyPrefabs[Random.Range(0, _enemyPrefabs.Length)], spawnPos, Quaternion.identity);
    }
}
