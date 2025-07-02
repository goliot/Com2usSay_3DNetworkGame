using Photon.Pun;

public class BearDeadState : IBearState
{
    public void Enter(Bear bear)
    {
        bear.PhotonView.RPC(nameof(bear.SetDieAnim), RpcTarget.All);
    }

    public void Execute(Bear bear)
    {
    }

    public void Exit(Bear bear)
    {
    }
}