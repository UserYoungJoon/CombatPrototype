using UnityEngine;

public class CinematicManager : ManagerBase
{
    [Header("Stop Motion")]
    [SerializeField] private float stopMotionDuration = 0.1f;

    [Header("Camera Shake")]
    [SerializeField] private float shakeDuration = 0.15f;
    [SerializeField] private float shakeMagnitude = 0.1f;

    [Header("Camera Reference")]
    [SerializeField] private Camera mainCamera;

    private float stopMotionTimer;
    private float shakeTimer;
    private Vector3 originalCameraPos;

    public bool IsStopMotion => stopMotionTimer > 0f;

    protected override void OnInit()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
        originalCameraPos = mainCamera.transform.localPosition;
    }

    private void Update()
    {
        UpdateStopMotion();
        UpdateCameraShake();
    }

    private void UpdateStopMotion()
    {
        if (stopMotionTimer > 0f)
        {
            stopMotionTimer -= Time.unscaledDeltaTime;
        }
    }

    private void UpdateCameraShake()
    {
        if (shakeTimer > 0f)
        {
            shakeTimer -= Time.unscaledDeltaTime;

            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;
            mainCamera.transform.localPosition = originalCameraPos + new Vector3(x, y, 0f);

            if (shakeTimer <= 0f)
            {
                mainCamera.transform.localPosition = originalCameraPos;
            }
        }
    }

    public void PlayHitEffect()
    {
        TriggerStopMotion();
        TriggerCameraShake();
    }

    public void TriggerStopMotion()
    {
        stopMotionTimer = stopMotionDuration;
    }

    public void TriggerStopMotion(float duration)
    {
        stopMotionTimer = duration;
    }

    public void TriggerCameraShake()
    {
        shakeTimer = shakeDuration;
        originalCameraPos = mainCamera.transform.localPosition;
    }

    public void TriggerCameraShake(float duration, float magnitude)
    {
        shakeDuration = duration;
        shakeMagnitude = magnitude;
        TriggerCameraShake();
    }
}
