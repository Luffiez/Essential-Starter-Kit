using UnityEditor;

[CustomEditor(typeof(SceneLoader))]
public class SceneLoaderEditor : Editor
{
    private SettingsSO settingsSO;
    private ScenesSO scenesSO;
    private string[] sceneNames;
    private int selectedIndex;

    private void OnEnable()
    {
        // Load SettingsSO asset via AssetDatabase
        settingsSO = null;
        scenesSO = null;
        sceneNames = new string[] { "None" };
        selectedIndex = 0;

        string[] guids = AssetDatabase.FindAssets("t:SettingsSO");
        if (guids.Length > 0)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            settingsSO = AssetDatabase.LoadAssetAtPath<SettingsSO>(path);
        }
        if (settingsSO == null)
            return;

        // Find ScenesSO from SettingsSO
        foreach (var so in settingsSO.Settings)
        {
            if (so is ScenesSO sso)
            {
                scenesSO = sso;
                break;
            }
        }
        if (scenesSO == null || scenesSO.scenes == null || scenesSO.scenes.Count == 0)
            return;

        sceneNames = new string[scenesSO.scenes.Count + 1];
        sceneNames[0] = "None";
        for (int i = 0; i < scenesSO.scenes.Count; i++)
        {
            string name = scenesSO.scenes[i].displayName;
            sceneNames[i + 1] = name;
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        base.OnInspectorGUI();
        SerializedProperty sceneNameProp = serializedObject.FindProperty("sceneName");

        if (settingsSO == null)
        {
            EditorGUILayout.HelpBox("SettingsSO asset not found in project. Please create one via the Create menu.", MessageType.Error);
            return;
        }
        
        // Find selected index
        selectedIndex = 0;
        for (int i = 1; i < sceneNames.Length; i++)
        {
            if (sceneNameProp.stringValue == sceneNames[i])
            {
                selectedIndex = i;
                break;
            }
        }

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
