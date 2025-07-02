using Photon.Pun;
using UnityEngine;

// 아이템 공장 : 아이템 생성 담당
[RequireComponent(typeof(PhotonView))]
public class ObjectFactory : MonoBehaviourPunCallbacks
{
    private static ObjectFactory _instance;
    public static ObjectFactory Instance => _instance;

    private PhotonView _photonView;

    private void Awake()
    {
        _instance = this;
        _photonView = GetComponent<PhotonView>();
    }

    public void RequestCreate(string name, Vector3 position)
    {
        if(PhotonNetwork.IsMasterClient)
        {
            Create(name, position);
        }
        else
        {
            _photonView.RPC(nameof(Create), RpcTarget.MasterClient, name, position);
        }
    }

    public void RequestDelete(int viewId)
    {
        if(PhotonNetwork.IsMasterClient)
        {
            Delete(viewId);
        }
        else
        {
            _photonView.RPC(nameof(Delete), RpcTarget.MasterClient, viewId);
        }
    }

    public void RequestDelete(GameObject toDelete)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Delete(toDelete);
        }
        else
        {
            _photonView.RPC(nameof(Delete), RpcTarget.MasterClient, toDelete);
        }
    }

    [PunRPC]
    private void Create(string name, Vector3 position)
    {
        PhotonNetwork.InstantiateRoomObject(name, position, Quaternion.identity);
    }

    [PunRPC]
    private void Delete(int viewId)
    {
        GameObject objectToDelete = PhotonView.Find(viewId)?.gameObject;
        if (objectToDelete == null)
        {
            return;
        }
        else
        {
            PhotonNetwork.Destroy(objectToDelete);
        }
    }

    [PunRPC]
    private void Delete(GameObject toDelete)
    {
        if(toDelete != null)
        {
            PhotonNetwork.Destroy(toDelete);
        }
    }
}