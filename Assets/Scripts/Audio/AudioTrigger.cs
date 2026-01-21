using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AudioTrigger : MonoBehaviour
{
    [SerializeField, HideInInspector] private string clipName;
    [SerializeField, HideInInspector] private int clipId;
    [SerializeField, HideInInspector] private AudioChannel channel;
    [SerializeField] TriggerType triggerType;
    
    private Button button;

    public string ClipName => clipName;
    public AudioChannel Channel => channel;

    public enum TriggerType
    {
        None,
        PlayOnStart,
        PlayOnEnable,
        PlayOnDisable,
        OnButtonClick
    }

    private void Start()
    {
        if (triggerType == TriggerType.PlayOnStart)
            PlayClip();
        else if (triggerType == TriggerType.OnButtonClick)
        {
            if (TryGetComponent(out button))
                button.onClick.AddListener(PlayClip);
        }
    }
    private void OnDestroy()
    {
        if (button)
            button.onClick.RemoveAllListeners();
    }

    private void OnEnable()
    {
        if (triggerType == TriggerType.PlayOnEnable)
            PlayClip();
    }
    private void OnDisable()
    {
        if (triggerType == TriggerType.PlayOnDisable)
            PlayClip();
    }


    public void PlayClip()
    {
        if(clipId == -1)
        {
            // Special case: Stop BGM
            AudioManager.Instance.StopBgm();
            return;
        }

        if (string.IsNullOrEmpty(clipName))
        {
            Debug.LogWarning($"AudioTrigger on '{gameObject.name}' has no clip assigned.");
            return;
        }

        AudioSettings audioSettings = Game.Settings.GetSetting<AudioSettings>();
        AudioClipInfo clip = audioSettings.GetClip(Channel, clipName);
        
        if (clip != null)
            AudioManager.Instance.Play(clip);
        else
            Debug.LogWarning($"AudioTrigger on '{gameObject.name}' could not find clip '{clipName}' in AudioLibrary.");
    }
}
