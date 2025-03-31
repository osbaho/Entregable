using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpawnPoint
{
    public Transform point;
    [Range(0f, 1f)]
    public float probability = 0.5f;
}

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> enemyPrefabs;
    [SerializeField] private float spawnInterval = 0.5f;
    [SerializeField] private int maxEnemies;
    private int currentEnemies = 0;
    [SerializeField] private List<SpawnPoint> spawnPoints = new List<SpawnPoint>();
    [SerializeField] private float spawnXOffset = 1f; // Rango máximo de variación en X

    private void Awake()
    {
        // Convertir los hijos en SpawnPoints con probabilidades por defecto
        foreach (Transform child in transform)
        {
            spawnPoints.Add(new SpawnPoint { point = child, probability = 1f / transform.childCount });
        }
        maxEnemies = Random.Range(1, 5);
    }

    private void Start()
    {
        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
    {
        while (currentEnemies < maxEnemies)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnEnemy()
    {
        if (enemyPrefabs.Count == 0 || spawnPoints.Count == 0)
        {
            Debug.LogError("No hay prefabs de enemigos o puntos de spawn asignados.");
            return;
        }

        GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
        Transform selectedSpawnPoint = SelectSpawnPointByProbability();

        if (selectedSpawnPoint != null)
        {
            // Crear una posición modificada con offset aleatorio en X
            Vector3 spawnPosition = selectedSpawnPoint.position;
            spawnPosition.x += Random.Range(-spawnXOffset, spawnXOffset);

            Instantiate(enemyPrefab, spawnPosition, selectedSpawnPoint.rotation);
            currentEnemies++;
        }
    }

    private Transform SelectSpawnPointByProbability()
    {
        float random = Random.value;
        float cumulativeProbability = 0;

        foreach (var spawnPoint in spawnPoints)
        {
            cumulativeProbability += spawnPoint.probability;
            if (random <= cumulativeProbability)
            {
                return spawnPoint.point;
            }
        }

        return spawnPoints[0].point; // Fallback al primer punto si algo sale mal
    }
}
