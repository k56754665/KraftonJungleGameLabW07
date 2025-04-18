using UnityEngine;
using Define;
using UnityEngine.UIElements;
public class Soundwave_Script : MonoBehaviour
{
    [Header("Soundwave")]
    public float increaseSpeed = 0.5f;
    public float decreaseSpeed = 0.1f;
    public float changeTime = 1f;
    public bool isIncreasing = true; // 상태를 명확하게 관리
    private Vector3 soundwaveScale;

    [Header("Player Transform")]
    public Transform playerTransform;

    private void Start()
    {
        // 초기 scale 설정
        soundwaveScale = Vector3.zero;
        this.transform.localScale = soundwaveScale;
        Invoke("StartDecrease", changeTime); // 일정 시간 후 줄어들기 시작
    }

    private void Update()
    {
        // 음파 크기 변화
        if (isIncreasing)
        {
            soundwaveScale += new Vector3(increaseSpeed * Time.deltaTime, increaseSpeed * Time.deltaTime, increaseSpeed * Time.deltaTime);
        }
        else
        {
            soundwaveScale -= new Vector3(decreaseSpeed * Time.deltaTime, decreaseSpeed * Time.deltaTime, decreaseSpeed * Time.deltaTime);
        }

        // 스케일이 너무 작아지지 않도록 제한
        if (soundwaveScale.x < 0.1f && !isIncreasing)
        {
            soundwaveScale = Vector3.zero; // 0으로 초기화
            DeleteSoundwave();
        }

        // 스케일 적용
        this.transform.localScale = soundwaveScale;
    }

    void StartDecrease()
    {
        isIncreasing = false; // 줄어드는 상태로 전환
    }

    public void DeleteSoundwave()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 적이 음파에 닿았을 때
        if (other.CompareTag("Enemy"))
        {
            //Debug.Log("들린다들린다~!");

            // 적 정보 가져온 후, 해당 적이 Checking 상태로 변함
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.currentState = EnemyState.Checking;
                enemy.SoundwavePosition = transform.position;
                enemy.MoveToCurrentSoundwave();
            }
        }
    }

}
