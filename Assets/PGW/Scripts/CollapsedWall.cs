using UnityEngine;
using Define;
using System.Collections;

public class CollapsedWall : MonoBehaviour
{
    PlayerController _playerController;
    PlayerFire _playerFire;
    Vector2 _wallPosition1;
    Vector2 _wallPosition2;

    private void Start()
    {
        _playerController = GameObject.FindFirstObjectByType<PlayerController>();
        _playerFire = GameObject.FindFirstObjectByType<PlayerFire>();
        _wallPosition1 = transform.position + transform.up;
        _wallPosition2 = transform.position - transform.up;
    }

    public void Activate()
    {
        _playerController.CurrentState = PlayerState.Interaction;
        _playerFire.CanFire = false;

        // 플레이어와 벽 사이의 벡터 계산
        Vector2 playerPos = _playerController.transform.position;
        Vector2 wallToPlayer = playerPos - (Vector2)transform.position;

        // 벽의 업 방향으로 투영
        float dotProduct = Vector2.Dot(wallToPlayer, transform.up);

        // 목표 위치 설정
        Vector2 targetPosition = dotProduct > 0 ? _wallPosition2 : _wallPosition1;

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
        _playerController.CurrentState = PlayerState.Walk;
        _playerFire.CanFire = true;
    }
}
