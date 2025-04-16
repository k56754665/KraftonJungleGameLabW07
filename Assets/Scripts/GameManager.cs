using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [Header("Prefabs")]
    public Player player;
    public Transform playerTransform;
    public Canvas_Script canvas;
    private TextMeshProUGUI timerText;

    [Header("Enemy Manager")]
    public List<GameObject> enemies = new List<GameObject>(); // 모든 적 오브젝트를 List로 변경
    float deleteDistance = 40f; // 비활성화 거리

    [Header("Game System")]
    public bool isgameover = false;
    public bool isGameClear = false;
    public float startingTime = 60f; // 시작시간 초 단위
    private float timeRemaining;

    void Start()
    {
        timeRemaining = startingTime; // 남은 시간 초기화

        player = GameObject.FindFirstObjectByType<Player>();
        canvas = GameObject.FindFirstObjectByType<Canvas_Script>();
        timerText = canvas.timer.GetComponent<TextMeshProUGUI>();
        canvas.GetComponent<Canvas_Script>().gameOver.SetActive(false);
    }

    void Update()
    {
        // 적 상태 업데이트
        for (int i = enemies.Count - 1; i >= 0; i--) // 역방향으로 순회
        {
            if (enemies[i] != null) // 적이 파괴되지 않았을 경우
            {
                Enemy enemyController = enemies[i].GetComponent<Enemy>();
                if (enemyController != null)
                {
                    enemyController.NavMeshEnemyOnOff(playerTransform, deleteDistance);
                }
            }
            else
            {
                // 적이 파괴된 경우 리스트에서 제거
                enemies.RemoveAt(i);
            }
        }

        // 남은 시간이 0보다 큰 경우에만 감소
        if (timeRemaining > 0 && !isgameover)
        {
            timeRemaining -= Time.deltaTime; // 매 프레임 시간 감소
            UpdateTimerDisplay(); // 타이머 표시 업데이트
        }
        else
        {
            Gameover();
            timerText.text = "00:00";
        }

        if (isGameClear)
        {
            SceneManager.LoadScene("ClearMenu");
        }
    }


    public void Gameover()
    {
        Destroy(player.gameObject);
        StartCoroutine(DelayedGameOverActions());
    }

    void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60); // 분 계산
        int seconds = Mathf.FloorToInt(timeRemaining % 60); // 초 계산

        // "HH:mm" 형식으로 문자열 생성
        timerText.text = string.Format("{0:D2}:{1:D2}", minutes, seconds);
    }

    IEnumerator DelayedGameOverActions()
    {
        yield return new WaitForSeconds(1f); 

        isgameover = true;
        canvas.gameOver.SetActive(true);
    }

    private void OnApplicationFocus(bool focus)
    {
        Screen.SetResolution(1920, 1080, true);
    }

    private void OnApplicationQuit()
    {
        Screen.SetResolution(1920, 1080, true);
    }

}
