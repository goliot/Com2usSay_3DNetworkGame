public class ScoreItem : ItemObject
{
    public int scoreValue = 10;
    protected override void UseItem(PlayerStatHolder playerStatHolder)
    {
        if (playerStatHolder.PhotonView.IsMine)
        {
            ScoreManager.Instance.AddScore(100);
        }
    }
}