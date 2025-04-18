using System;
using System.Collections.Generic;
using UnityEngine;

public class SavePointManager : MonoBehaviour
{
    public static SavePointManager Instance => _instance;
    static SavePointManager _instance;

    public Vector2 SavePlayerPosition { get { return _savePlayerPosition; } set { _savePlayerPosition = value; } }
    Vector2 _savePlayerPosition; // 저장 시점의 플레이어 위치

    public int SaveHP { get { return _saveHP; } set { _saveHP = value; } }
    int _saveHP; // 저장 시점의 플레이어 hp

    public int SaveBlueGunNumber { get { return _saveBlueGunNumber; } set { _saveBlueGunNumber = value; } }
    int _saveBlueGunNumber; // 저장 시점의 blue gun 총알 수

    public int SaveRedGunNumber { get { return _saveRedGunNumber; } set { _saveRedGunNumber = value; } }
    int _saveRedGunNumber; // 저장 시점의 red gun 총알 수

    public Dictionary<Enemy, bool> SaveEnemyStatus
    {
        get => new Dictionary<Enemy, bool>(_saveEnemyStatus); // 얕은 복사본 반환
        set { _saveEnemyStatus = value; } // 값만 가져와야 함
    }
    Dictionary<Enemy, bool> _saveEnemyStatus = new Dictionary<Enemy, bool>(); // 저장 시점의 적 생존 상태를 저장할 딕셔너리

    public Action OnSaveEvent;
    public Action OnLoadEvent;

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        //DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
