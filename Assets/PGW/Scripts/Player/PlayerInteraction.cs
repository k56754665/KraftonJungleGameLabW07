using UnityEngine;
using static PlayerController;
using System.Collections;

public class PlayerInteraction : MonoBehaviour
{
    PlayerController _playerController;
    GameManager _gameManager;

    // Healing
    [SerializeField] ParticleSystem _healParticle;
    GameObject _targetPatient;
    bool _isInteractionActive; // 인터랙션 진행 여부

    float _pressTime;

    void Start()
    {
        _targetPatient = GameObject.Find("Patient");
        _playerController = GetComponent<PlayerController>();
        _gameManager = GameObject.FindFirstObjectByType<GameManager>();
        InputManager.Instance.interactionAction += PlayerInteractionAction;
        InputManager.Instance.holdInteractionAction += PlayerHoldInteractionAction;
    }

    void PlayerInteractionAction()
    {
        if (_playerController.IsCanSave && _targetPatient)
        {
            _playerController.CurrentState = PlayerState.Interaction;
            _pressTime = Time.time;
            _isInteractionActive = true; // 인터랙션 시작
            StartCoroutine(AutoExecuteAfterDelay(2f)); // 3초 후 자동 실행
        }
    }

    void PlayerHoldInteractionAction()
    {
        _isInteractionActive = false; // 인터랙션 종료
        StopCoroutine(AutoExecuteAfterDelay(2f)); // 자동 실행 코루틴 중지

        if (_playerController.IsCanSave && _targetPatient)
        {
            float timeDifference = Time.time - _pressTime;
            float holdDuration = 2f;

            if (timeDifference >= holdDuration)
            {
                TriggerPatientHeal();
                Destroy(_targetPatient);
                _playerController.IsCanSave = false;
                _gameManager.isGameClear = true;
            }
            _playerController.CurrentState = PlayerState.Walk;
        }
    }

    private IEnumerator AutoExecuteAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // 3초 대기

        // PlayerHoldInteractionAction이 호출되지 않은 경우에만 실행
        if (_isInteractionActive && _playerController.IsCanSave && _targetPatient)
        {
            // gameManager.score += 1;
            TriggerPatientHeal();
            Destroy(_targetPatient);
            _playerController.IsCanSave = false;
            _gameManager.isGameClear = true;
            _playerController.CurrentState = PlayerState.Walk;
        }
    }

    void TriggerPatientHeal()
    {
        if (_healParticle != null)
        {
            ParticleSystem heal = Instantiate(_healParticle, _targetPatient.transform.position, Quaternion.identity);
            heal.Play();
            Destroy(heal.gameObject, 2f);
        }
    }
}

