using UnityEngine;
using CodeMonkey.Utils;
using Define;

public class PlayerController : MonoBehaviour
{
    PlayerState _currentState = PlayerState.Walk;
    [SerializeField] Target _targetType = Target.None;

    [SerializeField] ParticleSystem deathParticle;
    [SerializeField] FieldOfView_Script fieldOfView;

    Canvas_Script _canvas;
    GameManager _gameManager;
    [SerializeField] GameObject _target;
    PlayerMove _playerMove;
    PlayerInteraction _playerInteraction;

    public int hp;
    public int maxHp;
    float runMultiply = 1;

    public PlayerState CurrentState { get { return _currentState; } set { _currentState = value; } }
    public Target TargetType { get { return _targetType; } set { _targetType = value; } }
    public GameObject CurrentTarget { get { return _target; } set { _target = value; } }
    public float RunMultiply => runMultiply;
       


    void Start()
    {
        hp = maxHp;
        runMultiply = 1;

        _gameManager = GameObject.FindFirstObjectByType<GameManager>();
        _canvas = GameObject.FindFirstObjectByType<Canvas_Script>();
        _playerMove = GetComponent<PlayerMove>();
        _playerInteraction = GetComponent<PlayerInteraction>();
        InputManager.Instance.runAction += PlayerRun;
        InputManager.Instance.stopRunAction += StopRun;

        SavePointManager.Instance.OnSaveEvent += SavePointPlayer;
        SavePointManager.Instance.OnLoadEvent += LoadSavePointPlayer;
    }

    private void LateUpdate()
    {
        // 이동
        if (_currentState != PlayerState.Interaction)
        {
            _playerMove.Move();
        }
    }
    void Update()
    {

        SetHealth();


        if (!_gameManager.isgameover)
        {
            // 플레이어 상태에 따른 속도 배수 변화
            switch (_currentState)
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
                _canvas.lowHp_UI.SetActive(true);
            }
            else
            {
                _canvas.lowHp_UI.SetActive(false);
            }

            if (hp < 1)
            {
                _gameManager.Gameover();
                TriggerDeath();
                if (gameObject != null)
                {
                    Destroy(gameObject);
                }
            }
        }

        if(EnemyManager.Instance.CheckClosestEnemy())
        {
            GameObject closestEnemy = EnemyManager.Instance.CheckClosestEnemy();
            if(_playerInteraction.CheckAssassinateCondition(closestEnemy))
            {
                Debug.Log("Assassination condition met");
                _targetType = Target.Enemy;
                _target = closestEnemy;
                _playerInteraction.ShowEKeyUI(true);
            }
            else if(_target && _targetType == Target.Enemy)
            {
                _targetType = Target.None;
                _playerInteraction.ShowEKeyUI(false);
            }
        }
    }

        void PlayerRun()
    {
        _currentState = PlayerState.Run;
    }

    void StopRun()
    {
        if (_currentState == PlayerState.Run)
        {
            _currentState = PlayerState.Walk;
        }
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

        _target = _collision.gameObject;
        //환자 살리기
        if (_collision.gameObject.CompareTag("Patient"))
        {
            _targetType = Target.Patient;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == _target) 
        {
            _targetType = Target.None;
            _target = null;
        }
    }

    /// <summary>
    /// 현재 플레이어의 hp와 위치를 세이브 포인트에 저장하는 함수
    /// </summary>
    void SavePointPlayer()
    {
        SavePointManager.Instance.SaveHP = hp;
        SavePointManager.Instance.SavePlayerPosition = transform.position;
    }

    /// <summary>
    /// 플레이어의 hp와 위치를 세이브 포인트에 저장된 값으로 변경해주는 함수
    /// </summary>
    void LoadSavePointPlayer()
    {
        hp = SavePointManager.Instance.SaveHP;
        transform.position = SavePointManager.Instance.SavePlayerPosition;
    }
}