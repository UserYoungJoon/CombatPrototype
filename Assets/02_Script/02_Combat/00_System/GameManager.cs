using UnityEngine;

[DefaultExecutionOrder(-100)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] InputManager inputManager;
    [SerializeField] EffectManager effectManager;
    public InputManager InputManager => inputManager;
    public EffectManager EffectManager => effectManager;

    private void Awake()
    {
        Instance = this;
        inputManager.Init();
        effectManager.Init();
    }
}