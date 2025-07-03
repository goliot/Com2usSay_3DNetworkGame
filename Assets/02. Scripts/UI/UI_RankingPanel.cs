using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class UI_RankingPanel : MonoBehaviour
{
    [SerializeField] private List<UI_RankingSlot> _slots;
    [SerializeField] private UI_RankingSlot _mySlot;

    private void Start()
    {
        ScoreManager.Instance.OnDataChanged += Refresh;
    }

    private void Refresh()
    {
        // Scores가 이미 정렬된 List<KeyValuePair<string, int>> 라고 가정
        var sortedScores = ScoreManager.Instance.Scores;

        string myKey = $"{PhotonNetwork.LocalPlayer.NickName}_{PhotonNetwork.LocalPlayer.ActorNumber}";
        bool isMyRanked = false;

        for (int i = 0; i < _slots.Count; ++i)
        {
            if (i < sortedScores.Count)
            {
                string nickname = sortedScores[i].Key;
                int score = sortedScores[i].Value;

                _slots[i].Refresh(i + 1, nickname, score);

                if (nickname == myKey)
                {
                    RefreshMyScore(i + 1, nickname, score);
                    isMyRanked = true;
                }
            }
            else
            {
                _slots[i].Refresh(i + 1, "", 0);
            }
        }

        if (!isMyRanked)
        {
            // 내 점수가 순위권에 없을 때 표시
            int myScore = 0;
            // ScoreManager.Instance에 내 점수가 있으면 가져오기 (없으면 0)
            if (ScoreManager.Instance.Scores.Any(x => x.Key == myKey))
            {
                myScore = ScoreManager.Instance.Scores.First(x => x.Key == myKey).Value;
            }

            RefreshMyScore(0, myKey, myScore);
        }
    }

    private void RefreshMyScore(int rank, string nickname, int score)
    {
        if (rank == 0)
            _mySlot.Refresh(-1, nickname, score);  // 랭킹 외 표시용
        else
            _mySlot.Refresh(rank, nickname, score);
    }
}
