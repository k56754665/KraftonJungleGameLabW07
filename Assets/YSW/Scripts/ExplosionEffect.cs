using UnityEngine;

public class ExplosionEffect : MonoBehaviour
{
    [Header("Particle Systems")]
    public ParticleSystem mainExplosion;
    public ParticleSystem shockWave;
    public ParticleSystem sparks;

    [Header("Debug Settings")]
    public bool showDebugLog = true;
    public bool showGizmos = true;
    public Color gizmoColor = Color.yellow;

    private void DebugLog(string message)
    {
        if (showDebugLog)
        {
            Debug.Log($"[ExplosionEffect] {message}");
        }
    }

    void Awake()
    {
        DebugLog("Initializing ExplosionEffect...");

        // 메인 폭발 이펙트가 없다면 생성
        if (mainExplosion == null)
        {
            mainExplosion = GetComponent<ParticleSystem>();
            if (mainExplosion == null)
            {
                mainExplosion = gameObject.AddComponent<ParticleSystem>();
                DebugLog("Created new main explosion ParticleSystem");
            }
        }
        SetupMainExplosion();

        // 충격파 이펙트가 없다면 생성
        if (shockWave == null)
        {
            GameObject shockWaveObj = new GameObject("Shockwave");
            shockWaveObj.transform.parent = transform;
            shockWaveObj.transform.localPosition = Vector3.zero;
            shockWave = shockWaveObj.AddComponent<ParticleSystem>();
            DebugLog("Created new shockwave ParticleSystem");
        }
        SetupShockwave();

        // 불꽃 이펙트가 없다면 생성
        if (sparks == null)
        {
            GameObject sparksObj = new GameObject("Sparks");
            sparksObj.transform.parent = transform;
            sparksObj.transform.localPosition = Vector3.zero;
            sparks = sparksObj.AddComponent<ParticleSystem>();
            DebugLog("Created new sparks ParticleSystem");
        }
        SetupSparks();

        // 모든 파티클 시스템 재생
        DebugLog("Playing all particle systems...");
        mainExplosion.Play();
        shockWave.Play();
        sparks.Play();

        // 3초 후 자동 삭제
        Destroy(gameObject, 3f);
        DebugLog("Explosion will be destroyed in 3 seconds");
    }

    void Update()
    {
        if (showDebugLog)
        {
            if (mainExplosion != null && mainExplosion.isPlaying)
                DebugLog($"Main Explosion particles active: {mainExplosion.particleCount}");
            if (shockWave != null && shockWave.isPlaying)
                DebugLog($"Shockwave particles active: {shockWave.particleCount}");
            if (sparks != null && sparks.isPlaying)
                DebugLog($"Sparks particles active: {sparks.particleCount}");
        }
    }

    void OnDrawGizmos()
    {
        if (!showGizmos) return;

        // 메인 이펙트 범위
        if (mainExplosion != null)
        {
            Gizmos.color = gizmoColor;
            Gizmos.DrawWireSphere(transform.position, mainExplosion.main.startSize.constant);
        }

        // 충격파 범위
        if (shockWave != null)
        {
            Gizmos.color = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, 0.5f);
            Gizmos.DrawWireSphere(transform.position, shockWave.main.startSize.constant);
        }

        // 불꽃 범위
        if (sparks != null)
        {
            Gizmos.color = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, 0.3f);
            float sparkRange = sparks.main.startSpeed.constant * sparks.main.startLifetime.constant;
            Gizmos.DrawWireSphere(transform.position, sparkRange);
        }
    }

    void SetupMainExplosion()
    {
        DebugLog("Setting up main explosion...");
        var main = mainExplosion.main;
        main.duration = 0.5f;
        main.loop = false;
        main.startLifetime = 0.5f;
        main.startSpeed = 5f;
        main.startSize = 3f;
        main.gravityModifier = -0.1f;
        main.playOnAwake = false;
        main.simulationSpace = ParticleSystemSimulationSpace.World;

        var emission = mainExplosion.emission;
        emission.rateOverTime = 0;
        emission.burstCount = 1;  // 버스트 카운트 설정
        var burst = new ParticleSystem.Burst(0.0f, 50);
        emission.SetBurst(0, burst);

        var shape = mainExplosion.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.1f;

        // 색상 그라데이션 설정
        var colorOverLifetime = mainExplosion.colorOverLifetime;
        colorOverLifetime.enabled = true;
        
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { 
                new GradientColorKey(Color.yellow, 0.0f),
                new GradientColorKey(new Color(1f, 0.3f, 0f), 0.3f),
                new GradientColorKey(Color.red, 0.6f),
                new GradientColorKey(Color.grey, 1.0f)
            },
            new GradientAlphaKey[] { 
                new GradientAlphaKey(1.0f, 0.0f),
                new GradientAlphaKey(1.0f, 0.5f),
                new GradientAlphaKey(0.0f, 1.0f) 
            }
        );
        colorOverLifetime.color = gradient;

        // 크기 변화 설정
        var sizeOverLifetime = mainExplosion.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        AnimationCurve curve = new AnimationCurve();
        curve.AddKey(0.0f, 0.0f);
        curve.AddKey(0.5f, 1.0f);
        curve.AddKey(1.0f, 0.0f);
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1.0f, curve);

        // 파티클 렌더러 설정
        var renderer = mainExplosion.GetComponent<ParticleSystemRenderer>();
        renderer.renderMode = ParticleSystemRenderMode.Billboard;
        
        // 기본 파티클 머티리얼 사용
        renderer.material = new Material(Shader.Find("Legacy Shaders/Particles/Additive"));
        renderer.sortingLayerName = "Default";
        renderer.sortingOrder = 5;
    }

    void SetupShockwave()
    {
        DebugLog("Setting up shockwave...");
        var main = shockWave.main;
        main.duration = 0.3f;
        main.loop = false;
        main.startLifetime = 0.3f;
        main.startSpeed = 10f;
        main.startSize = 3f;
        main.gravityModifier = 0f;
        main.playOnAwake = false;
        main.simulationSpace = ParticleSystemSimulationSpace.World;

        var emission = shockWave.emission;
        emission.rateOverTime = 0;
        emission.burstCount = 1;  // 버스트 카운트 설정
        var burst = new ParticleSystem.Burst(0.0f, 30);
        emission.SetBurst(0, burst);

        var shape = shockWave.shape;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = 0.1f;

        // 파티클 렌더러 설정
        var renderer = shockWave.GetComponent<ParticleSystemRenderer>();
        renderer.renderMode = ParticleSystemRenderMode.Billboard;
        renderer.material = new Material(Shader.Find("Legacy Shaders/Particles/Additive"));
        renderer.sortingLayerName = "Default";
        renderer.sortingOrder = 4;
    }

    void SetupSparks()
    {
        DebugLog("Setting up sparks...");
        var main = sparks.main;
        main.duration = 1f;
        main.loop = false;
        main.startLifetime = 1f;
        main.startSpeed = 15f;
        main.startSize = 0.5f;
        main.gravityModifier = 0f;
        main.playOnAwake = false;
        main.simulationSpace = ParticleSystemSimulationSpace.World;

        var emission = sparks.emission;
        emission.rateOverTime = 0;
        emission.burstCount = 1;  // 버스트 카운트 설정
        var burst = new ParticleSystem.Burst(0.0f, 60);
        emission.SetBurst(0, burst);

        var shape = sparks.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.1f;

        // 파티클 렌더러 설정
        var renderer = sparks.GetComponent<ParticleSystemRenderer>();
        renderer.renderMode = ParticleSystemRenderMode.Billboard;
        renderer.material = new Material(Shader.Find("Legacy Shaders/Particles/Additive"));
        renderer.sortingLayerName = "Default";
        renderer.sortingOrder = 6;
    }
} 