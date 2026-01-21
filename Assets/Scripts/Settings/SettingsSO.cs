using UnityEngine;

[CreateAssetMenu(fileName = "SettingsSO", menuName = "Settings/SettingsSO")]
public class SettingsSO : ScriptableObject
{
    [SerializeField] private ScriptableObject[] settings;
    public ScriptableObject[] Settings => settings;
}
