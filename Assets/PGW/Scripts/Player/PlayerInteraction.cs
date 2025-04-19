using UnityEngine;
using System.Collections;
using Define;

public class PlayerInteraction : MonoBehaviour
{
    PlayerController _playerController;
    GameManager _gameManager;

    // 구출
    [SerializeField] ParticleSystem _healParticle;
    bool _isInteractionActive; // 인터랙션 진행 여부
    float _pressTime;

    //암살
    [Header("Assassination")]
    float assassinRange = 3f; // 공격 범위

    //오브젝트 상호작용
    [SerializeField] bool _isInCloset = false; // 플레이어가 옷장 안에 있는지 여부
    public bool IsInCloset { get { return _isInCloset; } set { _isInCloset = value; } }

    void Start()
    {
        _playerController = GetComponent<PlayerController>();
        _gameManager = GameObject.FindFirstObjectByType<GameManager>();
        InputManager.Instance.interactionAction += PlayerInteractionAction;
        InputManager.Instance.holdInteractionAction += PlayerHoldInteractionAction;
    }

    void PlayerInteractionAction()
    {
        if (_playerController.CurrentTarget)
        {
            switch(_playerController.TargetType)
            {
                case Target.Patient:
                    StartRescue();
                    break;
                case Target.Enemy:
                    Assassinate(_playerController.CurrentTarget);
                    break;
                case Target.Object:
                    _playerController.CurrentTarget.SendMessage("Activate", SendMessageOptions.DontRequireReceiver);
                    break;
            }
        }
    }

    void PlayerHoldInteractionAction()
    {
        _isInteractionActive = false; // 인터랙션 종료
        StopCoroutine(AutoExecuteAfterDelay(2f)); // 자동 실행 코루틴 중지

        if (_playerController.TargetType == Target.Patient && _playerController.CurrentTarget)
        {
            EndRescue(_playerController.CurrentTarget);
        }
    }

    //환자 구출
    void StartRescue()
    {
        _playerController.CurrentState = PlayerState.Interaction;
        _pressTime = Time.time;
        _isInteractionActive = true; // 인터랙션 시작
        StartCoroutine(AutoExecuteAfterDelay(2f)); // 3초 후 자동 실행
    }

    void EndRescue(GameObject target)
    {
        float timeDifference = Time.time - _pressTime;
        float holdDuration = 2f;

        if (timeDifference >= holdDuration)
        {
            TriggerPatientHeal();
            Destroy(target);
            _gameManager.isGameClear = true;
        }
        _playerController.CurrentState = PlayerState.Walk;
    }

    private IEnumerator AutoExecuteAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // 3초 대기

        // PlayerHoldInteractionAction이 호출되지 않은 경우에만 실행
        if (_isInteractionActive && _playerController.TargetType == Target.Patient && _playerController.CurrentTarget)
        {
            TriggerPatientHeal();
            Destroy(_playerController.CurrentTarget);
            _gameManager.isGameClear = true;
            _playerController.CurrentState = PlayerState.Walk;
        }
    }

    void TriggerPatientHeal()
    {
        if (_healParticle != null)
        {
            ParticleSystem heal = Instantiate(_healParticle, _playerController.CurrentTarget.transform.position, Quaternion.identity);
            heal.Play();
            Destroy(heal.gameObject, 2f);
        }
    }

    //적 암살
    void Assassinate(GameObject target)
    {
        if (_isInCloset) return;
        if (CheckAssassinateCondition(target))
        {
            target.GetComponent<Enemy>().EnemyDie();
            Instantiate(_healParticle, transform.position, transform.rotation);
        }
    }

    public bool CheckAssassinateCondition(GameObject target)
    {
        // 암살시 적과 플레이어 거리 확인
        Vector3 enemyPosition = target.transform.position;
        Vector3 directionToEnemy = enemyPosition - transform.position;
        float distance = directionToEnemy.magnitude;
        // 암살시 플레이어가 공격 범위 내에 있는지 확인
        if (distance < assassinRange)
        {
            // 적 상태 확인
            EnemyState enemyState = target.GetComponent<Enemy>().currentState;
            if (enemyState != EnemyState.Chasing)
            {
                return true;
            }
        }
        return false;
    }

    public void ShowEKeyUI(bool _isPlayerClose)
    {
        Canvas pressE_UI = _playerController.CurrentTarget?.transform.GetChild(0).GetComponent<Canvas>();
        Debug.Log("ShowEKeyUI");
        if (pressE_UI == null) return;
        if (_isPlayerClose == true)
        {
            pressE_UI.enabled = true;
        }
        else
        {
            pressE_UI.enabled = false;
            _playerController.CurrentTarget = null;
        }
    }

    private void OnDestroy()
    {
        InputManager.Instance.interactionAction -= PlayerInteractionAction;
        InputManager.Instance.holdInteractionAction -= PlayerHoldInteractionAction;
    }
}