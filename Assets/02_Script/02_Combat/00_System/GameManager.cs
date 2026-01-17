using UnityEngine;

[DefaultExecutionOrder(-100)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] InputManager inputManager;
    [SerializeField] EffectManager effectManager;
    [SerializeField] ParticleManager particleManager;
    public InputManager InputManager => inputManager;
    public EffectManager EffectManager => effectManager;
    public ParticleManager ParticleManager => particleManager;

    public Unit enemy;

    private void Awake()
    {
        Instance = this;
        inputManager.Init();
        effectManager.Init();
        particleManager.Init();
    }
}