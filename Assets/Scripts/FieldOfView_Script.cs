using UnityEngine;
using CodeMonkey.Utils;

public class FieldOfView_Script : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask;
    private Mesh mesh;
    private float fov;
    private Vector3 origin;
    private float startingAngle;
    private float viewDistance;

    private void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        viewDistance = 50f;
        fov = 90f;
        origin = Vector3.zero;
    }

    // 흔들림 방지를 위해 LateUpdate로 시야각 업데이트
    private void LateUpdate()
    {
        int rayCount = 200;
        float angle = startingAngle;
        float angleIncrease = fov / rayCount;

        Vector3[] vertices = new Vector3[rayCount + 1 + 1];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[rayCount * 3];

        vertices[0] = origin;

        int vertexIndex = 1;
        int triangleIndex = 0;
        for (int i = 0; i <= rayCount; i++)
        {
            Vector3 vertex;
            RaycastHit2D raycastHit2D = Physics2D.Raycast(origin, UtilsClass.GetVectorFromAngle(angle), viewDistance, layerMask);
            
            if (raycastHit2D.collider == null)
            {
                // 장애물 안 부딪힘
                vertex = origin + UtilsClass.GetVectorFromAngle(angle) * viewDistance;
            } else
            {
                // 장애물과 닿음
                vertex = raycastHit2D.point;
            }
            vertices[vertexIndex] = vertex;

            if (i > 0)
            {
                triangles[triangleIndex + 0] = 0;
                triangles[triangleIndex + 1] = vertexIndex - 1;
                triangles[triangleIndex + 2] = vertexIndex;

                triangleIndex += 3;
            }

            vertexIndex++;
            angle -= angleIncrease;
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.bounds = new Bounds(origin, Vector3.one * 1000f);

        mesh.RecalculateBounds();
    }


    public void SetOrigin(Vector3 _origin)
    {
        this.origin = _origin;
    }

    public void SetAimDirection(Vector3 _aimDirection)
    {
        startingAngle = UtilsClass.GetAngleFromVectorFloat(_aimDirection) - fov / 2f;
    }
}
