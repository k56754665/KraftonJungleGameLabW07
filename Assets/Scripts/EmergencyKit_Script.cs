using UnityEngine;

public class EmergencyKit_Script : MonoBehaviour
{
    public Player_ScriptTEMP Player_ScriptTEMP; // 플레이어 가져오기

    private void OnTriggerEnter2D(Collider2D _collision)
    {
        

        if (_collision.gameObject.CompareTag("Player"))
        {
            AddEmergencyKitNum();
            Destroy(gameObject);
        }
    }

    

    // 플레이어가 구급상자와 충돌시, 플레이어의 보유 구급상자 갯수를 증가시키는 함수
    public void AddEmergencyKitNum()
    {
        Player_ScriptTEMP.playerKitNum += 1;
    }


    
}
