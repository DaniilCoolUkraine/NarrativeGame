namespace NarrativeGame.StateMachine
{
    public interface IState
    {
        void Enter();
        void Exit();
        void Update();
    }
}