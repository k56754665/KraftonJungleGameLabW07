using UnityEngine;

public class Bomb : MonoBehaviour
{
    [Header("Bomb Settings")]
    public float explosionRadius = 5f;  // 폭발 범위
    public int damage = 3;              // 폭발 데미지
    public GameObject explosionEffect;   // 폭발 이펙트 프리팹

    [Header("Debug")]
    public bool showExplosionRange = true;
    public Color rangeColor = Color.red;

    private CircleCollider2D bombCollider;

    private void Start()
    {
        // 콜라이더 설정
        bombCollider = GetComponent<CircleCollider2D>();
        if (bombCollider != null)
        {
            // 콜라이더 크기를 약간 키워서 충돌 감지 확률 증가
            bombCollider.radius *= 1.2f;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 총알에 맞았을 때 폭발
        if (collision.gameObject.CompareTag("Bullet"))
        {
            Explode();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 충돌 방식으로도 체크
        if (collision.gameObject.CompareTag("Bullet"))
        {
            Explode();
        }
    }

    private void Explode()
    {
        // 폭발 범위 내의 모든 콜라이더 검출
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (Collider2D nearbyObject in colliders)
        {
            // 플레이어가 범위 안에 있다면 데미지 처리
            PlayerController player = nearbyObject.GetComponent<PlayerController>();
            if (player != null)
            {
                player.hp -= damage;
            }

            // 적이 범위 안에 있다면 데미지 처리
            Enemy enemy = nearbyObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.hp -= damage;
                // 빨간 데미지 파티클 재생 (Enemy 스크립트에 있는 기능 활용)
                enemy.DamagedBullet("Red");
            }
        }

        // 폭발 이펙트 생성
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, transform.rotation);
        }

        // 폭탄 오브젝트 제거
        Destroy(gameObject);
    }

    // 디버그용 - 폭발 범위를 씬 뷰에서 시각화
    private void OnDrawGizmos()
    {
        if (showExplosionRange)
        {
            // 폭발 범위 표시
            Gizmos.color = rangeColor;
            Gizmos.DrawWireSphere(transform.position, explosionRadius);

            // 충돌 범위 표시 (더 진한 색으로)
            if (bombCollider != null)
            {
                Gizmos.color = new Color(rangeColor.r, rangeColor.g, rangeColor.b, 0.8f);
                Gizmos.DrawWireSphere(transform.position, bombCollider.radius);
            }
        }
    }
} 