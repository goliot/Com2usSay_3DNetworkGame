using Photon.Pun;

public class StaminaItem : ItemObject
{
    protected override void UseItem(PlayerStatHolder playerStatHolder)
    {
        playerStatHolder.RecoverStamina(20f); // 스태미나를 20만큼 회복
    }
}