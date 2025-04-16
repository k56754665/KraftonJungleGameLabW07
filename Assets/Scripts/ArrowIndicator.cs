using System.Collections.Generic;
using UnityEngine;

public class ArrowIndicator : MonoBehaviour
{
    public Transform player; // 플레이어 참조
    public GameObject arrowPrefab; // 화살표 프리팹
    private List<GameObject> arrows = new List<GameObject>(); // 생성된 화살표 리스트
    private Camera cam;
    public float indicatorRange = 15f;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        UpdateArrowIndicators();
    }

    void UpdateArrowIndicators()
    {
        // 기존 화살표 삭제
        foreach (var arrow in arrows)
        {
            Destroy(arrow);
        }
        arrows.Clear();

        // 환자 찾기
        GameObject[] patients = GameObject.FindGameObjectsWithTag("Patient");

        foreach (GameObject patient in patients)
        {
            Vector3 screenPos = cam.WorldToViewportPoint(patient.transform.position);

            // 화면 밖에 있는 환자만 화살표 표시
            if (screenPos.x < 0 || screenPos.x > 1 || screenPos.y < 0 || screenPos.y > 1)
            {
                CreateArrow(patient.transform);
            }
        }
    }

    void CreateArrow(Transform target)
    {
        if (player) 
        {
            // 화살표 생성
            GameObject arrow = Instantiate(arrowPrefab, transform);
            arrows.Add(arrow);

            // 플레이어와 목표물 사이의 방향 계산
            Vector3 direction = (target.position - player.position).normalized;

            // 화살표를 화면 가장자리에 배치
            Vector3 arrowPosition = cam.WorldToViewportPoint(player.position + direction * indicatorRange);
            arrowPosition.x = Mathf.Clamp(arrowPosition.x, 0.05f, 0.95f);
            arrowPosition.y = Mathf.Clamp(arrowPosition.y, 0.05f, 0.95f);

            // 뷰포트 좌표 -> 월드 좌표 변환
            arrow.transform.position = cam.ViewportToWorldPoint(arrowPosition);
            arrow.transform.position = new Vector3(arrow.transform.position.x, arrow.transform.position.y, 0f);

            // 🔥 환자의 방향을 향하도록 회전
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            arrow.transform.rotation = Quaternion.Euler(0, 0, angle - 90f); // -90도 조정(화살표 모양에 맞게)
        }
    }
}
