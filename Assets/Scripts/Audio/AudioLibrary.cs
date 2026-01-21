using UnityEngine;

[System.Serializable]
public class AudioLibrary
{
    [SerializeField] private AudioClipInfo[] clips;

    public AudioClipInfo[] Clips => clips;

    public AudioClipInfo GetClipByName(string clipName) =>
        System.Array.Find(clips, clip => clip.Name.ToLowerInvariant() == clipName.ToLowerInvariant());
}
