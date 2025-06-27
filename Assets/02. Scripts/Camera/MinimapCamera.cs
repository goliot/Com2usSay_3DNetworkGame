using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
    private Transform _target;

    void Update()
    {
        if (_target == null)
        {
            return;
        }

        transform.position = new Vector3(_target.position.x, transform.position.y, _target.position.z);
    }

    public void SetTarget(Transform target)
    {
        _target = target;
    }
}
