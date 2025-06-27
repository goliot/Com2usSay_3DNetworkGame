using UnityEngine;

public abstract class PlayerAbility : MonoBehaviour
{
    protected PlayerStatHolder _player { get; private set; }

    protected virtual void Awake()
    {
        _player = GetComponent<PlayerStatHolder>();
    }
}