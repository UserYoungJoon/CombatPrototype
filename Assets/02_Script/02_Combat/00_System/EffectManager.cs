using UnityEngine;

public class EffectManager : ManagerBase
{
    [Header("Movement Effects")]
    [SerializeField] private GameObject breakEffectPrefab;
    [SerializeField] private GameObject jumpEffectPrefab;
    [SerializeField] private GameObject landingEffectPrefab;
    [SerializeField] private Transform effectTransform;


    [Header("Settings")]
    [SerializeField] private float footOffset = 0.3f;  // 발 뒤쪽 오프셋

    public void PlayBreakEffect(Vector3 position, int lookDir)
    {
        if (breakEffectPrefab == null) return;

        // 발 뒤쪽 (이동 반대 방향)
        Vector3 spawnPos = position + new Vector3(-lookDir * footOffset, 0f, 0f);

        var effect = Instantiate(breakEffectPrefab, spawnPos, Quaternion.identity, effectTransform);

        var ps = effect.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            // Stop Action을 Destroy로 설정
            var main = ps.main;
            main.stopAction = ParticleSystemStopAction.Destroy;

            // FlipX로 방향 전환
            var renderer = effect.GetComponent<ParticleSystemRenderer>();
            if (renderer != null)
            {
                renderer.flip = new Vector3(lookDir < 0 ? 0f : 1f, 0f, 0f);
            }
        }
    }

    public void PlayJumpEffect(Vector3 position)
    {
        if (jumpEffectPrefab == null) return;

        var effect = Instantiate(jumpEffectPrefab, position, Quaternion.identity);
        SetStopActionDestroy(effect);
    }

    public void PlayLandingEffect(Vector3 position)
    {
        if (landingEffectPrefab == null) return;

        var effect = Instantiate(landingEffectPrefab, position, Quaternion.identity);
        SetStopActionDestroy(effect);
    }

    private void SetStopActionDestroy(GameObject effect)
    {
        var ps = effect.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            var main = ps.main;
            main.stopAction = ParticleSystemStopAction.Destroy;
        }
    }
}
