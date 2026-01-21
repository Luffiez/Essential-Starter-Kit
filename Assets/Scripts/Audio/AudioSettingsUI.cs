using UnityEngine;
using UnityEngine.UI;

public class AudioSettingsUI : MonoBehaviour
{
    [SerializeField] private Slider sliderMaster;
    [SerializeField] private Slider sliderMusic;
    [SerializeField] private Slider sliderSound;

    private void OnEnable()
    {
        sliderMaster.minValue = 0;
        sliderMaster.maxValue = 1;
        sliderMaster.value = Game.Audio.MasterVolume;
        sliderMaster.onValueChanged.AddListener(OnMasterChanged);

        sliderMusic.minValue = 0;
        sliderMusic.maxValue = 1;
        sliderMusic.value = Game.Audio.BgmVolume;
        sliderMusic.onValueChanged.AddListener(OnMusicChanged);

        sliderSound.minValue = 0;
        sliderSound.maxValue = 1;
        sliderSound.value = Game.Audio.SfxVolume;
        sliderSound.onValueChanged.AddListener(OnSoundChanged);
    }

    private void OnDisable()
    {
        sliderMaster.onValueChanged.RemoveAllListeners();
        sliderMusic.onValueChanged.RemoveAllListeners();
        sliderSound.onValueChanged.RemoveAllListeners();
    }

    private void OnMasterChanged(float value) => 
        Game.Audio.UpdateVolume(AudioChannel.Master, value);

    private void OnMusicChanged(float value) => 
        Game.Audio.UpdateVolume(AudioChannel.BGM, value);

    private void OnSoundChanged(float value) => 
        Game.Audio.UpdateVolume(AudioChannel.SFX, value);
}
