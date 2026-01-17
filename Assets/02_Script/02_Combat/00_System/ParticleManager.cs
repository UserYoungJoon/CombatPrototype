using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum ParticleEaseType
{
    Linear,
    EaseIn,
    EaseOut,
    EaseInOut
}

[System.Serializable]
public class BasicParticleConfig
{
    [Header("Emission")]
    public int particleCount = 20;

    [Header("Lifetime")]
    public float lifetime = 0.5f;

    [Header("Speed & Distance")]
    public float startSpeed = 10f;
    public float maxDistance = 3f;

    [Header("Cone Shape")]
    [Range(0f, 90f)]
    public float coneAngle = 25f;

    [Header("Size")]
    public float startSize = 0.15f;
    public float endSize = 0.05f;

    [Header("Color")]
    public Color colorMin = new Color(1f, 0.9f, 0.2f, 1f);   // Yellow
    public Color colorMax = new Color(1f, 0.5f, 0f, 1f);     // Orange
    public float fadeOutAlpha = 0f;                           // 끝날 때 투명도

    [Header("Easing")]
    public ParticleEaseType speedEase = ParticleEaseType.EaseOut;
    public ParticleEaseType sizeEase = ParticleEaseType.Linear;
    public ParticleEaseType alphaEase = ParticleEaseType.EaseIn;
}

public class ParticleHandle
{
    private ParticleSystem particleSystem;
    private GameObject gameObject;

    public ParticleHandle(ParticleSystem ps)
    {
        particleSystem = ps;
        gameObject = ps.gameObject;
    }

    public void Pause() => particleSystem.Pause();
    public void Play() => particleSystem.Play();
    public void Stop() => particleSystem.Stop();
    public bool IsAlive() => particleSystem.IsAlive();
    public bool IsPaused() => particleSystem.isPaused;

    public void Destroy()
    {
        if (gameObject != null)
            Object.Destroy(gameObject);
    }
}

public class ParticleManager : ManagerBase
{
    [Header("Base Particle Config")]
    [SerializeField] private BasicParticleConfig baseConfig = new();
    private List<ParticleHandle> activeParticles = new();

    private AnimationCurve GetEaseCurve(ParticleEaseType easeType, float startValue, float endValue)
    {
        return easeType switch
        {
            ParticleEaseType.Linear => AnimationCurve.Linear(0f, startValue, 1f, endValue),
            ParticleEaseType.EaseIn => new AnimationCurve(
                new Keyframe(0f, startValue, 0f, 0f),
                new Keyframe(1f, endValue, 2f * (endValue - startValue), 0f)
            ),
            ParticleEaseType.EaseOut => new AnimationCurve(
                new Keyframe(0f, startValue, 2f * (endValue - startValue), 0f),
                new Keyframe(1f, endValue, 0f, 0f)
            ),
            ParticleEaseType.EaseInOut => AnimationCurve.EaseInOut(0f, startValue, 1f, endValue),
            _ => AnimationCurve.Linear(0f, startValue, 1f, endValue)
        };
    }

    private bool wasPaused = false;

    private void Update()
    {
        // Stop Motion 중이면 파티클 일시정지
        bool isStopMotion = GameManager.Instance.CinematicManager.IsStopMotion;
        if (isStopMotion && !wasPaused)
        {
            PauseAll();
            wasPaused = true;
        }
        else if (!isStopMotion && wasPaused)
        {
            ResumeAll();
            wasPaused = false;
        }

        // Clean up dead particles
        for (int i = activeParticles.Count - 1; i >= 0; i--)
        {
            if (!activeParticles[i].IsAlive())
            {
                activeParticles[i].Destroy();
                activeParticles.RemoveAt(i);
            }
        }
    }

    public ParticleHandle PlayBasicEffect(Vector3 hitPos, Vector3 direction)
    {
        return PlayBasicEffect(hitPos, direction, baseConfig);
    }

    public ParticleHandle PlayBasicEffect(Vector3 hitPos, Vector3 direction, BasicParticleConfig config)
    {
        GameObject particleObj = new GameObject("BasicHitEffect");
        particleObj.transform.position = hitPos;
        particleObj.transform.rotation = Quaternion.LookRotation(direction);

        ParticleSystem ps = particleObj.AddComponent<ParticleSystem>();

        // Stop to configure
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        // Main module
        var main = ps.main;
        main.duration = 0.1f;
        main.loop = false;
        main.startLifetime = config.lifetime;
        main.startSpeed = config.startSpeed;
        main.startSize = config.startSize;
        main.startColor = new ParticleSystem.MinMaxGradient(config.colorMin, config.colorMax);
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.maxParticles = config.particleCount;


        // Emission module - burst
        var emission = ps.emission;
        emission.enabled = true;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[]
        {
            new ParticleSystem.Burst(0f, config.particleCount)
        });

        // Shape module - cone
        var shape = ps.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = config.coneAngle;
        shape.radius = 0.1f;
        shape.radiusThickness = 1f;

        // Size over lifetime
        var sizeOverLifetime = ps.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f,
            GetEaseCurve(config.sizeEase, 1f, config.endSize / config.startSize));

        // Color over lifetime - alpha fade out with easing
        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        var alphaCurve = GetEaseCurve(config.alphaEase, 1f, config.fadeOutAlpha);
        gradient.SetKeys(
            new GradientColorKey[]
            {
                new GradientColorKey(Color.white, 0f),
                new GradientColorKey(Color.white, 1f)
            },
            new GradientAlphaKey[]
            {
                new GradientAlphaKey(alphaCurve.Evaluate(0f), 0f),
                new GradientAlphaKey(alphaCurve.Evaluate(0.25f), 0.25f),
                new GradientAlphaKey(alphaCurve.Evaluate(0.5f), 0.5f),
                new GradientAlphaKey(alphaCurve.Evaluate(0.75f), 0.75f),
                new GradientAlphaKey(alphaCurve.Evaluate(1f), 1f)
            }
        );
        colorOverLifetime.color = gradient;

        // Velocity over lifetime - slow down
        var velocityOverLifetime = ps.velocityOverLifetime;
        velocityOverLifetime.enabled = true;
        velocityOverLifetime.speedModifier = new ParticleSystem.MinMaxCurve(1f,
            GetEaseCurve(config.speedEase, 1f, 0f));

        // Renderer
        var renderer = particleObj.GetComponent<ParticleSystemRenderer>();
        renderer.material = new Material(Shader.Find("Particles/Standard Unlit"));
        renderer.material.color = Color.white;  // Particle color handles tinting

        // Play
        ps.Play();

        var handle = new ParticleHandle(ps);
        activeParticles.Add(handle);

        return handle;
    }

    public void PauseAll()
    {
        foreach (var handle in activeParticles)
            handle.Pause();
    }

    public void ResumeAll()
    {
        foreach (var handle in activeParticles)
            handle.Play();
    }

    public void StopAll()
    {
        foreach (var handle in activeParticles)
            handle.Stop();
    }
}
