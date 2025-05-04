using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Este spawner me lo robe de un proyecto mio que estaba realizando en el motor Phaser en TypeScript. Obviamente esta adaptado
public class WaveSpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    [SerializeField] private List<GameObject> normalEnemies;
    [SerializeField] private GameObject bossEnemy;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private float timeBetweenWaves = 5f;

    [SerializeField] private LayerMask enemyLayer;

    [Header("Wave Settings")]
    [SerializeField] private int enemiesToSpawn = 3;
    private int enemiesAlive = 0;
    private int currentWave  = 1;
    private bool isSpawning  = false;

    private void Update()
    {
        // Checkeo para arreglar bug que no pasaba de wave
        if (!isSpawning)
        {
            enemiesAlive = GameObject.FindGameObjectsWithTag("Enemies").Length;

            if (enemiesAlive <= 0)
            {
                StartCoroutine(StartNextWave());
            }
        }
    }

    private IEnumerator StartNextWave()
    {
        isSpawning = true;

        WaveUIManager.Instance?.ShowWave(currentWave);

        yield return new WaitForSeconds(timeBetweenWaves);

        if (currentWave == 10)
        {
            SpawnBossWave();
        }
        else
        {
            SpawnNormalWave();
        }

        isSpawning = false;
    }

    private void SpawnNormalWave()
    {
        Debug.Log("Spawning normal wave: " + currentWave);

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            SpawnEnemy();
        }

        enemiesToSpawn += 2;
        currentWave++;
    }

    private void SpawnBossWave()
    {
        Debug.Log("Spawning BOSS wave!");

        int spawnIndex = Random.Range(0, spawnPoints.Length);
        Vector3 pos = spawnPoints[spawnIndex].position;
        Instantiate(bossEnemy, pos, Quaternion.identity);
        enemiesAlive = 1;

        currentWave++;
    }

    private void SpawnEnemy()
    {
        int enemyIndex = ChooseEnemyBasedOnWave();
        Vector3 pos    = GetFreeSpawnPosition();
        Instantiate(normalEnemies[enemyIndex], pos, Quaternion.identity);

        enemiesAlive++;
    }

    private Vector3 GetFreeSpawnPosition()
    {
        const float radius = 1f;
        for (int i = 0; i < 20; i++)
        {
            int idx = Random.Range(0, spawnPoints.Length);
            Vector3 candidate = spawnPoints[idx].position;
            if (!Physics.CheckSphere(candidate, radius, enemyLayer))
                return candidate;
        }
        return spawnPoints[Random.Range(0, spawnPoints.Length)].position;
    }

    // Para variar enemigos en cada wave
    private int ChooseEnemyBasedOnWave()
    {
        if (currentWave == 1)
        {
            return 0;
        }
        else if (currentWave == 2)
        {
            return Random.Range(0, Mathf.Min(2, normalEnemies.Count)); // Normal o Rapido
        }
        else if (currentWave == 3)
        {
            return Random.Range(0, Mathf.Min(3, normalEnemies.Count)); // Normal, Rapido o Tank
        }
        else
        {
            return Random.Range(0, normalEnemies.Count); // Todos disponibles
        }
    }

    public void EnemyDied()
    {
        enemiesAlive--;
        if (currentWave > 10 && enemiesAlive == 0)
        {
            VictoryScreen victory = FindObjectOfType<VictoryScreen>();
            if (victory != null)
            {
                victory.ShowVictory();
            }
        }
    }
}
