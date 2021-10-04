using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEventBus;


namespace CoreLib
{
    [Serializable]
    public class StateMachine<TLabel>
    {
        private static State                      s_Default = new State(default, null, null, null);
        private        IState                     m_CurrentState;
        private        Dictionary<TLabel, IState> m_StateDictionary;

        private StateChangeDelagate m_OnStateChange;
        public delegate void StateChangeDelagate(IState current, IState next);

        public ICollection<IState> States => m_StateDictionary.Values;
        public ICollection<TLabel> Labels => m_StateDictionary.Keys;

        public StateChangeDelagate OnStateChange
        {
            get => m_OnStateChange;
            set => m_OnStateChange = value;
        }

        // =======================================================================
        public interface IState
        {
            TLabel Label { get; }

            void Init(StateMachine<TLabel> stateMachine);
            void OnEnter();
            void OnExit();
            void OnReEnter();
        }

        public class State : IState
        {
            public TLabel Label { get; }

            private readonly Action m_OnStart;
            private readonly Action m_OnStop;
            private readonly Action m_OnReEnter;

            // =======================================================================
            public State(TLabel label, Action onStart, Action onStop, Action onReEnter)
            {
                Label = label;
                m_OnStart = onStart;
                m_OnStop = onStop;
                m_OnReEnter = onReEnter ?? _OnReEnterDefault;
            }

            public void Init(StateMachine<TLabel> stateMachine)
            {
            }

            public void OnEnter()
            {
                m_OnStart?.Invoke();
            }

            public void OnExit()
            {
                m_OnStop?.Invoke();
            }

            public void OnReEnter()
            {
                m_OnReEnter?.Invoke();
            }

            private void _OnReEnterDefault()
            {
                OnExit();
                OnEnter();
            }
        }

        public abstract class StateObject : IState
        {
            public abstract TLabel Label { get; }
            public StateMachine<TLabel> StateMachine { get; set; }
            public bool IsActiveState => StateMachine.CurrentState().Equals(this);

            public virtual void Init(StateMachine<TLabel> stateMachine) => StateMachine = stateMachine;
            public virtual void OnEnter() { }
            public virtual void OnExit() { }
            public virtual void OnReEnter()
            {
                OnExit();
                OnEnter();
            }
        }
        
        public abstract class StateObjectMonoBehaviour : MonoBehaviour, IState
        {
            public abstract TLabel Label { get; }
            public StateMachine<TLabel> StateMachine { get; set; }
            public bool IsActiveState => StateMachine.CurrentState().Equals(this);

            public virtual void Init(StateMachine<TLabel> stateMachine) => StateMachine = stateMachine;
            public virtual void OnEnter() { }
            public virtual void OnExit() { }
            public virtual void OnReEnter()
            {
                OnExit();
                OnEnter();
            }
        }

        /// <summary> sets self as main state if enabled </summary>
        public abstract class StateObjectToggledMonoBehaviour : StateObjectMonoBehaviour
        {
            public override void OnEnter() { gameObject.SetActive(true); }

            public override void OnExit() { gameObject.SetActive(false); }

            public override void OnReEnter()
            {
            }

            protected virtual void OnEnable()
            {
                StateMachine.SetState(this);
            }
        }

        public abstract class StateObjectReactive<TEvent> : StateObject, IListener<TEvent>
        {
            public List<Reaction> Reactions
            {
                get => m_Reactions;
                set => m_Reactions = value;
            }

            private List<Reaction> m_Reactions = new List<Reaction>();

            // =======================================================================
            [Serializable]
            public abstract class Reaction : IListener<TEvent>
            {
                public abstract void React(in TEvent e);
            }

            [Serializable]
            public class Transition : Reaction
            {
                public StateObjectReactive<TEvent> Owner { get; private set; }
                public TLabel                      DestinationState;
                public Func<TEvent, bool>  Condition;

                // =======================================================================
                public override void React(in TEvent e)
                {
                    if (Condition(e))
                        Owner.StateMachine.SetState(DestinationState);
                }

                public Transition(TLabel destinationState, Func<TEvent, bool> condition, StateObjectReactive<TEvent> owner)
                {
                    DestinationState = destinationState;
                    Condition        = condition;
                    Owner            = owner;
                }
            }

            // =======================================================================
            public virtual void React(in TEvent e)
            {
                foreach (var reaction in Reactions)
                {
                    reaction.React(e);

                    // stop iteration if transition triggered
                    if (IsActiveState == false)
                        break;
                }
            }

            public StateObjectReactive<TEvent> AddReaction(Reaction reaction)
            {
                Reactions.Add(reaction);
                return this;
            }

            public StateObjectReactive<TEvent> AddTransition(TLabel destinationState, TEvent trigger)
            {
                AddTransition(destinationState, e => Equals(e, trigger));
                return this;
            }

            public StateObjectReactive<TEvent> AddTransition(TLabel destinationState, Func<TEvent, bool> condition)
            {
                Reactions.Add(new Transition(destinationState, condition, this));
                return this;
            }
        }

        // =======================================================================
        public StateMachine()
        {
            // allocate
            m_StateDictionary = new Dictionary<TLabel, IState>();
            m_CurrentState = s_Default;
        }

        public StateMachine(params IState[] states) : this()
        {
            foreach (var state in states)
                AddState(state);
        }

        public StateMachine<TLabel> AddState(TLabel label, Action onStart = null, Action onStop = null, Action onReEnter = null)
        {
            return AddState(new State(label, onStart, onStop, onReEnter));
        }

        public StateMachine<TLabel> AddState<T>(T state) where T : IState
        {
            state.Init(this);
            m_StateDictionary[state.Label] = state;

            return this;
        }

        public StateMachine<TLabel> AddStates<T>(params T[] states) where T : IState
        {
            foreach (var state in states)
                AddState(state);

            return this;
        }

        public TStateType GetStateOfType<TStateType>()
        {
            return (TStateType)m_StateDictionary.Values.FirstOrDefault(n => n is TStateType);
        }

        public IState GetState(TLabel label)
        {
            if (m_StateDictionary.TryGetValue(label, out var state))
                return state;

            return default;
        }

        public bool IsCurrentState<T>() where T : class
        {
            return m_CurrentState.GetType() == typeof(T);
        }

        public TLabel CurrentLabel()
        {
            return m_CurrentState.Label;
        }

        public IState CurrentState()
        {
            return m_CurrentState;
        }

        public T CurrentState<T>()
        {
            if (m_CurrentState is T state)
                return state;

            return default;
        }

        public bool TryGetState<T>(out T state) where T : IState
        {
            if (m_CurrentState is T s)
            {
                state = s;
                return true;
            }

            state = default;
            return false;
        }

        public void SetState<TStateType>() where TStateType : IState
        {
            var state = GetStateOfType<TStateType>();
            if (state != null)
                _setState(state);
        }

        public void SetState(TLabel key)
        {
            if (m_StateDictionary.TryGetValue(key, out var state))
                _setState(state);
        }
        
        public void SetStateEqualTo<T>(T state)
        {
            var setState = m_StateDictionary.Values.FirstOrDefault(n => Equals(n, state));
            if (setState != null)
                _setState(setState);
        }

        public void SetState(IState state)
        {
            _setState(state);
        }
        
        public void SetStateOfType<T>()
        {
            if (GetStateOfType<T>() is IState state)
                _setState(state);
        }

        public void SetStateOfType(Type type)
        {
            var state = m_StateDictionary.Values.FirstOrDefault(type.IsInstanceOfType);
            if (state != null)
                _setState(state);
        }

        /// <summary> Returns the current state name </summary>
        public override string ToString()
        {
            return m_CurrentState?.ToString() ?? string.Empty;
        }

        // =======================================================================
        /// <summary> Changes the state from the existing one to the given </summary>
        private void _setState(IState state)
        {
            if (state.Equals(m_CurrentState))
            {
                // activate ReEnter
                m_CurrentState.OnReEnter();
                return;
            }

            // set new state
            m_CurrentState.OnExit();
            m_OnStateChange?.Invoke(m_CurrentState, state);
            m_CurrentState = state;
            m_CurrentState.OnEnter();
        }
    }
}