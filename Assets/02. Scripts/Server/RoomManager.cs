using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using System.Collections.Generic;
using System;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager Instance { get; private set; }

    public event Action OnRoomDataChanged;
    public event Action<string> OnPlayerEnter;
    public event Action<string> OnPlayerLeft;
    public event Action<string, string> OnPlayerDead;

    [Header("# Spawn")]
    [SerializeField] private Transform[] _spawnPoints;

    private Room _room;
    public Room Room => _room;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public override void OnJoinedRoom()
    {
        SetRoom();
        GeneratePlayer();

        OnRoomDataChanged?.Invoke();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        OnRoomDataChanged?.Invoke();
        OnPlayerEnter?.Invoke($"{newPlayer.NickName}_{newPlayer.ActorNumber}");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        OnRoomDataChanged?.Invoke();
        OnPlayerLeft?.Invoke($"{otherPlayer.NickName}_{otherPlayer.ActorNumber}");
    }

    public void OnPlayerDeath(int deadActorNumber, int attackerActorNumber)
    {
        string deadNickname = $"{_room.Players[deadActorNumber].NickName}_{deadActorNumber}";
        string attackerNickname = $"{_room.Players[attackerActorNumber].NickName}_{attackerActorNumber}";
        OnPlayerDead?.Invoke(deadNickname, attackerNickname);
    }

    private void SetRoom()
    {
        _room = PhotonNetwork.CurrentRoom;
        Debug.Log($"{_room.Name} : {_room.PlayerCount} / {_room.MaxPlayers}");
    }

    private void GeneratePlayer()
    {
        PhotonNetwork.Instantiate("Player/ChemicalMan", _spawnPoints[UnityEngine.Random.Range(0, _spawnPoints.Length)].position, Quaternion.identity);
    }
}