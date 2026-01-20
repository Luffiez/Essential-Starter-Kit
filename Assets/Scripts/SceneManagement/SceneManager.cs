using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManager : MonoBehaviour
{
    public static SceneManager Instance { get; private set; }

    private List<string> loadedScenes = new();

    /// <summary>
    /// We assume a main scene for core game objects, and load other scenes additively.
    /// </summary>
    private LoadSceneMode Mode => LoadSceneMode.Additive;

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
    /// Loads the scene with the given name.
    /// </summary>
    public void LoadSceneSingle(string sceneName)
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
    /// Loads the scene with the given name.
    /// </summary>
    public void LoadScene(string sceneName)
    {
        if(loadedScenes.Contains(sceneName))
        {
            Debug.LogWarning($"Scene '{sceneName}' is already loaded.");
            return;
        }

        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName, Mode);
        loadedScenes.Add(sceneName);
    }

    /// <summary>
    /// Loads the scene with the given name asynchronously.
    /// Allows specifying a callback to be invoked when loading is complete.
    /// </summary>
    public void LoadSceneAsync(string sceneName, Action onLoaded = null)
    {
        if (loadedScenes.Contains(sceneName))
        {
            Debug.LogWarning($"Scene '{sceneName}' is already loaded.");
            onLoaded?.Invoke();
            return;
        }

        AsyncOperation asyncOp = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, Mode);
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
    public void UnloadScene(string sceneName, Action onUnloaded = null)
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
