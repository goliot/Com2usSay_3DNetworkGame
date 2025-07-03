using UnityEngine;

public class PlayerKnife : MonoBehaviour
{
    private PlayerAttackAbility _attackAbility;

    private void Start()
    {
        _attackAbility = GetComponentInParent<PlayerAttackAbility>();

        ScoreManager.Instance.OnDataChanged += MakeBigger;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform == _attackAbility.transform)
        {
            return;
        }

        if(other.TryGetComponent<IDamageable>(out var damageable))
        {
            _attackAbility.HitEnemy(other);
        }
    }

    private void MakeBigger()
    {
        int score = ScoreManager.Instance.FinalScore;
        transform.localScale *= 1 + (score / 10000) * 0.1f;
    }
}
