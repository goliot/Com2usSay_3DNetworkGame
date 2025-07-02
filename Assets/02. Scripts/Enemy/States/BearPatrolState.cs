using UnityEngine;

public class BearPatrolState : IBearState
{
    private Transform _nextPatrolPoint;
    private const float ARRIVAL_THRESHOLD = 0.5f; // 목적지 도달 판정 거리

    public void Enter(Bear bear)
    {
        SelectNextPoint(bear);
        bear.NavAgent.speed = bear.MoveSpeed;
        bear.NavAgent.isStopped = false;
        bear.Anim.SetBool("IsWalking", true); // 애니메이션 파라미터에 따라 수정
    }

    public void Execute(Bear bear)
    {
        if (bear.ClosestEnemy != null)
        {
            bear.ChangeState(EBearState.Chase);
            return;
        }

        if (_nextPatrolPoint == null) return;

        float dist = Vector3.Distance(bear.transform.position, _nextPatrolPoint.position);
        if (dist <= ARRIVAL_THRESHOLD)
        {
            SelectNextPoint(bear); // 새 목적지 설정
        }
    }

    public void Exit(Bear bear)
    {
        bear.NavAgent.isStopped = true;
    }

    private void SelectNextPoint(Bear bear)
    {
        int idx = Random.Range(0, bear.PatrolPoints.Length);
        _nextPatrolPoint = bear.PatrolPoints[idx];
        bear.NavAgent.SetDestination(_nextPatrolPoint.position);
    }
}
