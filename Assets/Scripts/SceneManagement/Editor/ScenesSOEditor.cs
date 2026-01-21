using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;

[CustomEditor(typeof(ScenesSO))]
public class ScenesSOEditor : Editor
{
    private bool showScenes = true;
    private const string ScenesRoot = "Assets/Scenes/";
    private HashSet<string> allScenePaths;
    private ScenesSO scenesSO;

    private void OnEnable()
    {
        LoadAllScenePaths();
        scenesSO = target as ScenesSO;
    }

    private void LoadAllScenePaths()
    {
        allScenePaths = Directory.Exists(ScenesRoot)
            ? Directory.GetFiles(ScenesRoot, "*.unity", SearchOption.AllDirectories)
                .Select(p => p.Replace("\\", "/")).ToHashSet()
            : new HashSet<string>();
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        EditorGUILayout.Space();
        showScenes = EditorGUILayout.Foldout(showScenes, "Scenes in Build", true);

        if (showScenes)
        {
            HashSet<string> currentScenes = EditorBuildSettings.scenes.Select(s => s.path).ToHashSet();
            bool needsPopulate = !allScenePaths.SetEquals(currentScenes.Where(p => p.StartsWith(ScenesRoot)));

            if (needsPopulate)
            {
                if (GUILayout.Button("Update Build Scene List"))
                    UpdateBuildSceneList();
            }

            EditorGUILayout.HelpBox("Scenes should be located in 'Assets/Scenes/' to appear here. Sub-folders are OK", MessageType.Info);
            Dictionary<string, List<SceneInfo>> folderScenes = BuildFolderSceneMap();
            EditorGUI.indentLevel++;
            foreach (var kvp in folderScenes)
            {
                // Draw folder as header
                EditorGUILayout.LabelField(kvp.Key, EditorStyles.boldLabel);
                foreach (var sceneInfo in kvp.Value)
                {
                    EditorGUILayout.BeginHorizontal();
                    // Indent based on folder depth
                    //int folderDepth = kvp.Key == "(Root)" ? 0 : kvp.Key.Count(c => c == '/');
                    GUILayout.Space(20/* * folderDepth*/);
                    // Find the matching SceneInfo in ScenesSO
                    var soScene = scenesSO.scenes.FirstOrDefault(s => s.assetPath == sceneInfo.assetPath);
                    if (soScene == null)
                    {
                        soScene = new SceneInfo { displayName = sceneInfo.displayName, assetPath = sceneInfo.assetPath, isSingleLoad = false };
                        scenesSO.scenes.Add(soScene);
                        EditorUtility.SetDirty(scenesSO);
                    }
                    // Draw isSingleLoad checkbox (no label, with tooltip, using GUI.Toggle for tooltip support)
                    var singleLoadContent = new GUIContent("", "Single Load: If checked, this scene will be loaded in single mode.");
                    Rect toggleRect = GUILayoutUtility.GetRect(18, 18, GUILayout.Width(18));
                    soScene.isSingleLoad = GUI.Toggle(toggleRect, soScene.isSingleLoad, singleLoadContent);
                    GUILayout.Space(10);
                    // Draw as blue underlined clickable text (like a link)
                    GUIStyle linkStyle = new GUIStyle(EditorStyles.label)
                    {
                        richText = true,
                        normal = { textColor = new Color(0.2f, 0.4f, 1f) }, // blue
                        hover = { textColor = new Color(0.1f, 0.3f, 0.9f) }
                    };
                    string linkText = $"<color=#3366ff><u>{sceneInfo.displayName}</u></color>";
                    if (GUILayout.Button(linkText, linkStyle))
                    {
                        var asset = AssetDatabase.LoadAssetAtPath<SceneAsset>(sceneInfo.assetPath);
                        if (asset != null)
                            EditorGUIUtility.PingObject(asset);
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.Space(2);
            }
            EditorGUI.indentLevel--;
        }
    }

    private void UpdateBuildSceneList()
    {
        HashSet<string> currentScenes = EditorBuildSettings.scenes.Select(s => s.path).ToHashSet();
        List<EditorBuildSettingsScene> newScenes = new List<EditorBuildSettingsScene>();
        bool changed = false;

        // Add all valid scenes from folder
        foreach (var scenePath in allScenePaths)
        {
            newScenes.Add(new EditorBuildSettingsScene(scenePath, true));
            if (!currentScenes.Contains(scenePath))
            {
                changed = true;
                Debug.Log("Added scene to build settings: " + scenePath);
            }
        }

        // Remove scenes that are no longer valid (not in folder)
        foreach (var scene in EditorBuildSettings.scenes)
        {
            if (!allScenePaths.Contains(scene.path))
            {
                changed = true;
                Debug.Log("Removed invalid scene from build settings: " + scene.path);
            }
        }

        if (changed)
        {
            EditorBuildSettings.scenes = newScenes.ToArray();
        }
        else
        {
            Debug.Log("Build settings already match folder contents. No changes made.");
        }

        // Update ScenesSO scene list
        if (scenesSO != null)
        {
            scenesSO.scenes.Clear();
            foreach (var scenePath in allScenePaths)
            {
                string sceneName = Path.GetFileNameWithoutExtension(scenePath);
                scenesSO.scenes.Add(new SceneInfo { displayName = sceneName, assetPath = scenePath, isSingleLoad = false });
            }
            EditorUtility.SetDirty(scenesSO);
        }
    }

    private Dictionary<string, List<SceneInfo>> BuildFolderSceneMap()
    {
        Dictionary<string, List<SceneInfo>> folderScenes = new Dictionary<string, List<SceneInfo>>();
        EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
        foreach (var scene in scenes)
        {
            string path = scene.path;
            if (!path.StartsWith(ScenesRoot))
                continue;
            string relativePath = path.Substring(ScenesRoot.Length); // Remove "Assets/Scenes/"
            string[] parts = relativePath.Split('/');
            string folder;
            if (parts.Length > 1)
                folder = string.Join("/", parts.Take(parts.Length - 1)); // All folders, joined
            else
                folder = "(Root)";
            string sceneName = Path.GetFileNameWithoutExtension(parts[^1]);
            if (!folderScenes.TryGetValue(folder, out var list))
            {
                list = new List<SceneInfo>();
                folderScenes[folder] = list;
            }
            list.Add(new SceneInfo { displayName = sceneName, assetPath = path });
        }
        // Sort folders and scenes alphabetically
        foreach (string key in folderScenes.Keys.ToList())
            folderScenes[key] = folderScenes[key].OrderBy(n => n.displayName).ToList();
        return folderScenes.OrderBy(kvp => kvp.Key).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }
}
