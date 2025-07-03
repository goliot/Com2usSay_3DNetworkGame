using Photon.Pun;
using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Realtime;

public class ScoreManager : MonoBehaviourPunCallbacks
{
    public static ScoreManager Instance { get; private set; }

    private int _score = 0;
    public int Score => _score;


    private void Awake()
    {
        Instance = this;
    }

    public override void OnJoinedRoom()
    {
        Refresh();
    }

    public void Refresh()
    {
        Hashtable _hashTable = new Hashtable();
        _hashTable.Add("Score", _score);
        PhotonNetwork.LocalPlayer.SetCustomProperties(_hashTable);
        Debug.Log(_score);
    }

    public void AddScore(int value)
    {
        _score += value;
        Refresh();
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (!changedProps.ContainsKey("Score"))
        {
            Debug.LogWarning("변경된 프로퍼티에 Score가 없음");
            return;
        }

        Debug.Log($"Player {targetPlayer.NickName}_{targetPlayer.ActorNumber}의 점수 : {changedProps["Score"]}");
    }

}