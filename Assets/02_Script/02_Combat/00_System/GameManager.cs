using UnityEngine;

[DefaultExecutionOrder(-100)]
[RequireComponent(typeof(EventBus))]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private InputManager inputManager;
    [SerializeField] private EffectManager effectManager;
    [SerializeField] private ParticleManager particleManager;
    [SerializeField] private EventBus eventBus;
    public InputManager InputManager => inputManager;
    public EffectManager EffectManager => effectManager;
    public ParticleManager ParticleManager => particleManager;
    public EventBus EventBus => eventBus;

    public MonsterUnit monster;

    private void Awake()
    {
        Instance = this;
        inputManager.Init();
        effectManager.Init();
        particleManager.Init();
    }
}