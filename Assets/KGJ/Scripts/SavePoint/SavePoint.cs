using UnityEngine;

public class SavePoint : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // 플레이어가 SavePoint에 닿았을 때
            // SavePointManager의 SavePoint를 호출하여 저장
            //SavePointManager.Instance.SavePoint();
            SavePointManager.Instance.OnSaveEvent?.Invoke();
            Debug.Log("Save Point Triggered");
            gameObject.SetActive(false);
        }
    }
}
