using UnityEngine;
using System.Collections;

public class FollowPlayer : MonoBehaviour
{
    public GameObject player;
    public Vector2 minBounds; // 맵 최소 좌표
    public Vector2 maxBounds; // 맵 최대 좌표
    private Vector3 offset = new Vector3(0, 0, -10);

    private Camera cam;
    private float halfHeight;
    private float halfWidth;
    private bool isShaking = false; // 카메라 흔들림 상태 체크
    private Vector3 shakeOffset = Vector3.zero; // 흔들림 오프셋 저장

    void Start()
    {
        cam = Camera.main;
        halfHeight = cam.orthographicSize;
        halfWidth = halfHeight * cam.aspect;
    }

    void LateUpdate()
    {
        if (player == null) return;

        Vector3 targetPosition = player.transform.position + offset;

        float clampedX = Mathf.Clamp(targetPosition.x, minBounds.x + halfWidth, maxBounds.x - halfWidth);
        float clampedY = Mathf.Clamp(targetPosition.y, minBounds.y + halfHeight, maxBounds.y - halfHeight);

        // 카메라 이동 시 흔들림 오프셋을 추가
        transform.position = new Vector3(clampedX, clampedY, targetPosition.z) + shakeOffset;
    }

    // 📌 카메라 흔들기 효과 (자연스럽게 움직이면서 흔들림 적용)
    public IEnumerator CameraShake(float duration, float magnitude)
    {
        isShaking = true; // 흔들리는 중

        float elapsed = 0f;

        while (elapsed < duration)
        {
            // 현재 위치 기준으로 흔들림 적용
            float offsetX = Random.Range(-1f, 1f) * magnitude;
            float offsetY = Random.Range(-1f, 1f) * magnitude;
            shakeOffset = new Vector3(offsetX, offsetY, 0); // 오프셋 저장

            elapsed += Time.deltaTime; //경과된 시간 기록
            yield return null;
        }

        // 흔들림 종료 후 오프셋 초기화
        shakeOffset = Vector3.zero;
        isShaking = false;
    }
}
