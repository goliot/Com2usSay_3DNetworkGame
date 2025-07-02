using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

[RequireComponent(typeof(PhotonView))]
public class Bear : MonoBehaviour, IDamageable
{
    private BearStateMachine _stateMachine;

    [Header("Bear Stats")]
    public float MaxHp = 100f;
    public float CurrentHealth { get; private set; }
    public float MoveSpeed = 3f;
    public float Damage = 20f;
    public float DetectRange = 5f;
    public float AttackRange = 1f;
    public float AttackCoolTime = 1f;

    [Header("# Status")]
    public bool IsDead => _stateMachine.CurrentState is BearDeadState;
    public GameObject ClosestEnemy { get; private set; }

    [Header("# Hierarchy")]
    [SerializeField] private Transform[] _patrolPoints;
    public Transform[] PatrolPoints => _patrolPoints;

    [Header("# Components")]
    private PhotonView _photonView;
    public PhotonView PhotonView => _photonView;
    private Animator _anim;
    public Animator Anim => _anim;
    private NavMeshAgent _navAgent;
    public NavMeshAgent NavAgent => _navAgent;

    private void Awake()
    {
        _navAgent = GetComponent<NavMeshAgent>();
        _photonView = GetComponent<PhotonView>();
        _anim = GetComponent<Animator>();
        _patrolPoints = GameObject.FindGameObjectsWithTag("PatrolPoint")
            .Select(go => go.transform)
            .ToArray();
        CurrentHealth = MaxHp;

        _stateMachine = new BearStateMachine(this, new Dictionary<EBearState, IBearState>
            {
                { EBearState.Attack, new BearAttackState() },
                { EBearState.Dead, new BearDeadState() },
                { EBearState.Patrol, new BearPatrolState() },
                { EBearState.Chase, new BearChaseState() },
            }
        );
    }

    private void Start()
    {
        StartCoroutine(CoFindClosestPlayer());
    }

    private void Update()
    {
        _stateMachine.Update();
    }

    private IEnumerator CoFindClosestPlayer()
    {
        while (!IsDead)
        {
            FindClosestPlayer();
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void FindClosestPlayer()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, DetectRange, LayerMask.GetMask("Player"));
        float closestDistanceSqr = Mathf.Infinity;
        GameObject closestTarget = null;

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                float distSqr = (hit.transform.position - transform.position).sqrMagnitude;
                if (distSqr < closestDistanceSqr)
                {
                    closestDistanceSqr = distSqr;
                    closestTarget = hit.gameObject;
                }
            }
        }

        ClosestEnemy = closestTarget;
    }

    public void ChangeState(EBearState state)
    {
        _stateMachine.ChangeState(state);
    }

    [PunRPC]
    public void TakeDamage(float damage, string attackerNickname, int actorNumber = default)
    {
        if (IsDead)
        {
            return;
        }

        CurrentHealth -= damage;
        //PlayerHpEvent?.Invoke(CurrentHealth, GetStat(EStatType.MaxHealth));

        if (CurrentHealth <= 0f)
        {
            ChangeState(EBearState.Dead);
            CurrentHealth = 0f;
            _photonView.RPC(nameof(Die), RpcTarget.All);
        }
    }

    [PunRPC]
    private void Die()
    {
        if (IsDead)
        {
            return;
        }
        _anim.SetTrigger("DoDie");
        StartCoroutine(CoDie());
    }

    private IEnumerator CoDie()
    {
        yield return new WaitForSeconds(5f);

        ObjectFactory.Instance.RequestDelete(gameObject);
    }

    public void TakeFallDeath()
    {
        _photonView.RPC(nameof(TakeDamage), RpcTarget.AllBuffered, float.MaxValue, PhotonNetwork.NickName, _photonView.ViewID);
    }

    [PunRPC]
    public void SetAttackAnim()
    {
        _anim.SetTrigger("DoAttack");
    }

    [PunRPC]
    public void SetDieAnim()
    {
        _anim.SetTrigger("DoDie");
    }

    [PunRPC]
    public void SpawnHitEffect(Vector3 position)
    {
        Vector3 dir = (position - transform.position).normalized;
        Quaternion rotation = Quaternion.LookRotation(dir);

        Instantiate(Resources.Load<GameObject>("HitEffect"), position, rotation);
    }
}
