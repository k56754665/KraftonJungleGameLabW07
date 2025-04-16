using System.Collections.Generic;
using UnityEngine;

public class ObjectPooling : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab; // 생성할 Enemy 프리팹
    [SerializeField] private int initEnemyCount = 10; // 초기 생성할 적의 개수

    private Queue<GameObject> enemyPool = new Queue<GameObject>(); // 적을 저장할 큐
    private static ObjectPooling instance;


    void Awake()
    {
        instance = this;
        InitializePool();
    }
    void InitializePool()
    {
        for (int i = 0; i < initEnemyCount; i++)
        {
            GameObject enemy = Instantiate(enemyPrefab);
            enemy.SetActive(false);
            enemyPool.Enqueue(enemy);
        }
    }
    public static GameObject GetEnemy(Vector3 spawnPosition)
    {
        GameObject enemy;

        if (instance.enemyPool.Count > 0)
        {
            enemy = instance.enemyPool.Dequeue();
        }
        else
        {
            enemy = Instantiate(instance.enemyPrefab);
        }

        enemy.transform.position = spawnPosition; // 항상 맵 끝에서 스폰
        enemy.SetActive(true);

        return enemy;
    }


    public static void ReturnEnemy(GameObject enemy)
    {
        enemy.SetActive(false);
        instance.enemyPool.Enqueue(enemy);
    }
}
