using UnityEngine;
using Photon.Pun;

public class PlayerAttackAbility : PlayerAbility
{
    [Header("# Stats")]
    [SerializeField] private float _coolTime;
    [SerializeField] private float _damage;

    [Header("# Components")]
    private Animator _anim;

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
        GameObject bullet = PhotonNetwork.Instantiate("PlayerBullet", _muzzle.position, Quaternion.identity);
        bullet.GetComponent<PlayerBullet>().SetDamage(_player.MakeDamage(), _muzzle.forward);

        _timer = 0f;
    }

    [PunRPC]
    private void PlayAttackAnimation()
    {
        _anim.SetTrigger("DoAttack");
    }
}