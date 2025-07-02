using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(PhotonTransformView))]
[RequireComponent(typeof (Rigidbody))]
[RequireComponent(typeof(Collider))]
public class ItemObject : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerStatHolder playerStatHolder = other.GetComponent<PlayerStatHolder>();
            if (playerStatHolder != null)
            {
                UseItem(playerStatHolder);
                ItemObjectFactory.Instance.RequestDelete(GetComponent<PhotonView>().ViewID);
            }
        }
    }

    protected virtual void UseItem(PlayerStatHolder playerStatHolder) { }
}
