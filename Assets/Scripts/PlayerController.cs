using UnityEngine;
using CodeMonkey.Utils;

public class PlayerController : MonoBehaviour
{
    public enum PlayerState 
    {
        Walk,
        Run,
        Interaction
    }

    PlayerState currentState = PlayerState.Walk;
    public PlayerState CurrentState { get { return currentState; } set { currentState = value; } }


    public int hp;
    public int maxHp;

    // 총
    public Canvas_Script canvas;
    public GameManager gameManager;
    public ParticleSystem deathParticle;
    public ParticleSystem healParticle;
    public Rigidbody2D playerRb;

    // 시야각 가져오기
    [SerializeField] private FieldOfView_Script fieldOfView;

    public GameObject targetPatient;
    bool isCanSave = false;
    public bool IsCanSave { get { return isCanSave; } set { isCanSave = value; } }

    public float holdKeyTime = 0f;

    //이동
    PlayerMove _playerMove; 
    float runMultiply = 1;
    public float RunMultiply => runMultiply;

    //사격
    PlayerFire _playerShooting;


    void Start()
    {
        hp = maxHp;
        runMultiply = 1;

        gameManager = GameObject.FindFirstObjectByType<GameManager>();
        canvas = GameObject.FindFirstObjectByType<Canvas_Script>();
        playerRb = GetComponent<Rigidbody2D>();
        _playerMove = GetComponent<PlayerMove>();
        _playerShooting = GetComponent<PlayerFire>();
        InputManager.Instance.runAction += PlayerRun;
        InputManager.Instance.stopRunAction += StopRun;
    }
    void Update()
    {
        // 마우스 휠 스크롤 시 총 변경
        _playerShooting.CheckMouseWheel();

        SetHealth();


        if (!gameManager.isgameover)
        {
            // 플레이어 상태에 따른 속도 배수 변화
            switch (currentState) 
            {
                case PlayerState.Walk:
                    runMultiply = 1f;
                    _playerMove.SoundwaveInterval = 0.4f;
                    break;
                case PlayerState.Run:
                    runMultiply = 2f;
                    _playerMove.SoundwaveInterval = 0.15f;
                    break;
                case PlayerState.Interaction:
                    runMultiply = 0;
                    _playerMove.SoundwaveInterval = 0.4f;
                    break;
            }

            // 이동
            if (currentState != PlayerState.Interaction)
            {
                _playerMove.Move();
            }

            // 플레이어 각도
            Vector3 mousePosition = InputManager.Instance.PointerMoveInput;
            Vector2 direction = (mousePosition - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle + 90));

            // 플레이어 위치, 각도와 Field of View 위치, 각도 동기화
            fieldOfView.SetAimDirection(UtilsClass.GetVectorFromAngle(angle + 90));
            fieldOfView.SetOrigin(this.transform.position);

            // 플레이어 체력이 적으면 UI표시
            if (hp == 1)
            {
                canvas.lowHp_UI.SetActive(true);
            }
            else
            {
                canvas.lowHp_UI.SetActive(false);
            }

            if (hp < 1)
            {
                gameManager.Gameover();
                TriggerDeath();
                if (gameObject != null)
                {
                    Destroy(gameObject);
                }
            }
        }
    }

    void PlayerRun()
    {
        currentState = PlayerState.Run;
    }

    void StopRun()
    {
        if (currentState == PlayerState.Run)
        {
            currentState = PlayerState.Walk;
        }
    }


    private void OnDestroy()
    {
        // 죽으면 파티클 생성
        Instantiate(deathParticle, transform.position, transform.rotation);
    }

    // 체력을 설정하는 메서드
    public void SetHealth()
    {
        // 체력이 최대 체력을 초과하지 않도록 제한
        if (hp > maxHp)
        {
            hp = maxHp;
        }
        // 체력이 0 이하로 떨어지지 않도록 설정
        if (hp < 0)
        {
            hp = 0; 
        }
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


    private void OnTriggerEnter2D(Collider2D _collision)
    {
        // 총알 맞기
        if (_collision.gameObject.CompareTag("Bullet"))
        {
            Bullet bullet = _collision.gameObject.GetComponent<Bullet>();

            if (bullet.bulletColor == Bullet.BulletColor.Yellow)
            {
                hp -= 1;
                Instantiate(deathParticle, transform.position, transform.rotation);
            }
        }

        //환자 살리기
        if (_collision.gameObject.CompareTag("Patient"))
        {
            isCanSave = true;
            targetPatient = _collision.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == targetPatient) 
        {
            isCanSave = false;
            holdKeyTime = 0f;
            targetPatient = null;
        }
    }

}