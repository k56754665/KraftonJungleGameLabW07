using UnityEngine;
using TMPro;
using System.Collections.Generic;
using Define;

public class EnemyDialogue : MonoBehaviour
{
    [Header("Text Offset")]
    [SerializeField] private Vector3 textOffset = new Vector3(0, 2, 0); // 적으로부터의 오프셋

    [Header("Dialogue Texts")]
    [SerializeField] List<string> patrollingDialogue;
    [SerializeField] List<string> chasingDialogue;
    [SerializeField] List<string> searchingDialogue;
    [SerializeField] List<string> stunningDialogue;
    [SerializeField] List<string> checkingDialogue;

    private TextMeshPro dialogueText; // 대화 텍스트
    private Transform enemy; // 적의 Transform
    private EnemyState lastState = EnemyState.Chasing; // 이전 적의 상태 저장(일단 처음부터 절대 될리 없는 Chasing으로 설정)


    private void Awake()
    {
        dialogueText = GetComponent<TextMeshPro>();

        // 부모 오브젝트에서 enemy 태그 찾기
        if (transform.parent != null)
        {
            if (transform.parent.CompareTag("Enemy"))
            {
                enemy = transform.parent;
            }
            else
            {
                Debug.Log("부모 오브젝트가 Enemy 태그를 가지고 있지 않습니다.");
            }
        }
    }

    // 흔들림 방지를 위해 LateUpdate에서 UI 위치 업데이트
    private void LateUpdate()
    {
        UpdateUIPosition();
    }


    public void UpdateUIPosition()
    {
        if (enemy == null) return;

        // 적의 위치에 오프셋을 더하여 UI의 위치를 설정
        Vector3 targetPosition = enemy.position + textOffset;
        transform.position = targetPosition;

        // UI의 회전을 초기화하여 항상 정면을 바라보게 설정
        transform.rotation = Quaternion.Euler(0, 0, 0);
    }


    public void DialogueTalking(EnemyState _enemyState)
    {
        // 상태가 변경되었는지 확인
        if (_enemyState != lastState)
        {
            // 상태 변경 시 대사 갱신
            switch (_enemyState)
            {
                case EnemyState.Patrolling:
                    dialogueText.text = patrollingDialogue[Random.Range(0, patrollingDialogue.Count)];
                    break;
                case EnemyState.Chasing:
                    dialogueText.text = chasingDialogue[Random.Range(0, chasingDialogue.Count)];
                    break;
                case EnemyState.Searching:
                    dialogueText.text = searchingDialogue[Random.Range(0, searchingDialogue.Count)];
                    break;
                case EnemyState.Stunning:
                    dialogueText.text = stunningDialogue[Random.Range(0, stunningDialogue.Count)];
                    break;
                case EnemyState.Checking:
                    dialogueText.text = checkingDialogue[Random.Range(0, checkingDialogue.Count)];
                    break;
            }

            // 이전 상태 업데이트
            lastState = _enemyState; 
        }
    }

}
