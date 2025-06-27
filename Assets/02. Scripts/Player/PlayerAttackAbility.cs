using UnityEngine;

public class PlayerAttackAbility : PlayerAbility
{
    [Header("# Stats")]
    [SerializeField] private float _coolTime;
    [SerializeField] private float _damage;

    [Header("# Components")]
    private Animator _anim;

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
        bool isAttacking = Input.GetMouseButton(0);
        _anim.SetLayerWeight(1, isAttacking ? 1 : 0);
        _anim.SetBool("IsAttacking", isAttacking);
        if(isAttacking)
        {
            Attack();
        }
    }

    private void Attack()
    {
        if(_timer < _player.GetStat(EStatType.CoolTime))
        {
            return;
        }

        _timer = 0f;
    }
}