using UnityEngine;

public class FoVEnemyColor_Script : MonoBehaviour
{
    public Material white;
    public Material yellow;
    public Material red;
    
    public Enemy enemy;

    void Start()
    {
        // 현재 오브젝트의 Material을 변경
        ChangeFoVColor(white);
    }

    public void ChangeFoVColor(Material _mat)
    {
        // Renderer 컴포넌트를 가져옴
        Renderer renderer = GetComponent<Renderer>();

        // Renderer가 존재하는지 확인하면 현재 Material을 새로운 Material로 변경
        if (renderer != null)
        {
            renderer.material = _mat;
        }
    }
}
