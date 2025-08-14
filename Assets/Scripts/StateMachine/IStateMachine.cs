namespace NarrativeGame.StateMachine
{
    public interface IStateMachine
    {
        IState CurrentState { get; }
        void   ChangeState(IState newState);
        void   Update();
    }
}