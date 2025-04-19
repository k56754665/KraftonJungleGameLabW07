using UnityEngine;

public class PlayerGunPosition : MonoBehaviour
{
    [SerializeField] float radius = 0.25f;

    void Update()
    {
        // 마우스 위치 가져오기
        Vector3 mousePosition = InputManager.Instance.PointerMoveInput;

        // 마우스에서 중심점으로의 방향 벡터 계산
        Vector2 direction = (mousePosition - transform.position).normalized;

        // 회전 각도 계산
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // 오브젝트의 회전 설정 (마우스 방향을 따라 Z축 회전)
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));

        // 중심점에서 반지름만큼 떨어진 위치 계산
        Vector3 offset = new Vector3(direction.x, direction.y, 0) * radius;
        transform.position = transform.parent.transform.position + offset;
    }
}
