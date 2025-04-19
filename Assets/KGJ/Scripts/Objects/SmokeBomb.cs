using System.Collections;
using UnityEngine;

public class SmokeBomb : MonoBehaviour
{
    [Header("Bomb Settings")]
    [SerializeField] float maxSize = 10f;  // 폭발 범위
    [SerializeField] float sizeupDuration = 0.75f;
    [SerializeField] float destroyDuration = 5f;
    GameObject explosionEffect;  // 폭발 이펙트 프리팹
    GameObject effectGameObject;
    Transform _smokeArea;


    void Start()
    {
        // 콜라이더 설정
        _smokeArea = transform.GetChild(0);
        explosionEffect = Resources.Load<GameObject>("Particles/Explosion_Particle");
        effectGameObject = Instantiate(explosionEffect, transform.position, transform.rotation);
        StartCoroutine(ScaleUp());
        
        Destroy(gameObject, destroyDuration);
    }

    IEnumerator ScaleUp()
    {
        Vector3 startScale = _smokeArea.localScale;
        Vector3 targetScale = new Vector3(maxSize, maxSize, 1f);
        float time = 0f;

        while (time < sizeupDuration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / sizeupDuration); // 0에서 1로 보간값
            _smokeArea.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }

        // 혹시 모를 오차 제거
        _smokeArea.localScale = targetScale;
        Destroy(effectGameObject);
    }
}
