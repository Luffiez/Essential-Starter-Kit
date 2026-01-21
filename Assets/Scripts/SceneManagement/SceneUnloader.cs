using UnityEngine;

public class SceneUnloader : MonoBehaviour
{
    [SerializeField, HideInInspector] private string sceneName;
    public string SceneName => sceneName;

    private ScenesSO scenesSO;
    private void Start()
    {
        scenesSO = Game.Settings.GetSetting<ScenesSO>();
    }

    public void UnloadScene()
    {
        if (!string.IsNullOrEmpty(sceneName))
            Game.SceneManager.UnloadScene(scenesSO.GetSceneInfoByName(sceneName));
    }
}
