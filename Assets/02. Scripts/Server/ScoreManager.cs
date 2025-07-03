using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    public int Score { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void AddScore(int value)
    {
        Score += value;
    }
}