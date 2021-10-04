using System.Collections.Generic;
using UnityEngine;

namespace CoreLib.Module
{
    [CreateAssetMenu(fileName = nameof(GlobalID), menuName = Core.k_CoreModuleMenu + nameof(GlobalID))]
    public class GlobalID : Core.Module<GlobalID>
    {
        [SerializeField]
        private ScriptableObjectCollection<CoreLib.GlobalID> m_ID;
        public IReadOnlyDictionary<string, CoreLib.GlobalID> ID => m_ID;

        // =======================================================================
        public override void Init()
        {
        }
    }
}