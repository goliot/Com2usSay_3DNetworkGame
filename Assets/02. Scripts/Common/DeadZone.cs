using Photon.Pun;
using UnityEngine;

public class DeadZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<IDamageable>(out var damageable))
        {
            damageable.TakeFallDeath(); // fall death 처리용 함수
        }
    }
}
