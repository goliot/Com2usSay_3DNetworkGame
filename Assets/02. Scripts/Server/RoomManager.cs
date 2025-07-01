using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using System.Collections.Generic;
using System;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager Instance { get; private set; }

    public event Action OnRoomDataChanged;

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
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        OnRoomDataChanged?.Invoke();
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