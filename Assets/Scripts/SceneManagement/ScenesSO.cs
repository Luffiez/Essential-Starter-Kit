using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScenesSO", menuName = "Scene Management/ScenesSO")]
public class ScenesSO : ScriptableObject
{
    [HideInInspector] public List<SceneInfo> scenes = new();

    public SceneInfo GetSceneInfoByName(string sceneName) => 
        scenes.Find(scene => scene.displayName == sceneName);
}
