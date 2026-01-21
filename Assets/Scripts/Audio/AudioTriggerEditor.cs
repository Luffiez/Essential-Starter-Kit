using UnityEditor;

[CustomEditor(typeof(AudioTrigger))]
public class AudioTriggerEditor : Editor
{
    private AudioSettings audioSettings;
    private AudioLibrary bgmLibrary;
    private AudioLibrary sfxLibrary;
    private string[] bgmClipNames = new string[0];
    private string[] sfxClipNames = new string[0];
    private int selectedClipIndex = 0;
    private int selectedTypeIndex = 0; // 0 = BGM, 1 = SFX
    private const string STOP_BGM = "[STOP BGM]";
    private static readonly string[] typeOptions = { "BGM", "SFX" };

    private void OnEnable()
    {
        // Find AudioSettings asset via AssetDatabase
        string[] guids = AssetDatabase.FindAssets("t:AudioSettings");
        if (guids.Length > 0)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            audioSettings = AssetDatabase.LoadAssetAtPath<AudioSettings>(path);
        }
        bgmLibrary = audioSettings != null ? audioSettings.BgmLibrary : null;
        sfxLibrary = audioSettings != null ? audioSettings.SfxLibrary : null;

        // Build BGM clip names (with [STOP BGM] at the top)
        if (bgmLibrary != null && bgmLibrary.Clips != null)
        {
            bgmClipNames = new string[bgmLibrary.Clips.Length + 1];
            bgmClipNames[0] = STOP_BGM;
            for (int i = 0; i < bgmLibrary.Clips.Length; i++)
                bgmClipNames[i + 1] = bgmLibrary.Clips[i]?.Name ?? $"Clip {i}";
        }
        else
        {
            bgmClipNames = new string[] { STOP_BGM };
        }
        // Build SFX clip names
        if (sfxLibrary != null && sfxLibrary.Clips != null)
        {
            sfxClipNames = new string[sfxLibrary.Clips.Length];
            for (int i = 0; i < sfxLibrary.Clips.Length; i++)
                sfxClipNames[i] = sfxLibrary.Clips[i]?.Name ?? $"Clip {i}";
        }
        else
        {
            sfxClipNames = new string[0];
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        base.OnInspectorGUI();

        SerializedProperty clipNameProp = serializedObject.FindProperty("clipName");
        SerializedProperty clipIdProp = serializedObject.FindProperty("clipId");
        SerializedProperty channelProp = serializedObject.FindProperty("channel");

        if (audioSettings == null)
        {
            EditorGUILayout.HelpBox("AudioSettings asset not found in project. Please create one via the Create menu.", MessageType.Error);
            return;
        }
        if (bgmLibrary == null && sfxLibrary == null)
        {
            EditorGUILayout.HelpBox("No AudioLibrary found in AudioSettings. Please assign at least one in the AudioSettings asset.", MessageType.Error);
            return;
        }
        if (bgmClipNames.Length == 1 && sfxClipNames.Length == 0)
        {
            EditorGUILayout.HelpBox("No AudioClipInfo entries found in either AudioLibrary.", MessageType.Warning);
        }

        // Determine current type and selection
        string currentClipName = clipNameProp.stringValue;
        // Default to BGM
        selectedTypeIndex = 0;
        selectedClipIndex = 0;
        // If currentClipName is in SFX, set type to SFX
        for (int i = 0; i < sfxClipNames.Length; i++)
        {
            if (sfxClipNames[i] == currentClipName)
            {
                selectedTypeIndex = 1;
                selectedClipIndex = i;
                break;
            }
        }
        // If currentClipName is in BGM, set type to BGM
        for (int i = 0; i < bgmClipNames.Length; i++)
        {
            if (bgmClipNames[i] == currentClipName)
            {
                selectedTypeIndex = 0;
                selectedClipIndex = i;
                break;
            }
        }

        // Type dropdown
        int newTypeIndex = EditorGUILayout.Popup("Audio Type", selectedTypeIndex, typeOptions);
        string[] clipOptions = newTypeIndex == 0 ? bgmClipNames : sfxClipNames;
        int newClipIndex = selectedClipIndex;
        bool typeChanged = newTypeIndex != selectedTypeIndex;
        if (typeChanged)
        {
            newClipIndex = 0;
        }
        if (clipOptions.Length > 0)
        {
            newClipIndex = EditorGUILayout.Popup("Audio Clip", newClipIndex, clipOptions);
        }
        else
        {
            EditorGUILayout.HelpBox(newTypeIndex == 0 ? "No BGM clips found." : "No SFX clips found.", MessageType.Warning);
        }

        // If selection changed, update properties
        if (typeChanged || newClipIndex != selectedClipIndex)
        {
            selectedTypeIndex = newTypeIndex;
            selectedClipIndex = newClipIndex;
            if (selectedTypeIndex == 0) // BGM
            {
                clipNameProp.stringValue = bgmClipNames[selectedClipIndex];
                clipIdProp.intValue = selectedClipIndex - 1; // -1 for [STOP BGM]
                if (channelProp != null) channelProp.enumValueIndex = (int)AudioChannel.BGM;
            }
            else // SFX
            {
                clipNameProp.stringValue = sfxClipNames.Length > 0 ? sfxClipNames[selectedClipIndex] : string.Empty;
                clipIdProp.intValue = selectedClipIndex;
                if (channelProp != null) channelProp.enumValueIndex = (int)AudioChannel.SFX;
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
