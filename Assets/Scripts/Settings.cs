using UnityEngine;
using System;
using System.Collections.Generic;

[DefaultExecutionOrder(-100)]
public class Settings : MonoBehaviour
{
    public static Settings Instance { get; private set; }

    [SerializeField] private ScriptableObject[] settings;
    private  Dictionary<Type, ScriptableObject> settingsByType;

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    private void ChacheSettings()
    {
        settingsByType = new Dictionary<Type, ScriptableObject>();
        foreach (ScriptableObject setting in settings)
        {
            if (setting != null)
            {
                Type type = setting.GetType();
                if (!settingsByType.ContainsKey(type))
                    settingsByType[type] = setting;
            }
        }
    }

    public T GetSetting<T>() where T : ScriptableObject
    {
        if(settingsByType == null)
            ChacheSettings();

        if (settingsByType != null && settingsByType.TryGetValue(typeof(T), out var setting))
            return setting as T;
        Debug.LogError($"Setting of type {typeof(T)} not found.");
        return null;
    }
}