using UnityEngine;
using System.Collections.Generic;
using System;

public class PlayerStatHolder : MonoBehaviour
{
    [Header("Base Stats (for Inspector only)")] // -> 나중에 DB 생기면 그거 기반으로 바꿀거임
    public float BaseMoveSpeed = 5f;
    public float BaseCoolTime = 1f;
    public float BaseAttackDamage = 10f;
    public float BaseMaxHealth = 100f;
    public float BaseJumpPower = 15f;

    private PlayerStats _runtimeStats;
    public PlayerStats Stats => _runtimeStats;

    [Header("# Components")]
    private Dictionary<Type, PlayerAbility> _abilitiesCache = new Dictionary<Type, PlayerAbility>();

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

    public T GetAbility<T>() where T : PlayerAbility
    {
        var type = typeof(T);

        if (_abilitiesCache.TryGetValue(type, out var ability))
        {
            return ability as T;
        }
        else
        {
            ability = GetComponent<T>();
            if(ability != null)
            {
                _abilitiesCache.Add(type, ability);
                return ability as T;
            }
        }

        throw new Exception($"어빌리티 {type.Name}을 {gameObject.name}에서 찾을 수 없습니다.");
    }
}