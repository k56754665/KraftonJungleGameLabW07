using UnityEngine;

public class DrawLine : MonoBehaviour
{
    public LineRenderer lineRenderer; // LineRenderer 컴포넌트
    public float lineLength = 5f; // 선의 길이
    public Vector3 startPoint; // 선의 시작점

    private void Start()
    {
        // LineRenderer에 점 설정
        lineRenderer.positionCount = 2; // 두 점으로 선을 만듭니다.
        startPoint = transform.position; // 현재 위치를 시작점으로 설정
        DrawLineSegment();
    }

    void DrawLineSegment()
    {
        // 시작점과 끝점 설정
        Vector3 endPoint = startPoint + transform.up * lineLength; // 위 방향으로 선을 긋습니다.
        lineRenderer.SetPosition(0, startPoint); // 시작점
        lineRenderer.SetPosition(1, endPoint); // 끝점
    }
}
