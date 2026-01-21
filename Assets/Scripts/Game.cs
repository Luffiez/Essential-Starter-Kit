using UnityEngine;

[DefaultExecutionOrder(-100)]
public class Game : MonoBehaviour
{
    public static Game Instance { get; private set; }
    public static SceneManager SceneManager { get; private set; }
    public static Settings Settings { get; private set; }
    public static AudioManager Audio => AudioManager.Instance;

    [SerializeField] SettingsSO settingsSO;

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
        Settings = new Settings(settingsSO);
        SceneManager = new SceneManager();
        Instance = this;
    }
}
