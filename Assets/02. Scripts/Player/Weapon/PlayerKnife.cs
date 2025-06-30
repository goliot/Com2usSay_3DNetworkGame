using UnityEngine;

public class PlayerKnife : MonoBehaviour
{
    private PlayerAttackAbility _attackAbility;

    private void Start()
    {
        _attackAbility.GetComponentInParent<PlayerAttackAbility>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform == _attackAbility.transform)
        {
            return;
        }

        if(other.TryGetComponent<IDamageable>(out var damageable))
        {
            _attackAbility.HitEnemy(damageable);
        }
    }
}
