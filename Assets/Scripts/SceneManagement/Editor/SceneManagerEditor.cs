using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;

[CustomEditor(typeof(SceneManager))]
public class SceneManagerEditor : Editor
{
    private bool showScenes = true;
    private const string ScenesRoot = "Assets/Scenes/";
    private HashSet<string> allScenePaths;

    private void OnEnable()
    {
        LoadAllScenePaths();
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
        showScenes = EditorGUILayout.Foldout(showScenes, "Scenes in Build (by Folder)", true);

        if (showScenes)
        {
            var currentScenes = EditorBuildSettings.scenes.Select(s => s.path).ToHashSet();
            bool needsPopulate = !allScenePaths.SetEquals(currentScenes.Where(p => p.StartsWith(ScenesRoot)));

            if (needsPopulate)
            {
                if (GUILayout.Button("Update Build Scene List"))
                    UpdateBuildSceneList();
            }

            EditorGUILayout.HelpBox("Scenes should be located in 'Assets/Scenes/' to appear here. Sub-folders are OK", MessageType.Info);
            var folderScenes = BuildFolderSceneMap();
            EditorGUI.indentLevel++;
            foreach (var kvp in folderScenes)
            {
                // Draw folder as header
                EditorGUILayout.LabelField(kvp.Key, EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                foreach (var sceneInfo in kvp.Value)
                {
                    // Draw as blue underlined clickable text (like a link)
                    var linkStyle = new GUIStyle(EditorStyles.label)
                    {
                        richText = true,
                        normal = { textColor = new Color(0.2f, 0.4f, 1f) }, // blue
                        hover = { textColor = new Color(0.1f, 0.3f, 0.9f) }
                    };
                    string linkText = $"<color=#3366ff><u>{sceneInfo.displayName}</u></color>";
                    Rect labelRect = GUILayoutUtility.GetRect(new GUIContent(sceneInfo.displayName), linkStyle);
                    EditorGUI.LabelField(labelRect, linkText, linkStyle);
                    EditorGUIUtility.AddCursorRect(labelRect, MouseCursor.Link);
                    if (Event.current.type == EventType.MouseDown && labelRect.Contains(Event.current.mousePosition))
                    {
                        var asset = AssetDatabase.LoadAssetAtPath<SceneAsset>(sceneInfo.assetPath);
                        if (asset != null)
                        {
                            EditorGUIUtility.PingObject(asset);
                            Selection.activeObject = asset;
                        }
                        Event.current.Use();
                    }
                }
                EditorGUI.indentLevel--;
                EditorGUILayout.Space(2);
            }
            EditorGUI.indentLevel--;
        }
    }

    private void UpdateBuildSceneList()
    {
        var currentScenes = EditorBuildSettings.scenes.Select(s => s.path).ToHashSet();
        var newScenes = new List<EditorBuildSettingsScene>();
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
    }

    private class SceneInfo
    {
        public string displayName;
        public string assetPath;
    }

    private Dictionary<string, List<SceneInfo>> BuildFolderSceneMap()
    {
        var folderScenes = new Dictionary<string, List<SceneInfo>>();
        var scenes = EditorBuildSettings.scenes;
        foreach (var scene in scenes)
        {
            var path = scene.path;
            if (!path.StartsWith(ScenesRoot))
                continue;
            var relativePath = path.Substring(ScenesRoot.Length); // Remove "Assets/Scenes/"
            var parts = relativePath.Split('/');
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
        foreach (var key in folderScenes.Keys.ToList())
            folderScenes[key] = folderScenes[key].OrderBy(n => n.displayName).ToList();
        return folderScenes.OrderBy(kvp => kvp.Key).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }
}
