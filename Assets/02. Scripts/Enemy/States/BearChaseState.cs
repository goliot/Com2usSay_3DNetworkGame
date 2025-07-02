using UnityEngine;

public class BearChaseState : IBearState
{
    public void Enter(Bear bear)
    {
        bear.NavAgent.speed = bear.MoveSpeed;
        bear.NavAgent.isStopped = false;
        bear.Anim.SetBool("IsRunning", true);
    }

    public void Execute(Bear bear)
    {
        if (bear.ClosestEnemy == null)
        {
            bear.ChangeState(EBearState.Patrol);
            return;
        }

        if (bear.NavAgent == null || bear.ClosestEnemy == null)
            return;

        // 적 위치로 이동
        bear.NavAgent.SetDestination(bear.ClosestEnemy.transform.position);

        // 적과의 거리 계산
        float distanceToEnemy = Vector3.Distance(bear.transform.position, bear.ClosestEnemy.transform.position);

        // 공격 사정거리 이내로 들어오면 공격 상태로 전환
        if (distanceToEnemy <= bear.AttackRange)
        {
            bear.ChangeState(EBearState.Attack);
        }
    }

    public void Exit(Bear bear)
    {
        if (bear.NavAgent != null)
        {
            bear.NavAgent.isStopped = true;
            bear.NavAgent.ResetPath();
        }
        bear.Anim.SetBool("IsRunning", false);
    }
}
