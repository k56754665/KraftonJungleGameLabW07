using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance => _instance;
    static EnemyManager _instance;

    Enemies _enemyRoot;
    //List<GameObject> _enemies = new List<GameObject>(); // 모든 적 오브젝트를 List로 변경
    float _deleteDistance = 40f; // 비활성화 거리

    PlayerController _player;

    Dictionary<Enemy, bool> _enemyStatus = new Dictionary<Enemy, bool>(); // 적 생존 상태를 저장할 딕셔너리

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }

    void Start()
    {
        _player = GameObject.FindFirstObjectByType<PlayerController>();
        _enemyRoot = FindAnyObjectByType<Enemies>();
        Enemy[] _enemiesArray = _enemyRoot.GetComponentsInChildren<Enemy>();
        for (int i = 0; i < _enemiesArray.Length; i++)
        {
            //_enemies.Add(_enemiesArray[i].gameObject); // 적 오브젝트를 리스트에 추가
            _enemyStatus.Add(_enemiesArray[i], true); // 적 생존 상태를 딕셔너리에 추가
        }
    }

    void Update()
    {
        if (_player == null) return;

        foreach (var enemy in _enemyStatus.Keys)
        {
            if (enemy != null)
            {
                Enemy enemyController = enemy.GetComponent<Enemy>();
                if (enemyController != null)
                {
                    // 적이 파괴되지 않았을 경우
                    if (_enemyStatus[enemyController])
                    {
                        // 적의 생존 상태를 업데이트
                        enemy.NavMeshEnemyOnOff(_player.transform, _deleteDistance);
                    }
                }
            }
        }
        // 적 상태 업데이트
        /*for (int i = _enemies.Count - 1; i >= 0; i--) // 역방향으로 순회
        {
            if (_enemies[i] != null) // 적이 파괴되지 않았을 경우
            {
                Enemy enemyController = _enemies[i].GetComponent<Enemy>();
                if (enemyController != null)
                {
                    enemyController.NavMeshEnemyOnOff(_player.transform, _deleteDistance);
                }
            }
            else
            {
                // 적이 파괴된 경우 리스트에서 제거
                _enemies.RemoveAt(i);
            }
        }*/
    }

    // TODO : Enemy 딕셔너리에 특정 적을 false로 바꾸는 함수
    public void UpdateEnemyStatus(Enemy enemy, bool status)
    {
        Debug.Log(enemy.name + " Dead");
        _enemyStatus[enemy] = status;
        if (status)
        {
            // 적을 살리는 로직
            enemy.Respawn();
        }
    }

    public GameObject CheckClosestEnemy()
    {
        Enemy closestEnemy = null;
        float closestDistance = Mathf.Infinity;
        foreach (var enemy in _enemyStatus.Keys)
        {
            if (_enemyStatus[enemy])
            {
                float distance = Vector3.Distance(_player.transform.position, enemy.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = enemy;
                }
            }
        }
        if (closestEnemy != null)
        {
            return closestEnemy.gameObject;
        }
        else
        {
            return null;
        }
    }
}
