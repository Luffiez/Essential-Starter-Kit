using System;
using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] AudioSource bgm;
    [SerializeField] AudioSource sfx;
    [SerializeField] AudioSource bgmFadeOut;
    [SerializeField] float fadeSpeed = 1f;

    private float masterVolume;
    private float bgmVolume;
    private float sfxVolume;

    AudioSettings settings;

    public float MasterVolume => masterVolume;
    public float BgmVolume => bgmVolume;
    public float SfxVolume => sfxVolume;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;

        settings = Game.Settings.GetSetting<AudioSettings>();
        GetAudioLevels();
    }


    private void GetAudioLevels()
    {
        masterVolume = PlayerPrefs.GetFloat(AudioSettings.SETTINGS_KEY_MASTER, settings.MasterVolumeDefault);
        bgmVolume = PlayerPrefs.GetFloat(AudioSettings.SETTINGS_KEY_BGM, settings.BGMVolumeDefault);
        sfxVolume = PlayerPrefs.GetFloat(AudioSettings.SETTINGS_KEY_SFX, settings.SFXVolumeDefault);

        ApplyAudioLevels();
    }

    public void PlaySfx(string clipName, float overrideVolume = -1)
    {
        AudioClipInfo info = settings.GetClip(AudioChannel.SFX, clipName);
        if (info == null)
        {
            Debug.LogWarning($"Sfx Clip '{clipName}' not found in sfx audio library");
            return;
        }
        PlaySfx(info, overrideVolume);
    }

    public void PlaySfx(AudioClipInfo info, float overrideVolume = -1)
    {
        float scale = overrideVolume >= 0f ? overrideVolume : 1;
        sfx.PlayOneShot(info.Clip, scale);
    }

    public void PlayBgm(string clipName, bool fade = true)
    {
        AudioClipInfo info = settings.GetClip(AudioChannel.BGM, clipName);
        if (info == null)
        {
            Debug.LogWarning($"BGM Clip '{clipName}' not found in bgm audio library");
            return;
        }

        PlayBgm(info);
    }

    public void StopBgm()
    {
        if(fadeOutCoroutine != null)
            StopCoroutine(fadeOutCoroutine);
        if(fadeInCoroutine != null)
            StopCoroutine(fadeInCoroutine);
        fadeOutCoroutine = StartCoroutine(BgmFadeOut(bgm, bgmFadeOut));
        bgm.Stop();
    }

    public void ResetAudioLevels()
    {
        SetChannelVolume(AudioChannel.Master, settings.MasterVolumeDefault);
        SetChannelVolume(AudioChannel.BGM, settings.BGMVolumeDefault);
        SetChannelVolume(AudioChannel.SFX, settings.SFXVolumeDefault);
        ApplyAudioLevels();
    }

    public void PlayBgm(AudioClipInfo info, bool fade = true)
    {
        if (bgm.clip == info.Clip && bgm.isPlaying)
        {
            Debug.LogWarning("Already playing this BGM clip: " + info.Clip.name);
            return;
        }

        if (fade)
        {
            // Fade Out Current
            if (fadeOutCoroutine != null)
                StopCoroutine(fadeOutCoroutine);
            if (bgm.clip != null && bgm.isPlaying)
                fadeOutCoroutine = StartCoroutine(BgmFadeOut(bgm, bgmFadeOut));

            // Fade in New
            if (fadeInCoroutine != null)
                StopCoroutine(fadeInCoroutine);
            fadeInCoroutine = StartCoroutine(BgmFadeIn(bgm, info));
        }
        else
        {
            bgm.clip = info.Clip;
            bgm.Play();
        }
    }

    Coroutine fadeInCoroutine;
    Coroutine fadeOutCoroutine;
    IEnumerator BgmFadeIn(AudioSource source, AudioClipInfo info)
    {
        bgm.clip = info.Clip;
        bgm.volume = 0f;
        bgm.Play();

        while (bgmFadeOut.isPlaying)
            yield return null;

        bool fadingOut = true;

        while (bgm.volume < bgmVolume * masterVolume)
        {
            bgm.volume += fadeSpeed * Time.deltaTime;
            yield return null;
        }

        bgm.volume = bgmVolume * masterVolume;
        bgmFadeOut.Stop();
        fadeInCoroutine = null;
    }

    IEnumerator BgmFadeOut(AudioSource source, AudioSource blender)
    {
        // Copy current bgm to blender so it continues playing while fading out
        blender.clip = source.clip;
        blender.volume = source.volume;
        blender.time = source.time;
        blender.Play();

        while (bgmFadeOut.volume > 0f)
        {
            blender.volume -= fadeSpeed * Time.deltaTime;
            yield return null;
        }

        blender.Stop();
        fadeOutCoroutine = null;
    }

    internal void UpdateVolume(AudioChannel channel, float amount)
    {
        SetChannelVolume(channel, amount);
        ApplyAudioLevels();
    }

    private void SetChannelVolume(AudioChannel channel, float amount)
    {
        switch (channel)
        {
            case AudioChannel.Master:
                masterVolume = amount;
                PlayerPrefs.SetFloat(AudioSettings.SETTINGS_KEY_MASTER, masterVolume);
                break;

            case AudioChannel.BGM:
                bgmVolume = amount;
                PlayerPrefs.SetFloat(AudioSettings.SETTINGS_KEY_BGM, bgmVolume);
                break;

            case AudioChannel.SFX:
                sfxVolume = amount;
                PlayerPrefs.SetFloat(AudioSettings.SETTINGS_KEY_SFX, sfxVolume);
                break;

            default: break;
        }
    }

    private void ApplyAudioLevels()
    {
        bgm.volume = masterVolume * bgmVolume;
        sfx.volume = masterVolume * sfxVolume;
    }

    internal void Play(AudioClipInfo clip)
    {
        if (clip.Channel is AudioChannel.BGM)
            PlayBgm(clip);
        else if (clip.Channel is AudioChannel.SFX)
            PlaySfx(clip);
        else
            Debug.LogWarning("Invalid clip channel");
    }
}
