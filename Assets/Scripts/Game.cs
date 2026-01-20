using UnityEngine;

public class Game : MonoBehaviour
{
    public static Game Instance { get; private set; }

    [SerializeField] SceneSelector defaultScene; 


    private void Awake()
    {
        if (Instance)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        SceneManager.Instance.LoadSceneAsync(defaultScene.SceneName);
    }
}
