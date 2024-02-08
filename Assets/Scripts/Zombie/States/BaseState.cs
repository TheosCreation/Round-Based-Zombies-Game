public abstract class BaseState
{
    public ZombieAI zombie;
    public StateMachine stateMachine;
    public abstract void Enter();
    public abstract void Perform();
    public abstract void Exit();
}
