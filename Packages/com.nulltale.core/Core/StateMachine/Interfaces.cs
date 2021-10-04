namespace CoreLib.StateMachine
{
    public interface IStateMachine<TState> 
        where TState : IStateNone
    {
        public TState CurrentState { get; set; }
    }

    public interface IStateNone { }

    public interface IStateDefault : IStateNone
    {
        void OnEnter();
        void OnExit();
    }

    public interface IState : IStateDefault { }

}