using UnityEngine;
using CodeMonkey.Utils;

public class FieldOfViewEnemy_Script : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask;

    private Mesh mesh;
    private Vector3 origin;
    private float startingAngle;

    // 시야각, 시야범위, 적 참조 변수
    public float fov;
    public float viewDistance;
    public Enemy enemy;

    private void Start()
    {
        int layer1 = LayerMask.GetMask("Field Of View Object");
        int layer2 = LayerMask.GetMask("Smoke");
        layerMask = layer1 | layer2;
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        origin = Vector3.zero;
    }


    private void LateUpdate()
    {
        // 흔들림 방지를 위해 LateUpdate로 시야각 업데이트
        VisualizeFOV();
    }

    public void VisualizeFOV()
    {
        int rayCount = 50;
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
            Vector3 direction = UtilsClass.GetVectorFromAngle(angle);
            RaycastHit2D raycastHit2D = Physics2D.Raycast(enemy.transform.position, direction, viewDistance, layerMask);

            if (raycastHit2D.collider == null)
            {
                // 장애물 없음
                vertex = direction * viewDistance; // 로컬 좌표 기준
            }
            else
            {
                vertex = raycastHit2D.point - (Vector2)transform.position; // 월드 좌표 차이로 로컬 좌표 계산
                //Debug.Log($"Ray {i}: Hit={raycastHit2D.collider?.name}, Point={raycastHit2D.point}, Vertex={vertex}");
            }
            vertices[vertexIndex] = new Vector3(vertex.x, vertex.y, 0); // Z축 0으로 고정

            if (i > 0)
            {
                triangles[triangleIndex + 0] = 0;
                triangles[triangleIndex + 1] = vertexIndex - 1;
                triangles[triangleIndex + 2] = vertexIndex;

                triangleIndex += 3;
            }

            vertexIndex++;
            angle += angleIncrease; // 시계 방향

            Debug.DrawRay(transform.position, direction * viewDistance, Color.green, 0.1f);
            //Debug.Log($"Ray {i}: Angle={angle}, Direction={direction}");
        }

        // Vertex 디버깅: 정점과 연결 선 시각화
        for (int i = 1; i < vertices.Length; i++)
        {
            Vector3 worldVertex = transform.TransformPoint(vertices[i]);
            Debug.DrawLine(transform.position, worldVertex, Color.red, 0.1f);
            if (i < vertices.Length - 1)
            {
                Vector3 worldVertexNext = transform.TransformPoint(vertices[i + 1]);
                Debug.DrawLine(worldVertex, worldVertexNext, Color.blue, 0.1f); // 정점 간 연결
            }
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.bounds = new Bounds(origin, Vector3.one * 1000f);

        mesh.RecalculateBounds();
        mesh.RecalculateNormals(); // 노멀 재계산으로 렌더링 문제 방지

        // 디버깅 로그
        /*Debug.Log($"FOV Position: {transform.position}, LocalPosition: {transform.localPosition}, Origin: {origin}, " +
                  $"VertexCount: {vertices.Length}, FirstVertex: {vertices[1]}, LastVertex: {vertices[vertices.Length - 1]}, " +
                  $"StartingAngle: {startingAngle}, AngleIncrease: {angleIncrease}");
        */
    }

    public void SetOrigin(Vector3 _origin)
    {
        this.origin = _origin; // 로컬 좌표로 설정
    }

    public void SetAimDirection(Vector3 _aimDirection)
    {
        _aimDirection.Normalize();
        float angle = UtilsClass.GetAngleFromVectorFloat(_aimDirection);
        startingAngle = angle - fov / 2f;
        //Debug.Log($"AimDirection: {_aimDirection}, Angle: {angle}, StartingAngle: {startingAngle}, FOV: {fov}, EnemyRotationZ: {transform.eulerAngles.z}");
    }


    public void FoVTurnOnOff(bool _bool)
    {
        this.gameObject.SetActive(_bool);
    }

    public void DestroyFOV()
    {
        Destroy(gameObject);
    }
}
