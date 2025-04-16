using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner instance;
    [Header("General")]

    public float spawnInterval = 3f;   // 초기 적 생성 간격
    public float minSpawnInterval = 0.5f; // 난이도가 올라갈 때 최소 간격
    public float difficultyIncreaseRate = 0.95f; // 점점 빠르게 생성 (5%씩 감소)
    public int maxEnemies = 30; // 최대 적 개수
    private float currentSpawnInterval;
    private int currentEnemyCount = 0;

    public float mapSizeX = 5f;
    public float mapSizeY = 2f;

    void Start()
    {
        instance = this;
        currentSpawnInterval = spawnInterval;
        StartCoroutine(SpawnEnemiesOverTime());
    }

    IEnumerator SpawnEnemiesOverTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(currentSpawnInterval);

            if (currentEnemyCount < maxEnemies)
            {
                SpawnEnemy();
                currentEnemyCount++;
            }

            // 난이도 증가 (생성 간격 점점 줄어듦)
            currentSpawnInterval *= difficultyIncreaseRate;
            if (currentSpawnInterval < minSpawnInterval)
                currentSpawnInterval = minSpawnInterval;
        }
    }

    public static void SpawnEnemy()
    {
        Vector3 spawnPosition = instance.GetRandomEdgePosition();
        ObjectPooling.GetEnemy(spawnPosition);
    }

    private Vector3 GetRandomEdgePosition()
    {
        int edge = Random.Range(0, 4); // 0: 왼쪽, 1: 오른쪽, 2: 위쪽, 3: 아래쪽
        float x = 0, y = 0;

        switch (edge)
        {
            case 0: // 왼쪽
                x = -mapSizeX;
                y = Random.Range(-mapSizeY, mapSizeY);
                break;
            case 1: // 오른쪽
                x = mapSizeX;
                y = Random.Range(-mapSizeY, mapSizeY);
                break;
            case 2: // 위쪽
                x = Random.Range(-mapSizeX, mapSizeX);
                y = mapSizeY;
                break;
            case 3: // 아래쪽
                x = Random.Range(-mapSizeX, mapSizeX);
                y = -mapSizeY;
                break;
        }

        return new Vector3(x, y, 0);
    }

    public static void EnemyDied()
    {
        instance.currentEnemyCount--;
    }
}
