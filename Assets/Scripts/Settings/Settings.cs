using UnityEngine;
using System;
using System.Collections.Generic;

public class Settings
{
    private SettingsSO settings;
    private Dictionary<Type, ScriptableObject> settingsByType;

    public Settings(SettingsSO settingsSO)
    {
        settings = settingsSO;
        ChacheSettings();
    }
    private void ChacheSettings()
    {
        settingsByType = new Dictionary<Type, ScriptableObject>();
        foreach (ScriptableObject setting in settings.Settings)
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
        if (settingsByType != null && settingsByType.TryGetValue(typeof(T), out var setting))
            return setting as T;
        Debug.LogError($"Setting of type {typeof(T)} not found.");
        return null;
    }
}