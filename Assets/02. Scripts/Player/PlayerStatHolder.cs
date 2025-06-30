using UnityEngine;
using System.Collections.Generic;
using System;
using Photon.Pun;

public class PlayerStatHolder : MonoBehaviour, IDamageable
{
    public event Action<float, float> PlayerStaminaEvent;

    [Header("Base Stats (for Inspector only)")] // -> 나중에 DB 생기면 그거 기반으로 바꿀거임
    public float BaseMoveSpeed = 5f;
    public float BaseSprintSpeed = 10f;
    public float BaseCoolTime = 1f;
    public float BaseAttackDamage = 10f;
    public float BaseMaxHealth = 100f;
    public float BaseJumpPower = 15f;
    public float BaseMaxStamina = 100f;
    public float BaseStaminaRecoveryRate = 20f;
    public float BaseJumpStaminaCost = 10f;
    public float BaseSprintStaminaCost = 10f;
    public float BaseAttackStaminaCost = 10f;

    private float _currentHealth;
    private float _currentStamina;

    private PlayerStats _runtimeStats;
    public PlayerStats Stats => _runtimeStats;

    [Header("UI")]
    [SerializeField] private GameObject _staminaSlider;

    [Header("# Components")]
    private Dictionary<Type, PlayerAbility> _abilitiesCache = new Dictionary<Type, PlayerAbility>();
    private PhotonView _photonView;

    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();
        _runtimeStats = new PlayerStats();
        _runtimeStats.SetBaseStats(new Dictionary<EStatType, float>
        {
            { EStatType.MoveSpeed, BaseMoveSpeed },
            { EStatType.CoolTime, BaseCoolTime },
            { EStatType.AttackDamage, BaseAttackDamage },
            { EStatType.MaxHealth, BaseMaxHealth },
            { EStatType.JumpPower, BaseJumpPower },
            { EStatType.MaxStamina, BaseMaxStamina },
            { EStatType.SprintSpeed, BaseSprintSpeed },
            { EStatType.StaminaRecoveryRate, BaseStaminaRecoveryRate },
            { EStatType.SprintStaminaCost, BaseSprintStaminaCost },
            { EStatType.JumpStaminaCost, BaseJumpStaminaCost },
            { EStatType.AttackStaminaCost, BaseAttackStaminaCost },
        });
        _currentHealth = GetStat(EStatType.MaxHealth);
        _currentStamina = GetStat(EStatType.MaxStamina);

        if(_photonView.IsMine)
        {
            GameObject staminaSlider = Instantiate(_staminaSlider, GameObject.FindGameObjectWithTag("HUDCanvas").transform);
            staminaSlider.GetComponent<UI_StaminaSlider>().Init(this);
        }
    }

    private void Update()
    {
        if (!_photonView.IsMine)
        {
            return;
        }

        RecoverStamina();
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

    public bool TryUseStamina(float value)
    {
        if(_currentStamina >= value)
        {
            _currentStamina -= value;
            if (_photonView.IsMine)
            {
                PlayerStaminaEvent?.Invoke(_currentStamina, GetStat(EStatType.MaxStamina));
            }
            return true;
        }

        return false;
    }

    public void RecoverStamina()
    {
        PlayerMoveAbility move = GetAbility<PlayerMoveAbility>();
        PlayerAttackAbility attack = GetAbility<PlayerAttackAbility>();
        float maxStamina = GetStat(EStatType.MaxStamina);
        if(!move.IsSprinting && !move.IsJumping && !attack.IsAttacking && !(Mathf.Approximately(_currentStamina, maxStamina) || (Mathf.Approximately(_currentStamina, 0))))
        {
            _currentStamina = Mathf.Clamp(_currentStamina + 20 * Time.deltaTime, 0, GetStat(EStatType.MaxStamina));
            if(_photonView.IsMine)
            {
                PlayerStaminaEvent?.Invoke(_currentStamina, maxStamina);
            }
        }
    }

    public void TakeDamage(Damage damage)
    {
        Debug.Log($"{gameObject.name} 피격 = {damage.Value}");
        _currentHealth -= damage.Value;

        if(_currentHealth <= 0f)
        {
            _currentHealth = 0f;
            Die();
        }
    }

    private void Die()
    {
        PhotonNetwork.Destroy(gameObject);
    }
}