using UnityEditor;

[CustomEditor(typeof(SceneLoader))]
public class SceneLoaderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        base.OnInspectorGUI();
        // Get properties
        SerializedProperty sceneNameProp = serializedObject.FindProperty("sceneName");


        // Get the ScenesSO asset
        ScenesSO scenesSO = Settings.Instance.GetSetting<ScenesSO>();
        string[] sceneNames;
        int selectedIndex = 0;

        if (scenesSO != null && scenesSO.scenes != null && scenesSO.scenes.Count > 0)
        {
            sceneNames = new string[scenesSO.scenes.Count + 1];
            sceneNames[0] = "None";
            for (int i = 0; i < scenesSO.scenes.Count; i++)
            {
                string name = scenesSO.scenes[i].displayName;
                sceneNames[i + 1] = name;
                if (sceneNameProp.stringValue == name)
                    selectedIndex = i + 1;
            }
        }
        else
        {
            sceneNames = new string[] { "None" };
            selectedIndex = 0;
        }

        // Draw dropdown for sceneName
        int newIndex = EditorGUILayout.Popup("Scene Name", selectedIndex, sceneNames);
        if (newIndex != selectedIndex)
        {
            if (newIndex == 0)
                sceneNameProp.stringValue = string.Empty;
            else
                sceneNameProp.stringValue = sceneNames[newIndex];
        }

        serializedObject.ApplyModifiedProperties();
    }
}
