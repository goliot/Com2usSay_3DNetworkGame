using UnityEngine;
using System.Collections.Generic;

public class PlayerStatHolder : MonoBehaviour
{
    [Header("Base Stats (for Inspector only)")]
    public float BaseMoveSpeed = 5f;
    public float BaseCoolTime = 1f;
    public float BaseAttackDamage = 10f;
    public float BaseMaxHealth = 100f;
    public float BaseJumpPower = 15f;

    private PlayerStats _runtimeStats;

    public PlayerStats Stats => _runtimeStats;

    private void Awake()
    {
        _runtimeStats = new PlayerStats();

        _runtimeStats.SetBaseStats(new Dictionary<EStatType, float>
        {
            { EStatType.MoveSpeed, BaseMoveSpeed },
            { EStatType.CoolTime, BaseCoolTime },
            { EStatType.AttackDamage, BaseAttackDamage },
            { EStatType.MaxHealth, BaseMaxHealth },
            { EStatType.JumpPower, BaseJumpPower }
        });

        _runtimeStats.Owner = gameObject;
    }

    public void ModifyStat(EStatType stat, float delta)
    {
        _runtimeStats[stat] += delta;
    }

    public float GetStat(EStatType stat)
    {
        return _runtimeStats[stat];
    }
}

// 서버에 모든 무기 스탯 저장해놓고 -> 로그인하면 받아와서 캐싱