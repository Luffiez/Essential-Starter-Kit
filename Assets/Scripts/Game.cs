using UnityEngine;

public class Game : MonoBehaviour
{
    public static Game Instance { get; private set; }

    public static SceneManager SceneManager { get; private set; }

    private void Awake()
    {
        if (Instance)
        {
            Destroy(this);
            return;
        }

        Init();
    }

    private void Init()
    {
        Instance = this;
        SceneManager = new SceneManager();
    }
}
