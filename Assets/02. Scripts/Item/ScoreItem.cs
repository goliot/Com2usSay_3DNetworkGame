public class ScoreItem : ItemObject
{
    public int scoreValue = 10;
    protected override void UseItem(PlayerStatHolder playerStatHolder)
    {
        playerStatHolder.Score += scoreValue; // 스태미나를 20만큼 회복
    }
}