using Photon.Pun;

public interface IDamageable
{
    public void TakeDamage(float damage, string attackerNickname, int attackerViewID = default);
    public void TakeFallDeath();
}