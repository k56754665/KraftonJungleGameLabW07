using UnityEngine;
using UnityEngine.AI;

public class NavAgent2D : MonoBehaviour
{
    NavMeshAgent agent;

    private void Awake()
    {
        // agent 본체만 붙게 하고 렌더러요소는 그대로
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }
}
