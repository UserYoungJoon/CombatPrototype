using UnityEngine;

[DefaultExecutionOrder(-100)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] InputManager inputManager;
    [SerializeField] EffectManager effectManager;

    private void Awake()
    {
        Instance = this;
        inputManager.Init();
        effectManager.Init();
    }
}