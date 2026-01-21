using UnityEngine;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private StartMode startMode;

    [SerializeField, HideInInspector] private string sceneName;
    public string SceneName => sceneName;

    private ScenesSO scenesSO;
    enum StartMode
    {
        LoadOnStart,
        Custom
    }

    private void Awake()
    {
        scenesSO = Settings.Instance.GetSetting<ScenesSO>();
    }

    private void Start()
    {
        if (startMode == StartMode.LoadOnStart && !string.IsNullOrEmpty(sceneName))
            LoadScene();
    }

    public void LoadScene()
    {
        if (!string.IsNullOrEmpty(sceneName))
            Game.SceneManager.LoadScene(scenesSO.GetSceneInfoByName(sceneName));
    }
}
