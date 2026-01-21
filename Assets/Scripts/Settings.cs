using UnityEngine;
using System;
using System.Collections.Generic;

public class Settings : MonoBehaviour
{
    [SerializeField] private ScriptableObject[] settings;
    private static Dictionary<Type, ScriptableObject> settingsByType;

    private void Awake()
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

    public static T GetSetting<T>() where T : ScriptableObject
    {
        if (settingsByType != null && settingsByType.TryGetValue(typeof(T), out var setting))
            return setting as T;
        Debug.LogError($"Setting of type {typeof(T)} not found.");
        return null;
    }
}