using UnityEngine;

[System.Serializable]
public class SceneSelector
{
    [SerializeField] private string sceneName;
    public string SceneName => sceneName;
}
