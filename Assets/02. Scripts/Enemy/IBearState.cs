public interface IBearState
{
    void Enter(Bear bear);
    void Execute(Bear bear);
    void Exit(Bear bear);
}