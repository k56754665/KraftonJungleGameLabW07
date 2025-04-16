using UnityEngine;

public class FloatingText_Script : MonoBehaviour
{
    public float textMoveRange = 0.5f; // 움직임의 범위
    public float textMoveSpeed = 1.0f;  // 움직임의 속도

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position; // 초기 위치 저장
    }

    void Update()
    {
        float newY = startPosition.y + Mathf.Sin(Time.time * textMoveSpeed) * textMoveRange;
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }
}
