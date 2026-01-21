using UnityEditor;

[CustomEditor(typeof(AudioSettings))]
public class AudioSettingsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        AudioSettings settings = (AudioSettings)target;
        if (settings.BgmLibrary != null && settings.BgmLibrary.Clips != null)
        {
            foreach (var clip in settings.BgmLibrary.Clips)
            {
                if (clip != null && clip.Channel != AudioChannel.BGM)
                   clip.OverrideChannel(AudioChannel.BGM);
            }
        }
        if (settings.SfxLibrary != null && settings.SfxLibrary.Clips != null)
        {
            foreach (var clip in settings.SfxLibrary.Clips)
            {
                if (clip != null && clip.Channel != AudioChannel.SFX)
                    clip.OverrideChannel(AudioChannel.SFX);
            }
        }
    }
}
