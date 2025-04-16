using System.Collections;
using UnityEngine;

public class RedZoneFire : MonoBehaviour
{
    public float appearTime = 3f; // 레드존이 완전히 활성화될 시간
    public float dangerTime = 0.1f; // 활성화된 후 유지되는 시간
    private SpriteRenderer spriteRenderer;
    private Collider2D col;
    public ParticleSystem explosionParticle;
    private float deleteExplosion = 5f;
    private FollowPlayer cameraShake;

    public GameObject redZoneBound;
    private bool isGrowing = true;

    public bool isboom = false;

    float testInt = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();

        cameraShake = Camera.main.GetComponent<FollowPlayer>();

        StartCoroutine(ActivateRedZone());

        redZoneBound.transform.localScale = new Vector3(size,size,1);
    }

    IEnumerator ActivateRedZone()
    {
        // 초기 상태: 반투명 & 충돌 비활성화
        Color color = spriteRenderer.color;
        color.a = 0.3f;
        spriteRenderer.color = color;
        col.enabled = false;

        yield return new WaitForSeconds(appearTime);

        // 레드존 활성화
        color.a = 1f;
        spriteRenderer.color = color;
        col.enabled = true;
        TriggerExplosion();

        if (cameraShake != null)
        {
            StartCoroutine(cameraShake.CameraShake(0.5f, 0.3f));
        }

        // 크기 증가가 완료될 때까지 대기
        while (isGrowing)
        {
            yield return null;
        }

        // dangerTime 후에 레드존 비활성화
        yield return new WaitForSeconds(dangerTime);
        DestroyObjects();
        Destroy(gameObject);
    }

    private void DestroyObjects()
    {
        if (redZoneBound != null)
        {
            Destroy(redZoneBound);
        }
    }


    int size = 10;
    // Update is called once per frame
    void Update()
    {
        if (isGrowing && testInt < size)
        {
            testInt += Time.deltaTime * size / 3;
            gameObject.transform.localScale = new Vector3(testInt, testInt, testInt);
        }
        else
        {
            testInt = size;
            isGrowing = false; // 크기 증가가 완료되면 플래그를 false로 설정
        }

        if (isboom)
        {
            Color c = GetComponent<SpriteRenderer>().color;
            c.a -= 0.5f * Time.deltaTime;
            GetComponent<SpriteRenderer>().color = c;
        }
        else 
        {
            Color c = GetComponent<SpriteRenderer>().color;
            c.a += 0.2f * Time.deltaTime;
            GetComponent<SpriteRenderer>().color = c;
        }
    }

    void TriggerExplosion()
    {
        if (explosionParticle != null)
        {
            // 폭발 파티클 실행
            ParticleSystem explosion = Instantiate(explosionParticle, transform.position, Quaternion.identity);
            explosion.Play();
            Destroy(explosion.gameObject, deleteExplosion);
            isboom = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            DestroyObjects();
            Destroy(gameObject);
            Destroy(collision.gameObject);
            GameManager gm = GameObject.FindFirstObjectByType<GameManager>();
            gm.Gameover();
        }

        if (collision.CompareTag("Enemy"))
        {
            DestroyObjects();
            Destroy(gameObject);
            Destroy(collision.gameObject);
        }
    }
}
