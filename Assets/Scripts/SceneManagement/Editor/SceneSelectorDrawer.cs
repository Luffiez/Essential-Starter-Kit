using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SceneSelector))]
public class SceneSelectorDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Find the sceneName property
        SerializedProperty sceneNameProp = property.FindPropertyRelative("sceneName");

        // Get all scenes in build settings
        var scenes = EditorBuildSettings.scenes;
        string[] sceneNames = new string[scenes.Length];
        int selectedIndex = -1;
        for (int i = 0; i < scenes.Length; i++)
        {
            string path = scenes[i].path;
            string name = System.IO.Path.GetFileNameWithoutExtension(path);
            sceneNames[i] = name;
            if (sceneNameProp.stringValue == name)
                selectedIndex = i;
        }

        // Draw dropdown
        EditorGUI.BeginProperty(position, label, property);
        int newIndex = EditorGUI.Popup(position, label.text, selectedIndex, sceneNames);
        if (newIndex != selectedIndex && newIndex >= 0)
        {
            sceneNameProp.stringValue = sceneNames[newIndex];
        }
        EditorGUI.EndProperty();
    }
}
