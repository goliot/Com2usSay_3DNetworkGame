using TMPro;
using UnityEngine;

public class UI_RankingSlot : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _rankText;
    [SerializeField] private TextMeshProUGUI _nicknameText;
    [SerializeField] private TextMeshProUGUI _scoreText;

    public void Refresh(int rank, string nickname, int score)
    {
        _rankText.text = rank == -1 ? "랭킹 외" : rank.ToString();
        _nicknameText.text = nickname;
        _scoreText.text = score.ToString();
    }
}
