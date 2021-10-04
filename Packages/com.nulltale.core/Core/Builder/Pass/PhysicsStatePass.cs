using System.Collections;
using UnityEngine;

namespace CoreLib.Builder
{
    public class PhysicsStatePass : BuilderPass
    {
        public bool m_On;

        public override IEnumerator Build()
        {
            Physics.autoSimulation = m_On;
            yield break;
        }
    }
}