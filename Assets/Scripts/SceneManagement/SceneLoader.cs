using UnityEngine;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private ScenesSO scenesSO;
    [SerializeField] private StartMode startMode;

    [SerializeField, HideInInspector] private string sceneName;
    public string SceneName => sceneName;

    enum StartMode
    {
        LoadOnStart,
        Custom
    }

    private void Start()
    {
        if (startMode == StartMode.LoadOnStart && !string.IsNullOrEmpty(sceneName))
            LoadScene();
    }

    public void LoadScene()
    {
        if (!string.IsNullOrEmpty(sceneName))
            SceneManager.Instance.LoadScene(scenesSO.GetSceneInfoByName(sceneName));
    }
}
