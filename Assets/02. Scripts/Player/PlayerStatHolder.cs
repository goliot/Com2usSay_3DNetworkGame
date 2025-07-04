using UnityEngine;
using System.Collections.Generic;
using System;
using Photon.Pun;
using System.Collections;
using Unity.Cinemachine;
using DG.Tweening;
using Unity.VisualScripting.Antlr3.Runtime.Tree;

public class PlayerStatHolder : MonoBehaviour, IDamageable
{
    public event Action<float, float> PlayerStaminaEvent;
    public event Action<float, float> PlayerHpEvent;

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
    [SerializeField] private List<EItemType> _dropItems;

    [Header("# Status")]
    public int Score = 0;
    public float CurrentHealth { get; private set; }
    public float CurrentStamina { get; private set; }
    public bool IsDead { get; private set; }

    private PlayerStats _runtimeStats;
    public PlayerStats Stats => _runtimeStats;

    [Header("UI")]
    [SerializeField] private GameObject _staminaSlider;
    [SerializeField] private GameObject _hpSlider;

    [Header("# Components")]
    [SerializeField] private GameObject _root;
    private Dictionary<Type, PlayerAbility> _abilitiesCache = new Dictionary<Type, PlayerAbility>();
    private PhotonView _photonView;
    public PhotonView PhotonView => _photonView;
    private Animator _anim;
    private CinemachineImpulseSource _source;

    private void Awake()
    {
        _source = GetComponent<CinemachineImpulseSource>();
        _anim = GetComponent<Animator>();
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
        CurrentHealth = GetStat(EStatType.MaxHealth);
        CurrentStamina = GetStat(EStatType.MaxStamina);
        IsDead = false;

        if(_photonView.IsMine)
        {
            //GameObject staminaSlider = Instantiate(_staminaSlider, GameObject.FindGameObjectWithTag("HUDCanvas").transform);
            //GameObject hpSlider = Instantiate(_hpSlider, GameObject.FindGameObjectWithTag("HUDCanvas").transform);
            HUDManager.Instance.HpSlider.Init(this);
            HUDManager.Instance.StaminaSlider.Init(this);
            //staminaSlider.GetComponent<UI_StaminaSlider>().Init(this);
            //hpSlider.GetComponent<UI_HpSlider>().Init(this);
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
        if(CurrentStamina >= value)
        {
            CurrentStamina -= value;
            if (_photonView.IsMine)
            {
                PlayerStaminaEvent?.Invoke(CurrentStamina, GetStat(EStatType.MaxStamina));
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
        if(!move.IsSprinting && !move.IsJumping && !attack.IsAttacking && !(Mathf.Approximately(CurrentStamina, maxStamina)))
        {
            CurrentStamina = Mathf.Clamp(CurrentStamina + 20 * Time.deltaTime, 0, GetStat(EStatType.MaxStamina));
            if(_photonView.IsMine)
            {
                PlayerStaminaEvent?.Invoke(CurrentStamina, maxStamina);
            }
        }
    }

    public void RecoverStamina(float value)
    {
        if (_photonView.IsMine)
        {
            _photonView.RPC(nameof(RPC_RecoverStamina), RpcTarget.All, value);
        }
        CurrentStamina = Mathf.Clamp(CurrentStamina + value, 0, GetStat(EStatType.MaxStamina));
        PlayerStaminaEvent?.Invoke(CurrentStamina, GetStat(EStatType.MaxStamina));
    }

    [PunRPC]
    private void RPC_RecoverStamina(float value)
    {
        CurrentStamina = Mathf.Clamp(CurrentStamina + value, 0, GetStat(EStatType.MaxStamina));
        PlayerStaminaEvent?.Invoke(CurrentStamina, GetStat(EStatType.MaxStamina));
    }

    public void RecoverHealth(float value)
    {
        if (_photonView.IsMine)
        {
            _photonView.RPC(nameof(RPC_RecoverHealth), RpcTarget.All, value);
        }
    }

    [PunRPC]
    private void RPC_RecoverHealth(float value)
    {
        CurrentHealth = Mathf.Clamp(CurrentHealth + value, 0, GetStat(EStatType.MaxHealth));
        PlayerHpEvent?.Invoke(CurrentHealth, GetStat(EStatType.MaxHealth));
    }

    [PunRPC]
    public void TakeDamage(float damage, string attackerNickname, int actorNumber = default)
    {
        if(IsDead)
        {
            return;
        }

        //if(actorNumber == default || actorNumber == _photonView.ViewID)
        //{
        //    Debug.Log($"{PhotonNetwork.NickName}이 {nameof(DeadZone)}에 의해 사망했습니다.");
        //}
        //else
        //{
        //    GameObject attacker = PhotonView.Find(actorNumber).gameObject;
        //    Debug.Log($"{attackerNickname} ({attacker?.name}) 에게 {damage} 데미지를 받았습니다.");
        //}

        if (_photonView.IsMine)
        {
            _source.GenerateImpulse();
        }
        
        CurrentHealth -= damage;
        PlayerHpEvent?.Invoke(CurrentHealth, GetStat(EStatType.MaxHealth));
        _root.transform.localScale = Vector3.one;
        _root.transform.DOScale(1.2f * Vector3.one, 0.1f).SetEase(Ease.InOutBounce).OnComplete(() => _root.transform.localScale = Vector3.one);

        if(CurrentHealth <= 0f)
        {
            CurrentHealth = 0f;
            RoomManager.Instance.OnPlayerDeath(_photonView.Owner.ActorNumber, actorNumber);
            if(actorNumber != default && _photonView.IsMine)
            {
                var target = PhotonNetwork.CurrentRoom.GetPlayer(actorNumber);
                _photonView.RPC(nameof(TakeScore), target, ScoreManager.Instance.FinalScore / 2);
                ScoreManager.Instance.AddScore(-(ScoreManager.Instance.Score / 2));
            }
            _photonView.RPC(nameof(Die), RpcTarget.All);
        }
    }

    [PunRPC]
    public void TakeScore(int value)
    {
        ScoreManager.Instance.AddScore(value);
    }

    [PunRPC]
    private void Die()
    {
        if (IsDead)
        {
            return;
        }
        _anim.SetTrigger("DoDie");
        IsDead = true;
        StartCoroutine(CoDie());
    }

    private IEnumerator CoDie()
    {
        yield return new WaitForSeconds(5f);

        if (_photonView.IsMine)
        {
            RoomManager.Instance.Respawn();
            DropItems(UnityEngine.Random.Range(1, 4));
            PhotonNetwork.Destroy(gameObject);
        }
    }

    public void TakeFallDeath()
    {
        _photonView.RPC(nameof(TakeDamage), RpcTarget.AllBuffered, float.MaxValue, PhotonNetwork.NickName, default);
    }

    [PunRPC]
    public void SpawnHitEffect(Vector3 position)
    {
        Vector3 dir = (position - transform.position).normalized;
        Quaternion rotation = Quaternion.LookRotation(dir);

        Instantiate(Resources.Load<GameObject>("HitEffect"), position, rotation);
    }

    private void DropItems(int count)
    {
        for(int i=0; i<count; ++i)
        {
            int idx = UnityEngine.Random.Range(0, _dropItems.Count);
            //PhotonNetwork.InstantiateRoomObject(_dropItems[idx].name, transform.position + new Vector3(0, 2, 0), Quaternion.identity);

            //PhotonNetwork.InstantiateRoomObject("ScoreItem", transform.position + new Vector3(0, 2, 0), Quaternion.identity);
            ObjectFactory.Instance.RequestCreate($"{_dropItems[idx]}Item", transform.position + new Vector3(0, 2, 0));
        }
    }
}