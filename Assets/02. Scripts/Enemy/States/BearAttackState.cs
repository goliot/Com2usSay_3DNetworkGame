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
        if (bear.ClosestEnemy == null)
        {
            bear.ChangeState(EBearState.Patrol);
            return;
        }

        float sqrDist = (bear.ClosestEnemy.transform.position - bear.transform.position).sqrMagnitude;
        float sqrAttackRange = bear.AttackRange * bear.AttackRange;

        // 사거리 안에 있을 때만 공격
        if (sqrDist <= sqrAttackRange)
        {
            bear.PhotonView.RPC(nameof(bear.SetAttackAnim), RpcTarget.All);

            var statHolder = bear.ClosestEnemy.GetComponent<PlayerStatHolder>();
            if (statHolder != null)
            {
                statHolder.PhotonView.RPC("TakeDamage", RpcTarget.AllBuffered, bear.Damage, "Bear", default);
            }
        }
        else
        {
            // 타겟이 너무 멀면 추적 상태로 전환
            bear.ChangeState(EBearState.Chase);
        }
    }
}