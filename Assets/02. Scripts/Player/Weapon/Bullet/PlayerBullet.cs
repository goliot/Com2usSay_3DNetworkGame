using UnityEngine;
using Photon.Pun;

public class PlayerBullet : MonoBehaviour
{
    private Damage _damage;
    private bool _isDamagedSet = false;
    private Vector3 _direction;

    [SerializeField] private float _speed = 20f;
    [SerializeField] private float _lifeTime = 5f;

    private float _timer = 0f;

    private void OnEnable()
    {
        _damage = default;
        _isDamagedSet = false;
        _timer = 0f;
    }

    public void SetDamage(Damage damage, Vector3 direction)
    {
        _damage = damage;
        _isDamagedSet = true;
        _direction = direction.normalized;
    }

    private void Update()
    {
        transform.position += _direction * _speed * Time.deltaTime;

        _timer += Time.deltaTime;
        if (_timer >= _lifeTime)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_isDamagedSet || other.gameObject == _damage.Owner)
        {
            return;
        }

        if (other.TryGetComponent<IDamageable>(out var damageable))
        {
            damageable.TakeDamage(_damage);
        }

        PhotonNetwork.Destroy(gameObject);
    }
}
