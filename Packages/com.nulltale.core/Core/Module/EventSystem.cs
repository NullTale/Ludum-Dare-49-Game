using UnityEngine;
using UnityEventBus;


namespace CoreLib.Module
{
    [CreateAssetMenu(fileName = nameof(EventSystem), menuName = Core.k_CoreModuleMenu + nameof(EventSystem))]
    public class EventSystem : Core.Module
    {
        [SerializeField]
        private bool      m_CollectClasses;
        [SerializeField]
        private bool      m_CollectFunctions;

        // =======================================================================
        public override void Init()
        {
            // instantiate event manager game object
            GlobalBus.Create(m_CollectClasses, m_CollectFunctions);
            GlobalBus.Instance.transform.SetParent(Core.Instance.transform, false);
        }
    }
}