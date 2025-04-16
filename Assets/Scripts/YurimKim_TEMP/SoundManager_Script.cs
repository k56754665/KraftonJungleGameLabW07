using UnityEngine;

public class SoundManager_Script : MonoBehaviour
{
    public static SoundManager_Script Instance;
    private AudioSource audioSource;

    // 사용할 Audioclip 리스트
    public AudioClip sound1; 

    private void Awake()
    {
        // 싱글톤 패턴 구현
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬이 변경되어도 파괴되지 않게
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Start()
    {
        // 사운드매니저 오디오소스 불러오기
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void PlaySFX_PlayerShoot()
    {
        audioSource.PlayOneShot(sound1);
    }
}
