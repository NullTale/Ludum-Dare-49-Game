using System.Collections.Generic;
using UnityEngine;

namespace CoreLib.Module
{
    [CreateAssetMenu(fileName = nameof(GlobalValues), menuName = Core.k_CoreModuleMenu + nameof(GlobalValues))]
    public class GlobalValues : Core.Module<GlobalValues>
    {
        [SerializeField]
        private ScriptableObjectCollection<GlobalValue> m_Values;

        public IReadOnlyDictionary<string, GlobalValue> Value => m_Values;

        // =======================================================================
        public override void Init()
        {
            foreach (var value in m_Values.Values)
                value.Init();
        }
    }
}