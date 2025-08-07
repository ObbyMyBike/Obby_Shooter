using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Localization", fileName = "New Localization")]
public class LocalizationData : ScriptableObject, ILocalizationService
{
    public Entry[] Entries;
    
    [Serializable] public struct Entry { public string Key, Value; }

    private Dictionary<string,string> _map;

    private void OnEnable()
    {
        _map = Entries.ToDictionary(entry => entry.Key, entry => entry.Value);
    }

    public string Get(string key) => _map.TryGetValue(key, out String value) ? value : key;
}