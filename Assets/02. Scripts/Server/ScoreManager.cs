using Photon.Pun;
using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Realtime;
using System.Collections.Generic;
using System;
using System.Linq;

public class ScoreManager : MonoBehaviourPunCallbacks
{
    public event Action OnDataChanged;

    public static ScoreManager Instance { get; private set; }

    private int _score = 0;
    public int Score => _score;
    private int _killCount = 0;
    public int KillCount => _killCount;
    public int FinalScore => _killCount * 5000 + _score;

    public Dictionary<string, int> _scores = new Dictionary<string, int>();
    public List<KeyValuePair<string, int>> Scores => _scores.ToList().OrderByDescending(a => a.Value).ToList();

    private void Awake()
    {
        Instance = this;
    }

    public void Start()
    {
        UploadScoreToServer();

        string myKey = $"{PhotonNetwork.LocalPlayer.NickName}_{PhotonNetwork.LocalPlayer.ActorNumber}";
        _scores[myKey] = FinalScore;

        // ✅ 기존 플레이어들의 점수 수동 동기화
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties.TryGetValue("Score", out object scoreObj))
            {
                string key = $"{player.NickName}_{player.ActorNumber}";
                _scores[key] = (int)scoreObj;
            }
        }

        OnDataChanged?.Invoke(); // UI 갱신
    }

    public void UploadScoreToServer()
    {
        Hashtable _hashTable = new Hashtable();
        _hashTable.Add("Score", FinalScore);
        PhotonNetwork.LocalPlayer.SetCustomProperties(_hashTable);
        OnDataChanged?.Invoke();
    }

    public void AddScore(int value)
    {
        _score += value;
        UploadScoreToServer();
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps.ContainsKey("Score"))
        {
            int score = (int)changedProps["Score"];
            string key = $"{targetPlayer.NickName}_{targetPlayer.ActorNumber}";
            _scores[key] = score;

            OnDataChanged?.Invoke(); // UI 갱신
        }
    }

    public void AddKillCount()
    {
        _killCount++;
        UploadScoreToServer();
    }
}