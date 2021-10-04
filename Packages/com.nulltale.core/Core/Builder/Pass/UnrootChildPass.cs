using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoreLib.Builder
{
    [DefaultExecutionOrder(Core.k_ManagerDefaultExecutionOrder + 1)]
    public class UnrootChildPass : BuilderPass
    {
        public  bool            m_ActiveOnly = true;
        private List<Transform> m_ToUnroot;

        // =======================================================================
        private void Awake()
        {
            m_ToUnroot = new List<Transform>(transform.childCount);

            foreach (Transform child in transform)
            {
                if (m_ActiveOnly && child.gameObject.activeSelf == false)
                    continue;

                m_ToUnroot.Add(child);
                child.gameObject.SetActive(false);
            }
        }

        public override IEnumerator Build()
        {
            foreach (var child in m_ToUnroot)
            {
                child.transform.SetParent(null);
                child.gameObject.SetActive(true);
            }

            yield break;
        }
    }
}