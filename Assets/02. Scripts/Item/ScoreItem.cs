public class ScoreItem : ItemObject
{
    public int scoreValue = 10;
    protected override void UseItem(PlayerStatHolder playerStatHolder)
    {
        ScoreManager.Instance.AddScore(100);
    }
}