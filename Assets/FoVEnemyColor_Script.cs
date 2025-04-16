using UnityEngine;

public class FoVEnemyColor_Script : MonoBehaviour
{
    public Material white;
    public Material yellow;
    public Material red;
    
    public Enemy enemy;

    void Start()
    {
        // ���� ������Ʈ�� Material�� ����
        ChangeFoVColor(white);
    }

    public void ChangeFoVColor(Material _mat)
    {
        // Renderer ������Ʈ�� ������
        Renderer renderer = GetComponent<Renderer>();

        // Renderer�� �����ϴ��� Ȯ���ϸ� ���� Material�� ���ο� Material�� ����
        if (renderer != null)
        {
            renderer.material = _mat;
        }
    }
}
