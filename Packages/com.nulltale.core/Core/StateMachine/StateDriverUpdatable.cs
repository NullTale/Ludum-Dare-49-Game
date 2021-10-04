using System;

namespace CoreLib.StateMachine
{
    public interface IStateUpdatable : IState
    {
        void OnUpdate();
    }

    public class StateDriverUpdatable : StateDriver, IStateUpdatable
    {
        protected Action m_OnUpdate;

        // =======================================================================
        public void OnUpdate()
        {
            m_OnUpdate?.Invoke();
        }

        public override void Setup(object source, Type state)
        {
            base.Setup(source, state);
            m_OnUpdate = _createBindAction(nameof(IStateUpdatable.OnUpdate), source, state);
        }
    }
}