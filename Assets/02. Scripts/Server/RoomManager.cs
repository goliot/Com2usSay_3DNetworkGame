using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;

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

    public void Start()
    {
        SetRoom();
        GeneratePlayer();
        OnRoomDataChanged?.Invoke();

        SpawnBears();
        SpawnItems();
    }

    private void SpawnBears()
    {
        if(!PhotonNetwork.IsMasterClient)
        {
            return;   
        }

        foreach (var point in _spawnPoints)
        {
            float value = UnityEngine.Random.value;
            if (value < 0.5f)
            {
                PhotonNetwork.Instantiate("Bear", point.position, Quaternion.identity);
            }
        }
    }

    private void SpawnItems()
    {
        if(!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        StartCoroutine(CoSpawnItems());
    }

    private IEnumerator CoSpawnItems()
    {
        WaitForSeconds wait = new WaitForSeconds(5f);

        while (true)
        {
            yield return wait;

            // 아이템 타입 랜덤 선택
            int random = UnityEngine.Random.Range(0, (int)EItemType.Count);
            EItemType randomItem = (EItemType)random;

            // 스폰 지점 랜덤 선택
            int spawnIndex = UnityEngine.Random.Range(0, _spawnPoints.Length);
            Vector3 spawnPosition = _spawnPoints[spawnIndex].position;

            // 아이템 이름 예: "HpItem", "ScoreItem" 등
            string itemName = $"{randomItem}Item";

            // 아이템 생성 요청
            ObjectFactory.Instance.RequestCreate(itemName, spawnPosition);
        }
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

        string attackerNickname = attackerActorNumber == default ? "시스템" : $"{_room.Players[attackerActorNumber].NickName}_{attackerActorNumber}";

        if(attackerActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            ScoreManager.Instance.AddKillCount();
        }

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

    public void Respawn()
    {
        PhotonNetwork.Instantiate("Player/ChemicalMan", _spawnPoints[UnityEngine.Random.Range(0, _spawnPoints.Length)].position, Quaternion.identity);
    }
}