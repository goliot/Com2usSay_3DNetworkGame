using UnityEngine;
using Photon.Pun;

public class BearAttackState : IBearState
{
    private float _timer;

    public void Enter(Bear bear)
    {
        _timer = 0f;
    }

    public void Execute(Bear bear)
    {
        _timer += Time.deltaTime;
        if (_timer >= bear.AttackCoolTime)
        {
            Attack(bear);
            _timer = 0f;
        }
    }

    public void Exit(Bear bear)
    {
    }

    private void Attack(Bear bear)
    {
        bear.PhotonView.RPC(nameof(bear.SetAttackAnim), RpcTarget.All);
        if (bear.ClosestEnemy == null)
        {
            bear.ChangeState(EBearState.Patrol);
            return;
        }
        if(bear.ClosestEnemy.TryGetComponent<IDamageable>(out var damageable))
        {
            damageable.TakeDamage(bear.Damage, "Bear");
        }
    }
}