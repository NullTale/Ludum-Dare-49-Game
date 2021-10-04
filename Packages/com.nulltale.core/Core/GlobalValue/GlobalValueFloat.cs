using NaughtyAttributes;
using UnityEngine;

namespace CoreLib
{
    public class GlobalValueFloat : GlobalValuePlayable<float>, IPlayableValue
    {
        protected override float _set(float initialValue, float curveValue, float weight)
        {
            return Mathf.Lerp(initialValue, curveValue, weight);
        }

        [Button]
        public void test()
        {
            Value = Random.Range(0, 100);
        }
    }
}