using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerStats
{
    private Dictionary<EStatType, float> _stats = new Dictionary<EStatType, float>();

    public GameObject Owner { get; set; }

    public float this[EStatType type]
    {
        get => _stats.ContainsKey(type) ? _stats[type] : 0f;
        set => _stats[type] = value;
    }

    /// <summary>
    /// 서버에서 처음 받아오는 Base Stat
    /// </summary>
    /// <param name="baseStats"></param>
    public void SetBaseStats(Dictionary<EStatType, float> baseStats)
    {
        _stats = new Dictionary<EStatType, float>(baseStats);
    }

    public Dictionary<EStatType, float> CloneStats()
    {
        return new Dictionary<EStatType, float>(_stats);
    }
}
