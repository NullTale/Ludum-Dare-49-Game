using System;
using System.Collections.Generic;
using System.Linq;

namespace CoreLib.StateMachine
{
    public static partial class StateMachineExtentions
    {
        private static Dictionary<Type, Type>   s_DefaultStateCache = new Dictionary<Type, Type>();

        // =======================================================================
        public static void SetState<TState>(this IStateMachine<StateDriver> stateMachine)
        {
            stateMachine.SetState(typeof(TState));
        }
        
        public static void SetState<TState>(this IStateMachine<StateDriverUpdatable> stateMachine)
        {
            stateMachine.SetState(typeof(TState));
        }

        public static void SetState<TDriver>(this IStateMachine<TDriver> stateMachine, Type state)
            where TDriver : StateDriver, new()
        {
            SetState(stateMachine, stateMachine, state, false);
        }

        public static void SetState<TDriver>(this IStateMachine<TDriver> stateMachine, object source, Type state, bool reEnter)
            where TDriver : StateDriver, new()
        {
            var driverType = typeof(TDriver);

            if (state == null)
            {
                // get default state interface from the driver
                if (s_DefaultStateCache.TryGetValue(driverType, out state) == false)
                {
                    var allInterfaces = driverType.GetInterfaces();
                    state = allInterfaces.Except(allInterfaces.SelectMany(t => t.GetInterfaces())).FirstOrDefault(n => n.Implements<IState>());

                    s_DefaultStateCache.Add(driverType, state);
                }
            }

            if (reEnter == false && ReferenceEquals(stateMachine.CurrentState?.Lable, state))
                return;

            // set state driver, if type is null the default state would be set
            StateDriver stateDriverState = new TDriver();
            stateDriverState.Setup(source, state);

            var driver = (TDriver)stateDriverState;

            stateMachine.CurrentState?.OnExit();
            stateMachine.CurrentState = driver;
            stateMachine.CurrentState.OnEnter();
        }
    }
}