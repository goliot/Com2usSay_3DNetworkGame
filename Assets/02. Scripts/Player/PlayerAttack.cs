using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("# Stats")]
    private PlayerStatHolder _playerStat;
    [SerializeField] private float _coolTime;
    [SerializeField] private float _damage;

    [Header("# Components")]
    private Animator _anim;

    private float _timer = 0f;

    private void Awake()
    {
        _anim = GetComponent<Animator>();
        _playerStat = GetComponent<PlayerStatHolder>();
    }

    private void Update()
    {
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
        if(_timer < _playerStat.GetStat(EStatType.AttackDamage))
        {
            return;
        }

        _timer = 0f;
    }
}