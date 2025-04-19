using UnityEngine;
using UnityEngine.AI;
using CodeMonkey.Utils;
using System.Collections.Generic;
using System.Collections;
using NavMeshPlus.Extensions;
using Define;

public class Enemy : MonoBehaviour
{
    // 플레이어의 Transform
    [Header("Player transform")]
    public Transform player;
    private Vector3 lastPlayerPosition; // 마지막으로 본 플레이어 위치

    [Header("Enemy Hp")]
    public int hp;
    public float stunDuration;
    private bool isStunned = false;
    private int maxHp;

    // 적 기본 총알
    [Header("Bullet")]
    public GameObject bulletPrefab;
    public float bulletSpeed = 20f;
    public Transform gunPosition;
    public ParticleSystem BuleDamagedParticle;
    public ParticleSystem RedDamagedParticle;
    public ParticleSystem HealingParticle;

    // 실제 raycast 시야각
    [Header("FoV of Enemy")]
    [SerializeField] private LayerMask layerMask;
    public float wideFov;
    public float wideViewDistance;
    public float longFov;
    public float longViewDistance;

    // 시야각 UI 가져오기
    [SerializeField] private FieldOfViewEnemy_Script FoVEnemy_Wide;
    [SerializeField] private FieldOfViewEnemy_Script FoVEnemy_Long;
    [SerializeField] private FoVEnemyColor_Script FoVEnemy_WideColor;
    [SerializeField] private FoVEnemyColor_Script FoVEnemy_LongColor;
    public GameObject FoVEnemy_WidePrefab;
    public GameObject FoVEnemy_LongPrefab;

    // 인공지능
    [Header("Enemy AI")]
    public NavMeshAgent agent;
    private AgentRotateSmooth2d agentRotate;

    public EnemyState currentState = EnemyState.Patrolling;

    [Header("Enemy Speed")]
    public float normalLinearSpeed;
    public float normalAngularSpeed;
    public float fastLinearSpeed;
    public float fastAngularSpeed;

    // ZoneMove
    [Header("AI: ZoneMove")]
    public List<Vector3> zoneMovePoints;
    private int currentZoneMovePointIndex;
    public bool isWalkingToZoneMovePoint;
    // Attacking
    [Header("AI: Attacking")]
    public float shootingInterval = 1f;
    public float attackRange;
    private bool isShooting = false;
    // Searching
    [Header("AI: Searching")]
    public float searchingWalkPointRange;
    public float searchingTime;
    private float serchingCountTimer;
    // Patroling
    [Header("AI: Patroling")]
    private Vector3 walkPoint;
    private bool isWalkPointSet;

    // 적 대사창
    [Header("Dialogue UI")]
    [SerializeField] private EnemyDialogue enemyDialogue;

    Vector3 _initPosition;
    GameObject _soundwaveRun; // 프리팹
    GameObject _soundwaveRunGameObject; // 생성된 게임 오브젝트

    Canvas _pressE_UI;

    public Vector3 SoundwavePosition
    {
        get { return _soundwavePosition; }
        set { _soundwavePosition = value; }
    }
    Vector3 _soundwavePosition;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agentRotate = GetComponent<AgentRotateSmooth2d>();
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        // 적 암살 E UI 시작시 비활성화
        _pressE_UI = transform.GetChild(0).GetComponent<Canvas>();
        _pressE_UI.enabled = false;

        // soundwaveRun Prefab 로드
        _soundwaveRun = Resources.Load<GameObject>("Prefabs/Soundwaves/SoundwaveRun");

        // 처음 인덱스를 0으로 설정, 처음에는 이동하지 않음
        currentZoneMovePointIndex = 0;
        isWalkingToZoneMovePoint = false;

        _initPosition = transform.position;
        maxHp = hp;

        // 레이어 마스크 설정
        layerMask = LayerMask.GetMask("Field Of View Object", "Smoke", "Player");

        FOVStart();
    }

    void Update()
    {
        if (player == null || agent == null)
        {
            StopAllCoroutines();
            return;
        }

        // 적 대사 변경
        enemyDialogue.DialogueTalking(currentState);

        // 가짜 시야각 UI(빛살) 위치 업데이트
        FOVUpdate();

        // 실제 시야각 레이캐스트 확인
        CheckFieldOfView(wideFov, wideViewDistance);
        CheckFieldOfView(longFov, longViewDistance);

        // 체력 체크
        if (hp <= 0)
        {
            EnemyDie();
        }

        // 스턴 상태 체크
        if (isStunned) return;

        // AI 3가지 상황 체크
        switch (currentState)
        {
            case EnemyState.Patrolling:
                ZoneMove();
                ChangeSpeed(normalLinearSpeed, normalAngularSpeed);
                FoVEnemy_WideColor.ChangeFoVColor(FoVEnemy_WideColor.white);
                FoVEnemy_LongColor.ChangeFoVColor(FoVEnemy_LongColor.white);
                break;
            case EnemyState.Checking:
                // 목표 위치에 도달했는지 확인
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    // 목표에 도달하면 Searching 상태로 전환
                    StartCoroutine(SearchForPlayer(1f));
                }
                ChangeSpeed(fastLinearSpeed, fastAngularSpeed);
                FoVEnemy_WideColor.ChangeFoVColor(FoVEnemy_WideColor.yellow);
                FoVEnemy_LongColor.ChangeFoVColor(FoVEnemy_LongColor.yellow);
                break;
            case EnemyState.Chasing:
                AttackPlayer(wideFov, wideViewDistance, longFov, longViewDistance);
                ChangeSpeed(fastLinearSpeed, fastAngularSpeed);
                FoVEnemy_WideColor.ChangeFoVColor(FoVEnemy_WideColor.red);
                FoVEnemy_LongColor.ChangeFoVColor(FoVEnemy_LongColor.red);
                break;
            case EnemyState.Searching:
                Searching();
                ChangeSpeed(fastLinearSpeed, fastAngularSpeed);
                FoVEnemy_WideColor.ChangeFoVColor(FoVEnemy_WideColor.yellow);
                FoVEnemy_LongColor.ChangeFoVColor(FoVEnemy_LongColor.yellow);
                break;
        }


        // FOV가 Enemy의 자식으로 들어갔기 때문에 회전값을 Enemy 기준으로 맞춰 줌
        // Lerp는 Quaternion이 0일때 급속도로 회전되는 버그 제거
        FoVEnemy_Wide.transform.rotation = Quaternion.Lerp(Quaternion.identity, FoVEnemy_Wide.transform.rotation, 0.5f);
        FoVEnemy_Long.transform.rotation = Quaternion.Lerp(Quaternion.identity, FoVEnemy_Long.transform.rotation, 0.5f);
    }

    private void OnTriggerEnter2D(Collider2D _collision)
    {
        if (_collision.gameObject.CompareTag("Bullet"))
        {
            Bullet bullet = _collision.gameObject.GetComponent<Bullet>();

            // 적이 순찰중일때는 파란총 -> 스턴, 빨간총 -> 주변살피기
            if (currentState == EnemyState.Patrolling)
            {
                if (bullet.bulletColor == Bullet.BulletColor.Blue)
                {
                    DamagedBullet("Blue");
                    Stun();
                }
                else if (bullet.bulletColor == Bullet.BulletColor.Red)
                {
                    DamagedBullet("Red");
                    StartSearching();
                }
            }
            // 적이 주변을 살피거나 이미 쫓아갈 때는 두 총 -> 모두 쫓아가기
            else if ((currentState == EnemyState.Chasing || currentState == EnemyState.Searching || currentState == EnemyState.Checking))
            {
                if (bullet.bulletColor == Bullet.BulletColor.Blue)
                {
                    DamagedBullet("Blue");
                    currentState = EnemyState.Chasing;
                }
                else if (bullet.bulletColor == Bullet.BulletColor.Red)
                {
                    DamagedBullet("Red");
                    currentState = EnemyState.Chasing;
                }
            }
            // 적이 스턴중일때는 두 총 -> 데미지만
            else if (currentState == EnemyState.Stunning)
            {
                if (bullet.bulletColor == Bullet.BulletColor.Blue)
                {
                    DamagedBullet("Blue");
                }
                else if (bullet.bulletColor == Bullet.BulletColor.Red)
                {
                    DamagedBullet("Red");
                }
            }
        }
    }


    /// <summary>
    /// 색깔 별 총알의 데미지를 결정하는 함수
    /// </summary>
    public void DamagedBullet(string _color)
    {
        if (_color == "Blue")
        {
            hp -= 1;
            Instantiate(BuleDamagedParticle, transform.position, transform.rotation);
        }
        else if (_color == "Red")
        {
            hp -= 4;
            Instantiate(RedDamagedParticle, transform.position, transform.rotation);
        }
    }


    /// <summary>
    /// FOV를 생성하고, Position, Rotation을 적의 위치에 맞게 초기화하는 함수
    /// </summary>
    public void FOVStart()
    {
        // Field of View Wide 오브젝트 생성
        GameObject fovObjectWide = Instantiate(FoVEnemy_WidePrefab, transform.position, Quaternion.identity);
        fovObjectWide.transform.SetParent(transform); // 적의 자식으로 설정
        fovObjectWide.transform.localPosition = Vector3.zero; // 로컬 좌표 (0,0,0)
        fovObjectWide.transform.localRotation = Quaternion.identity; // 회전 초기화

        FoVEnemy_Wide = fovObjectWide.GetComponent<FieldOfViewEnemy_Script>();
        FoVEnemy_WideColor = fovObjectWide.GetComponent<FoVEnemyColor_Script>();
        FoVEnemy_Wide.enemy = this;
        FoVEnemy_WideColor.enemy = this;

        // Field of View Long 오브젝트 생성
        GameObject fovObjectLong = Instantiate(FoVEnemy_LongPrefab, transform.position, Quaternion.identity);
        fovObjectLong.transform.SetParent(transform); // 적의 자식으로 설정
        fovObjectLong.transform.localPosition = Vector3.zero; // 로컬 좌표 (0,0,0)
        fovObjectLong.transform.localRotation = Quaternion.identity; // 회전 초기화

        FoVEnemy_Long = fovObjectLong.GetComponent<FieldOfViewEnemy_Script>();
        FoVEnemy_LongColor = fovObjectLong.GetComponent<FoVEnemyColor_Script>();
        FoVEnemy_Long.enemy = this;
        FoVEnemy_LongColor.enemy = this;

        // 시야각의 시작 위치 설정 (로컬 좌표 기준)
        FoVEnemy_Wide.SetOrigin(Vector3.zero);
        FoVEnemy_Long.SetOrigin(Vector3.zero);

        // Enemy에서 설정한 시야각과 시야범위 적용
        FoVEnemy_Wide.fov = wideFov;
        FoVEnemy_Wide.viewDistance = wideViewDistance;
        FoVEnemy_Long.fov = longFov;
        FoVEnemy_Long.viewDistance = longViewDistance;

        // MeshRenderer 활성화 확인
        MeshRenderer wideRenderer = fovObjectWide.GetComponent<MeshRenderer>();
        MeshRenderer longRenderer = fovObjectLong.GetComponent<MeshRenderer>();
        if (wideRenderer != null) wideRenderer.enabled = true;
        if (longRenderer != null) longRenderer.enabled = true;

        // 시작 시 시야각 활성화
        FoVEnemy_Wide.FoVTurnOnOff(true);
        FoVEnemy_Long.FoVTurnOnOff(true);

        //Debug.Log($"FOV Wide Created: Position={fovObjectWide.transform.position}, LocalPosition={fovObjectWide.transform.localPosition}");
        //Debug.Log($"FOV Long Created: Position={fovObjectLong.transform.position}, LocalPosition={fovObjectLong.transform.localPosition}");
    }

    /// <summary>
    /// FOV를 적의 위치, 회전에 맞게 업데이트하는 함수
    /// </summary>
    public void FOVUpdate()
    {
        // Enemy 스프라이트가 위를 바라보고 있으므로 위를 기준으로
        Vector3 aimDirection = transform.up; // 또는 transform.right (스프라이트 방향에 따라)

        FoVEnemy_Wide.SetAimDirection(aimDirection);
        FoVEnemy_Long.SetAimDirection(aimDirection);
    }

    /// <summary>
    /// 적의 이동 속도를 변경하는 함수
    /// </summary>
    void ChangeSpeed(float _linearSpeed, float _angularSpeed)
    {
        agent.speed = _linearSpeed;
        agentRotate.SetSmoothAngularSpeed(_angularSpeed);
    }

    /// <summary>
    /// 적의 시야각을 체크하여 플레이어를 발견했는지 확인
    /// </summary>
    private void CheckFieldOfView(float _fov, float _viewDistance)
    {
        Vector3 origin = transform.position;
        float startingAngle = transform.eulerAngles.z - _fov / 2f + 90;

        int rayCount = 50;
        float angleIncrease = _fov / rayCount;

        for (int i = 0; i <= rayCount; i++)
        {
            float angle = startingAngle + angleIncrease * i;
            RaycastHit2D hit = Physics2D.Raycast(origin, UtilsClass.GetVectorFromAngle(angle), _viewDistance, layerMask);

            // Raycast 결과 처리
            if (hit.collider != null)
            {
                if (hit.collider.CompareTag("Player"))
                {
                    //Debug.Log("플레이어를 발견했습니다!");
                    lastPlayerPosition = hit.collider.transform.position; // 마지막으로 본 플레이어 위치 저장
                    currentState = EnemyState.Chasing; // 상태를 Chasing으로 변경

                    // TODO : 주변 적에게 알림
                    if (_soundwaveRunGameObject == null)
                    {
                        _soundwaveRunGameObject = Instantiate(_soundwaveRun, transform.position, transform.rotation);
                    }
                }
                else if (hit.collider.CompareTag("Field Of View Object"))
                {
                    // Field of View Object에 가로막힘
                    //Debug.Log("Field of View Object에 의해 가로막힘");
                    // 장애물과의 거리로 Ray 길이 조정
                    //Debug.DrawRay(origin, UtilsClass.GetVectorFromAngle(angle) * hit.distance, UnityEngine.Color.red);
                }
            }
        }
    }

    /// <summary>
    /// NavMeshAgent를 다음 ZoneMove 포인트로 이동시키는 함수
    /// </summary>
    void ZoneMove()
    {
        if (isStunned) // Stun 상태일 때는 이동하지 않도록 체크
            return;

        if (zoneMovePoints.Count == 0)
            return; // 탐사할 포인트가 없으면 종료

        // 현재 목표 포인트 설정
        Vector3 walkPoint = zoneMovePoints[currentZoneMovePointIndex];
        agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        // walkPoint 도달 시
        if (distanceToWalkPoint.magnitude < 1f)
        {
            // 다음 포인트로 이동
            currentZoneMovePointIndex++;
            if (currentZoneMovePointIndex >= zoneMovePoints.Count)
            {
                currentZoneMovePointIndex = 0; // 마지막 포인트에 도달하면 처음으로 돌아감
            }
        }
    }

    /// <summary>
    /// 플레이어의 위치로 이동하는 함수
    /// </summary>
    public void MoveToPlayerPosition()
    {
        if (player == null) return;

        Vector3 playerPosition = player.position; // 플레이어의 위치 가져오기

        // 적의 위치를 플레이어의 위치로 설정
        agent.SetDestination(playerPosition);

        // 이동 후 Searching 상태로 전환을 위해 상태를 Checking으로 설정
        currentState = EnemyState.Checking;
    }

    // TODO : 가장 최근 음파로 이동하는 함수
    public void MoveToCurrentSoundwave()
    {
        agent.SetDestination(_soundwavePosition); // 음파 위치로 이동

        currentState = EnemyState.Checking; // 상태를 Checking으로 변경
    }

    /// <summary>
    /// 플레이어를 쫓는 함수
    /// </summary>
    private void ChasePlayer()
    {

        if (lastPlayerPosition == null) return;

        agent.SetDestination(lastPlayerPosition); // 마지막으로 본 위치로 이동

        // 마지막 위치에 도착했는지 확인
        if (Vector3.Distance(transform.position, lastPlayerPosition) < 1f)
        {
            StartSearching(); // Searching 상태로 전환
        }
    }

    private void ReturnToPatrolling()
    {
        currentState = EnemyState.Patrolling; // 다시 패트롤 상태로 변경
    }

    /// <summary>
    /// Searrcing 상태로 전환되는 모든 경우에서 serchingCountTimer를 초기화하는 함수
    /// </summary>
    void StartSearching()
    {
        currentState = EnemyState.Searching;
        serchingCountTimer = searchingTime; // 타이머 초기화
        isWalkPointSet = false; // walkPoint 초기화
    }


    void Searching()
    {
        // 타이머 감소
        serchingCountTimer -= Time.deltaTime;

        // 타이머가 0 이하일 경우, 다시 패트롤 상태로 돌아감
        if (serchingCountTimer <= 0f)
        {
            currentState = EnemyState.Patrolling;
            isWalkPointSet = false; // walkPoint 초기화
            serchingCountTimer = searchingTime; // 타이머 초기화
            return;
        }


        if (!isWalkPointSet)
        {
            SearchWalkPoint();
        }
        else if (isWalkPointSet)
        {
            agent.SetDestination(walkPoint);
        }

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        // walkPoint 도달 시
        if (distanceToWalkPoint.magnitude < 1f)
            isWalkPointSet = false;
    }

    /// <summary>
    /// 1초마다 랜덤한 walkPoint를 찾는 함수
    /// </summary>
    void SearchWalkPoint()
    {
        // 범위 내 랜덤한 포인트 지정
        float randomX = Random.Range(-searchingWalkPointRange, searchingWalkPointRange);
        float randomY = Random.Range(-searchingWalkPointRange, searchingWalkPointRange);

        Vector3 randomPoint = new Vector3(transform.position.x + randomX, transform.position.y + randomY, 0);

        NavMeshHit hit;

        // NavMesh에서 walkable한 위치를 찾기
        if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
        {
            walkPoint = hit.position; // 찾은 위치를 walkPoint로 설정
            isWalkPointSet = true;
        }
    }

    void AttackPlayer(float _wideFov, float _wideDistance, float _longFov, float _longDistance)
    {
        Vector3 origin = transform.position;
        int rayCount = 50;

        float wideStartingAngle = transform.eulerAngles.z - _wideFov / 2f + 90;
        float longStartingAngle = transform.eulerAngles.z - _longFov / 2f + 90;

        float wideAngleIncrease = _wideFov / rayCount;
        float longAngleIncrease = _longFov / rayCount;

        bool playerInSight = false; // 플레이어가 시야에 있는지 확인하기 위한 플래그

        for (int i = 0; i <= rayCount; i++)
        {
            float wideAngle = wideStartingAngle + wideAngleIncrease * i;
            float longAngle = longStartingAngle + longAngleIncrease * i;

            RaycastHit2D wideHit = Physics2D.Raycast(origin, UtilsClass.GetVectorFromAngle(wideAngle), _wideDistance, layerMask);
            RaycastHit2D longHit = Physics2D.Raycast(origin, UtilsClass.GetVectorFromAngle(longAngle), _longDistance, layerMask);

            // 플레이어가 발견된 경우
            if (wideHit.collider != null && wideHit.collider.CompareTag("Player") ||
                longHit.collider != null && longHit.collider.CompareTag("Player"))
            {
                playerInSight = true;
                break; // 플레이어를 찾았으므로 루프 종료
            }
        }

        // 플레이어가 시야에 있을 때 총을 쏘기 시작
        if (playerInSight && !isShooting)
        {
            isShooting = true;
            agent.SetDestination(transform.position); // 이동 멈춤
            StartCoroutine(ShootAtPlayer()); // 총 쏘기 시작
        }
        else if (!playerInSight)
        {
            // 플레이어가 시야에 없을 경우 마지막 위치로 이동
            ChasePlayer();
        }
    }

    private IEnumerator SearchForPlayer(float _waitSecond)
    {
        // 잠시 대기 (예: 1초)
        yield return new WaitForSeconds(_waitSecond);

        StartSearching();
    }


    private IEnumerator ShootAtPlayer()
    {
        while (isShooting)
        {
            // 플레이어 방향 계산, 적 몸 비틀기
            Vector3 directionToPlayer = (player.position - gunPosition.position).normalized;
            float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg - 90;
            Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle));
            this.transform.rotation = targetRotation; 

            // 총알 생성
            GameObject bullet = Instantiate(bulletPrefab, gunPosition.position, targetRotation);
            Debug.Log("총을 쏩니다!");

            // 총알 rigidbody 가져온 후, 힘 가하기
            Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
            bulletRb.linearVelocity = directionToPlayer * bulletSpeed;

            // 총을 쏜 후 잠시 대기
            yield return new WaitForSeconds(shootingInterval);

            // 다시 거리 체크
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            if (distanceToPlayer > attackRange)
            {
                isShooting = false; // 공격 범위를 벗어나면 멈춤
            }
        }
    }


    /// <summary>
    /// 적이 스턴될 시, 이동을 멈추고 시야각을 끄는 함수
    /// </summary>
    public void Stun()
    {
        isStunned = true;
        agent.isStopped = true;
        currentState = EnemyState.Stunning; // Stun 상태로 변경

        // 시야각 UI 끄기
        FoVEnemy_Wide.FoVTurnOnOff(false);
        FoVEnemy_Long.FoVTurnOnOff(false);

        Invoke("RecoverFromStun", stunDuration); // 일정 시간 후 Stun 해제
    }

    /// <summary>
    /// 플레이어 위치와 적의 위치 간의 거리를 계산하여 적을 활성화하거나 비활성화합니다.
    /// </summary>
    public void NavMeshEnemyOnOff(Transform player, float deleteDistance)
    {
        float distance = Vector3.Distance(player.position, this.transform.position);

        if (distance > deleteDistance)
        {
            // 적 비활성화
            gameObject.SetActive(false);
            FoVEnemy_WidePrefab.SetActive(false);
            FoVEnemy_LongPrefab.SetActive(false);
        }
        else
        {
            // 적 활성화
            gameObject.SetActive(true);
            FoVEnemy_WidePrefab.SetActive(true);
            FoVEnemy_LongPrefab.SetActive(true);
        }
    }

    /// <summary>
    /// 스턴 이후, Patrolling 상태로 복귀하는 함수
    /// </summary>
    private void RecoverFromStun()
    {
        isStunned = false;
        agent.isStopped = false;
        currentState = EnemyState.Patrolling; // Patrolling 상태로 복귀

        // 시야각 UI 켜기
        FoVEnemy_Wide.FoVTurnOnOff(true);
        FoVEnemy_Long.FoVTurnOnOff(true);
    }

    public void EnemyDie()
    {
        FoVEnemy_Wide.DestroyFOV();
        FoVEnemy_Long.DestroyFOV();
        Instantiate(RedDamagedParticle, transform.position, transform.rotation);

        // TODO : Destroy 대신 SetActive(false)로 비활성화, EnemyManager의 딕셔너리에 사망 현황 업데이트
        gameObject.SetActive(false);
        EnemyManager.Instance.AddDeadEnemyStatus(this);
        //Destroy(gameObject);
    }

    /// <summary>
    /// FOV 재생성, 적 위치 초기화, 체력 초기화
    /// </summary>
    public void Respawn()
    {
        FOVStart();
        transform.position = _initPosition;
        hp = maxHp;
    }
}
