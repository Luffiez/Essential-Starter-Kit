using System;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioSettings", menuName = "Settings/AudioSettings")]
public class AudioSettings : ScriptableObject
{
    [SerializeField, Range(0f,1f)] private float masterVolumeDefault = 1.0f;
    [SerializeField, Range(0f, 1f)] private float bgmVolumeDefault = 1.0f;
    [SerializeField, Range(0f, 1f)] private float sfxVolumeDefault = 1.0f;

    [Space]
    [SerializeField] private AudioLibrary bgmLibrary;
    [SerializeField] private AudioLibrary sfxLibrary;


    public const string SETTINGS_KEY_MASTER = "AudioSettings_Master";
    public const string SETTINGS_KEY_BGM = "AudioSettings_BGM";
    public const string SETTINGS_KEY_SFX = "AudioSettings_SFX";

    public float MasterVolumeDefault => masterVolumeDefault;
    public float BGMVolumeDefault => bgmVolumeDefault;
    public float SFXVolumeDefault => sfxVolumeDefault;

    public AudioClipInfo GetClip(AudioChannel channel, string name)
    {
        if (channel is AudioChannel.BGM)
            return bgmLibrary.GetClipByName(name);
        if (channel is AudioChannel.SFX)
            return sfxLibrary.GetClipByName(name);
        return null;
    }

    public AudioLibrary BgmLibrary => bgmLibrary;
    public AudioLibrary SfxLibrary => sfxLibrary;
}
