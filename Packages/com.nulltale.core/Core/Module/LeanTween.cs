using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CoreLib.Module
{
    [CreateAssetMenu(fileName = nameof(LeanTween), menuName = Core.k_CoreModuleMenu + nameof(LeanTween))]
    public class LeanTween : Core.Module
    {
        [SerializeField]
        private int m_MaxSimultaneousTweens    = 400;
        [SerializeField]
        private int m_MaxSimultaneousSequences = 400;

        [SerializeField]
        private ScriptableObjectCollection<AnimationCurvePreset> m_Tweens;

        public IReadOnlyDictionary<string, AnimationCurvePreset> Tweens => m_Tweens;

        // =======================================================================
        public override void Init()
        {
            // check or create lean tween on core
            var leanTween = Core.Instance.transform
                                .GetChildren()
                                .Select(n => n.GetComponent<global::LeanTween>())
                                .FirstOrDefault(n => n != null);

            if (leanTween == null)
            {
                var go = new GameObject("LeanTween");
                go.transform.SetParent(Core.Instance.transform);

                go.AddComponent<global::LeanTween>();
            }

            global::LeanTween.init(m_MaxSimultaneousTweens, m_MaxSimultaneousSequences, leanTween?.gameObject);
        }
    }
}