using UnityEngine;

public class EnemyUIController_Script : MonoBehaviour
{
    public Transform enemy; // ���� Transform
    public Vector3 UIoffset = new Vector3(0, 150, 0); // �����κ����� ������

    void Update()
    {
        // ���� ��ġ�� �������� ���Ͽ� UI�� ��ġ�� ����
        Vector3 targetPosition = enemy.position + UIoffset;

        // UI�� ��ġ�� ������Ʈ
        transform.position = targetPosition;

        // UI�� ȸ���� �ʱ�ȭ�Ͽ� �׻� ������ �ٶ󺸰� ����
        transform.rotation = Quaternion.Euler(0, 0, 0);
    }
}
