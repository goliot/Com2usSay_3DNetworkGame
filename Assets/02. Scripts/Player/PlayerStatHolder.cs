using UnityEngine;
using System.Collections.Generic;
using System;
using Photon.Pun;

public class PlayerStatHolder : MonoBehaviour, IDamageable
{
    [Header("Base Stats (for Inspector only)")] // -> 나중에 DB 생기면 그거 기반으로 바꿀거임
    public float BaseMoveSpeed = 5f;
    public float BaseCoolTime = 1f;
    public float BaseAttackDamage = 10f;
    public float BaseMaxHealth = 100f;
    public float BaseJumpPower = 15f;
    public float BaseMaxStamina = 100f;

    private float _currentHealth;

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
            { EStatType.JumpPower, BaseJumpPower },
            { EStatType.MaxStamnina, BaseMaxStamina },
        });
        _currentHealth = GetStat(EStatType.MaxHealth);
    }

    public void ModifyStat(EStatType stat, float delta)
    {
        _runtimeStats[stat] += delta;
    }

    public float GetStat(EStatType stat)
    {
        return _runtimeStats[stat];
    }

    public Damage MakeDamage()
    {
        return new Damage
        {
            Owner = gameObject,
            Value = Stats[EStatType.AttackDamage]
        };
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

    public void TakeDamage(Damage damage)
    {
        _currentHealth -= damage.Value;

        if(_currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        PhotonNetwork.Destroy(gameObject);
    }
}