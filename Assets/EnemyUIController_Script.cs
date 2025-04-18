using UnityEngine;

public class EnemyUIController_Script : MonoBehaviour
{
    public Transform enemy; // 적의 Transform
    public Vector3 UIoffset = new Vector3(0, 150, 0); // 적으로부터의 오프셋
    Canvas _canvas;

    void Start()
    {
        _canvas = GetComponent<Canvas>();
    }

    public void ShowUI()
    {
        _canvas.enabled = true;
    }

    public void HideUI()
    {
        _canvas.enabled = false;
    }
    void Update()
    {
        if (enemy == null) return;

        // 적의 위치에 오프셋을 더하여 UI의 위치를 설정
        Vector3 targetPosition = enemy.position + UIoffset;

        // UI의 위치를 업데이트
        transform.position = targetPosition;

        // UI의 회전을 초기화하여 항상 정면을 바라보게 설정
        transform.rotation = Quaternion.Euler(0, 0, 0);
    }
}
