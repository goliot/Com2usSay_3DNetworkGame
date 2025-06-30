using UnityEngine;
using Photon.Pun;
using System;

public class PlayerAttackAbility : PlayerAbility
{
    [Header("# Stats")]
    [SerializeField] private float _coolTime;
    public Damage Damage => _player.MakeDamage();

    [Header("# Components")]
    private Animator _anim;
    [SerializeField] private Collider _weaponCollider;

    [Header("# Bullet")]
    [SerializeField] private Transform _muzzle;

    public bool IsAttacking { get; private set; }

    private float _timer = 0f;

    protected override void Awake()
    {
        base.Awake();
        _anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!_photonView.IsMine)
        {
            return;
        }

        _timer += Time.deltaTime;

        GetInput();
    }

    private void GetInput()
    {
        IsAttacking = Input.GetMouseButton(0);
        _anim.SetLayerWeight(1, IsAttacking ? 1 : 0);
        if(IsAttacking)
        {
            Attack();
        }
    }

    private void Attack()
    {
        if(_timer < _player.GetStat(EStatType.CoolTime) || !_player.TryUseStamina(_player.GetStat(EStatType.AttackStaminaCost)))
        {
            return;
        }
        //_anim.SetTrigger("DoAttack");
        _photonView.RPC(nameof(PlayAttackAnimation), RpcTarget.All);

        //GameObject bullet = PhotonNetwork.Instantiate("PlayerBullet", _muzzle.position, Quaternion.identity);
        //bullet.GetComponent<PlayerBullet>().SetDamage(_player.MakeDamage(), _muzzle.forward);

        _timer = 0f;
    }

    [PunRPC]
    private void PlayAttackAnimation()
    {
        _anim.SetTrigger("DoMeleeAttack");
    }

    public void ActiveCollider()
    {
        _weaponCollider.enabled = true;
        Debug.Log("콜라이더 On");
    }

    public void DeactiveCollider()
    {
        _weaponCollider.enabled = false;
        Debug.Log("콜라이더 Off");
    }

    public void HitEnemy(IDamageable damageable)
    {
        damageable.TakeDamage(_player.MakeDamage());
    }
}