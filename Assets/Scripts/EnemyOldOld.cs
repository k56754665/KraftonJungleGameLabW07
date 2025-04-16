using UnityEngine;
using System.Collections;
using CodeMonkey.Utils;
using UnityEngine.AI;

public class EnemyOldOld : Character
{
    
    /*
    private Transform player; // 플레이어 위치 저장
    public GameObject bulletPrefab; // 총알 프리팹
    public Transform firePoint; // 총알 발사 위치
    
    // 시야각 가져오기
    [SerializeField] private FieldOfViewEnemy_Script fieldOfViewEnemy;
    public GameObject fieldOfViewEnemyPrefab;

    // 인공지능
    public NavMeshAgent agent;
    // Patroling
    public Vector3 walkPoint;
    bool isWalkPointSet;
    public float walkPointRange;
    // Attacking
    public float timeBetweenAttacks;
    bool isAlreadyAttacked;
    // States
    public float sightRange, attackRange, sightAngle, attackAngle;
    public bool isPlayerInSightRange, isPlayerInAttackRange;


    // public float alertDistance = 0f;
    // public float cautionDistance = 0f;
    public float fireRate = 3f; // 발사 주기
    public float moveTimer = 0f; // 자유 이동 시간 타이머

    private bool isFiring = false; // 발사 중인지 여부
    
    private Vector3 randomDirection; // 자유 이동 방향
    public ParticleSystem deathParticle;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        // Field of View 오브젝트 생성
        GameObject fovObject = Instantiate(fieldOfViewEnemyPrefab, Vector2.zero, Quaternion.identity);
        fieldOfViewEnemy = fovObject.GetComponent<FieldOfViewEnemy_Script>();
        fieldOfViewEnemy.enemy = this; // Field of View에 자신을 참조하게 설정

        // 시야각의 시작 위치와 회전 설정
        fieldOfViewEnemy.SetOrigin(transform.position);
        fieldOfViewEnemy.transform.rotation = transform.rotation; // 적과 같은 회전으로 설정

        movespeed = 10f;
        hp = 1;

        /*
        if (player != null)
        {
            moveTimer = 0f;
            ChooseRandomDirection();
        }
        */

    /*
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // 적의 현재 각도를 가져와 시야각을 업데이트
        float currentAngle = transform.eulerAngles.z;
        fieldOfViewEnemy.SetAimDirection(UtilsClass.GetVectorFromAngle(currentAngle));
        fieldOfViewEnemy.SetOrigin(this.transform.position);

        // 시야와 공격 범위 체크
        isPlayerInSightRange = Physics.CheckSphere(transform.position, sightRange);
        isPlayerInAttackRange = Physics.CheckSphere(transform.position, attackRange);

        // AI 3가지 상황 체크
        if (!isPlayerInSightRange && !isPlayerInAttackRange) Patroling();
        if (isPlayerInSightRange && !isPlayerInAttackRange) ChasePlayer();
        if (isPlayerInSightRange && isPlayerInAttackRange) AttackPlayer();


        /*
        if (distance > cautionDistance) //거리가 *f을 넘으면 자유로히움직임
        {
            FreeMove();
        }
        else if (distance <= cautionDistance && distance > alertDistance) //거리가 *f과 *f사이면 플레이어추적
        {
            ChasePlayerOldOld();
        }
        else if (distance <= alertDistance) //거리가 *f보다 작으면 격발
        {
            RotateEnemy();
            if (!isFiring && player)
            {                                  
                StartCoroutine(FireAtPlayer());
            }
        }
        */

        /*

        if (hp < 1) 
        {
            GameManager gm = GameObject.FindFirstObjectByType<GameManager>();
            TriggerDeath();
            Destroy(gameObject);
        }
    }

    void Patroling()
    {
        if (!isWalkPointSet) SearchWalkPoint();

        if (isWalkPointSet)
        {
            agent.SetDestination(walkPoint);
        }

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        // walkPoint 도달 시
        if (distanceToWalkPoint.magnitude < 1f)
            isWalkPointSet = false;
    }

    void SearchWalkPoint()
    {
        // 범위 내 랜덤한 포인트 지정
        float randomX = Random.Range(-walkPointRange, walkPointRange);
        float randomY = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y + randomY, transform.position.z);

        if (Physics2D.Raycast(walkPoint, -transform.up, 2f))
            isWalkPointSet = true;
    }

    void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }

    void AttackPlayer()
    {
        // 적이 움직이지 않도록 하는 코드
        agent.SetDestination(transform.position);

        transform.LookAt(player);

        if (!isAlreadyAttacked)
        {
            // 어택 코드
            // 어택 코드
            // 어택 코드


            isAlreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    void ResetAttack()
    {
        isAlreadyAttacked = false;
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 viewAngleSight = Quaternion.Euler(0, -sightAngle / 2, 0) * transform.forward * sightRange;
        Vector3 viewAngleAttack = Quaternion.Euler(0, attackAngle / 2, 0) * transform.forward * attackRange;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.DrawLine(transform.position, transform.position + viewAngleAttack);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
        Gizmos.DrawLine(transform.position, transform.position + viewAngleSight);
    }



    void TriggerDeath()
    {
        if (deathParticle != null)
        {
            ParticleSystem death = Instantiate(deathParticle, transform.position, Quaternion.identity);
            death.Play();
            Destroy(death.gameObject, 2f);
        }
    }
    void RotateEnemy() 
    {
        Vector3 direction = (player.position - transform.position).normalized;
        transform.up = -direction; // 플레이어를 향해 회전
    }
    
    // 자유롭게 이동 및 회전
    void FreeMove()
    {
        moveTimer += Time.deltaTime;

        // 일정 시간마다 방향 변경
        if (moveTimer > 3f)
        {
            ChooseRandomDirection();
            moveTimer = 0f;
        }

        transform.position += randomDirection * (movespeed * 0.5f) * Time.deltaTime;
        transform.Rotate(0, 0, Random.Range(-60f, 60f) * Time.deltaTime);
    }

    // 플레이어를 향해 이동
    void ChasePlayerOldOld()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * movespeed * Time.deltaTime;
        transform.up = -direction; // 플레이어를 향해 회전
    }

    // 3초마다 플레이어에게 총알 발사
    IEnumerator FireAtPlayer()
    {
        isFiring = true;
        yield return new WaitForSeconds(2f); //2초 대기 후 격발
        while (player && Vector3.Distance(transform.position, player.position) <= 20f)
        {
            GameObject flash = transform.Find("Flash").gameObject;
            if (flash)
            {
                flash.SetActive(true);
                yield return new WaitForSeconds(0.1f);
                flash.SetActive(false);
            }
            else 
            {
                Debug.Log("적 플래시 못찾음");
            }

            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            bullet.GetComponent<Bullet>().SetDirection((player.position - transform.position).normalized);
            bullet.GetComponent<Bullet>().from = gameObject.tag;
            bullet.transform.rotation = transform.rotation;
            yield return new WaitForSeconds(fireRate);
        }
        isFiring = false;
    }

    // 랜덤 방향 선택
    void ChooseRandomDirection()
    {
        randomDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0).normalized;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet") && collision.gameObject.GetComponent<Bullet>().from != gameObject.tag)
        {
            hp--;
            collision.gameObject.SetActive(false);
        }
    }



    */
}
