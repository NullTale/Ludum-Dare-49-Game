using System.Collections;
using UnityEngine;

namespace CoreLib.Builder
{
    public abstract class BuilderPass : MonoBehaviour
    {
        [SerializeField]
        private int m_Priority;
        public  int Priority => m_Priority;

        //////////////////////////////////////////////////////////////////////////
        public abstract IEnumerator Build();
    }
}