using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManager : MonoBehaviour
{
    public static SceneManager Instance { get; private set; }

    [SerializeField] private ScenesSO scenes;

    private List<string> loadedScenes = new(); 

    public List<string> LoadedScenes => loadedScenes;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// Loads the scene based on the provided SceneInfo.
    /// Single additive loading is determined by the isSingleLoad property.
    /// </summary>
    internal void LoadScene(SceneInfo sceneInfo, Action onLoadedAsync = null)
    {
        if (sceneInfo.isSingleLoad)
            LoadSceneSingle(sceneInfo.displayName);
        else
            LoadSceneAsync(sceneInfo.displayName, onLoadedAsync);
    }

    /// <summary>
    /// Unloads the scene based on the provided SceneInfo.
    /// </summary>
    /// <param name="sceneInfo"></param>
    /// <param name="onUnloadedAsync"></param>
    internal void UnloadScene(SceneInfo sceneInfo, Action onUnloadedAsync = null)
    {
        UnloadScene(sceneInfo.displayName, onUnloadedAsync);
    }

    /// <summary>
    /// Loads the scene with the given name.
    /// </summary>
    private void LoadSceneSingle(string sceneName)
    {
        if (loadedScenes.Contains(sceneName))
        {
            Debug.LogWarning($"Scene '{sceneName}' is already loaded.");
            return;
        }

        loadedScenes.Clear();
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        loadedScenes.Add(sceneName);
    }

    /// <summary>
    /// Loads the scene with the given name asynchronously.
    /// Allows specifying a callback to be invoked when loading is complete.
    /// </summary>
    private void LoadSceneAsync(string sceneName, Action onLoaded = null)
    {
        if (loadedScenes.Contains(sceneName))
        {
            Debug.LogWarning($"Scene '{sceneName}' is already loaded.");
            onLoaded?.Invoke();
            return;
        }

        AsyncOperation asyncOp = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        asyncOp.completed += _ => 
        { 
            onLoaded?.Invoke();
            loadedScenes.Add(sceneName);
        };
    }

    /// <summary>
    /// Unloads the scene with the given name asynchronously.
    /// Allows specifying an optional callback to be invoked when unloading is complete.
    /// </summary>
    internal void UnloadScene(string sceneName, Action onUnloaded = null)
    {
        if (!loadedScenes.Contains(sceneName))
        {
            Debug.LogWarning($"Scene '{sceneName}' is not loaded.");
            onUnloaded?.Invoke();
            return;
        }

        AsyncOperation asyncOp = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(sceneName);
        asyncOp.completed += _ =>
        {
            onUnloaded?.Invoke();
            loadedScenes.Remove(sceneName);
        };
    }
}
