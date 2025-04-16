using UnityEngine;

public class Player_ScriptTEMP : MonoBehaviour
{
    public ParticleSystem healingParticle;


    public int patientTimer = 50; // 환자의 타이머
    public int patientTimerMax = 100; // 환자의 최대 타이머
    public int healScore = 100; // 환자 치료시 올라가는 점수
    public int playerKitNum = 0; // 플레이어가 현재 보유한 구급상자 갯수
    public int score = 123; // 점수(임시...)


    private void OnTriggerEnter2D(Collider2D _collision)
    {
        // 환자와 충돌 시 HealPatient 실행
        if (_collision.gameObject.CompareTag("Patient"))
        {
            HealPatient(_collision.gameObject);
        }
    }

    // 플레이어가 구급상자 보유 시, (환자와 충돌하면) 환자를 치료하고, 파티클을 만든 뒤, 환자를 제거하는 함수
    public void HealPatient(GameObject patientGameObject)
    {
        if (playerKitNum > 0)
        {
            AddScore(healScore);
            Instantiate(healingParticle, transform.position, transform.rotation);
            Destroy(patientGameObject);
        }
    }

    // 점수를 올리는 함수
    public void AddScore(int _score)
    {
        score += _score;
    }


}


