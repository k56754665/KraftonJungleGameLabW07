using Define;
using UnityEngine;
using System.Collections;

public class Closet : MonoBehaviour
{
    PlayerController _playerController;
    PlayerFire _playerFire;
    bool _isPlayerInCloset = false;

    private void Start()
    {
        _playerController = GameObject.FindFirstObjectByType<PlayerController>();
        _playerFire = GameObject.FindFirstObjectByType<PlayerFire>();
    }

    public void Activate()
    {
        // 목표 위치 설정
        Vector2 targetPosition;
        if (!_isPlayerInCloset)
        {
            _playerController.CurrentState = PlayerState.Interaction;
            _playerController.GetComponent<CircleCollider2D>().enabled = false;
            _playerFire.CanFire = false;
            targetPosition = transform.position;
        }
        else
        {
            targetPosition = transform.position + transform.up * 3f;
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
        if(_isPlayerInCloset)
        {
            _playerController.CurrentState = PlayerState.Walk;
            _playerController.GetComponent<CircleCollider2D>().enabled = true;
            _playerFire.CanFire = true;
            _isPlayerInCloset = false;
        }
        else { _isPlayerInCloset = true; }
    }
}
