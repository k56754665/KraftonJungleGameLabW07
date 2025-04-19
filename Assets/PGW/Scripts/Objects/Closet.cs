using Define;
using UnityEngine;
using System.Collections;

public class Closet : MonoBehaviour
{
    PlayerController _playerController;
    PlayerFire _playerFire;
    PlayerInteraction _playerInteraction;

    bool _isMoveInCloset = false; // 플레이어가 옷장 안에 있는지 여부

    private void Start()
    {
        _playerController = GameObject.FindFirstObjectByType<PlayerController>();
        _playerFire = GameObject.FindFirstObjectByType<PlayerFire>();
        _playerInteraction = GameObject.FindFirstObjectByType<PlayerInteraction>();
    }

    public void Activate()
    {
        if (_isMoveInCloset) return; // 이미 옷장 안에 있는 경우
        _isMoveInCloset = true; // 옷장 안으로 이동 중
        

        // 목표 위치 설정
        Vector2 targetPosition;
        if (!_playerInteraction.IsInCloset)
        {
            _playerController.CurrentState = PlayerState.Interaction;
            _playerController.GetComponent<CircleCollider2D>().enabled = false;
            _playerFire.CanFire = false;
            targetPosition = transform.position;
            _playerInteraction.IsInCloset = true;
        }
        else
        {
            targetPosition = transform.position + transform.up * 3f;
            _playerInteraction.IsInCloset = false;
        }
        // 코루틴으로 부드러운 이동 시작
        StartCoroutine(MovePlayerSmoothly(targetPosition));
    }

    IEnumerator MovePlayerSmoothly(Vector2 targetPosition)
    {
        float duration = 1f; // 1초 동안 이동
        float elapsed = 0f;
        Vector2 startPosition = _playerController.transform.position;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            _playerController.transform.position = Vector2.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

        // 최종 위치 정확히 설정
        _playerController.transform.position = targetPosition;
        _playerController.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero; // 속도 초기화
        if (!_playerInteraction.IsInCloset)
        {
            _playerController.CurrentState = PlayerState.Walk;
            _playerController.GetComponent<CircleCollider2D>().enabled = true;
            _playerFire.CanFire = true;
            _playerInteraction.IsInCloset = false;
        }
        else 
        {
            _playerController.CurrentTarget = transform.gameObject;
            _playerController.TargetType = Target.Object;
            _playerInteraction.IsInCloset = true;
        }
        _isMoveInCloset = false; // 이동 완료
        
    }
}
