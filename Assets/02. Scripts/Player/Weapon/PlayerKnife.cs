using Photon.Pun;
using UnityEngine;

public class PlayerKnife : MonoBehaviour
{
    private PlayerAttackAbility _attackAbility;

    private void Start()
    {
        _attackAbility = GetComponentInParent<PlayerAttackAbility>();

        ScoreManager.Instance.OnDataChanged += MakeBigger;
    }

    private void OnDestroy()
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnDataChanged -= MakeBigger;
        }
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
        if (this == null || gameObject == null || !_attackAbility.GetComponent<PhotonView>().IsMine)
            return;

        int score = ScoreManager.Instance.FinalScore;
        transform.localScale *= 1 + (score / 10000f) * 0.1f;
    }

}
