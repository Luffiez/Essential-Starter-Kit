using UnityEngine;

[System.Serializable]
public class AudioClipInfo
{
    [SerializeField] private string name;
    [SerializeField] private AudioClip clip;
    [SerializeField, HideInInspector] private AudioChannel channel;

    public string Name => name;
    public AudioClip Clip => clip;
    public AudioChannel Channel => channel;

#if UNITY_EDITOR
    internal void OverrideChannel(AudioChannel channel)
    {
        this.channel = channel;
    }
#endif
}
