public class HealthItem : ItemObject
{
    protected override void UseItem(PlayerStatHolder playerStatHolder)
    {
        playerStatHolder.RecoverHealth(20f); // 체력을 20만큼 회복
    }
}