using UnityEngine;

public class Heart_Script : MonoBehaviour
{
    public PlayerController player;

    public GameObject[] pinkHearts; // 분홍 하트 오브젝트 배열
    public GameObject[] grayHearts; // 회색 하트 오브젝트 배열

    void Start()
    {
        player = GameObject.FindFirstObjectByType<PlayerController>();
        UpdateHearts(); // 하트 상태 업데이트
    }

    private void Update()
    {
        UpdateHearts();
    }

    // 하트 상태 업데이트 메서드
    private void UpdateHearts()
    {
        for (int i = 0; i < player.maxHp; i++)
        {
            // 분홍 하트와 회색 하트의 활성화 상태를 설정
            if (i < player.hp)
            {
                pinkHearts[i].SetActive(true); // 분홍 하트 켜기
                grayHearts[i].SetActive(false); // 회색 하트 끄기
            }
            else
            {
                pinkHearts[i].SetActive(false); // 분홍 하트 끄기
                grayHearts[i].SetActive(true); // 회색 하트 켜기
            }
        }
    }
}
