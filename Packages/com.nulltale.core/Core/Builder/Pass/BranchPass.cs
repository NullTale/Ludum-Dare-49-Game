using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CoreLib.Builder
{
    public class BranchPass : BuilderPass
    {
        public List<BuilderPass>    m_Passes;
        private SortedCollection<BuilderPass> m_PassesQueue = new SortedCollection<BuilderPass>(Comparer<BuilderPass>.Create(((a, b) => a.Priority - b.Priority)));

        //////////////////////////////////////////////////////////////////////////
        public override IEnumerator Build()
        {
            m_PassesQueue.Clear();

            // childs only
            foreach (Transform child in transform)
            {
                foreach (var builderPass in child.GetComponents<BuilderPass>())
                    if (builderPass.gameObject.activeInHierarchy)
                        m_PassesQueue.Add(builderPass);
            }

            m_Passes = m_PassesQueue.ToList();
            foreach (var pass in m_PassesQueue)
                yield return pass.Build();
        }
    }
}