using System.Linq;
using UnityEngine;

namespace CoreLib.States
{
    [CreateAssetMenu(fileName = nameof(StateManager), menuName = Core.k_CoreModuleMenu + nameof(StateManager))]
    public class StateManager : Core.Module<StateManager>
    {
		public ScriptableObjectCollection<GlobalState> m_Groups;
        [SerializeField]
        private State m_DefaultState;

        public State Default => m_DefaultState;

        // =======================================================================
        public override void Init()
        {
            foreach (var taskGroup in m_Groups.Values)
                taskGroup.Init();
        }

        public static WaitStateYieldInstruction WaitTaskGroup(string name, bool waitForClosing)
        {
            return WaitTaskGroup(Get(name), waitForClosing);
        }

        public static WaitStateYieldInstruction WaitTaskGroup(GlobalState state, bool waitForClosing)
        {
            return new WaitStateYieldInstruction(state, waitForClosing);
        }

        public static GlobalState Get(string name)
        {
            return Instance.m_Groups[name];
        }
    }
}