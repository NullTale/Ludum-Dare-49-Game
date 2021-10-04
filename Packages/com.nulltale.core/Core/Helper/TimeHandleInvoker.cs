using System;
using System.Collections;
using System.Linq;
using CoreLib.Module;
using NaughtyAttributes;
using UnityEngine;

namespace CoreLib
{
    [RequireComponent(typeof(TimeHandle))]
    public class TimeHandleInvoker : MonoBehaviour
    {
        public  float                    m_Duration = 1f;
        public  Optional<AnimationCurve> m_TimeScale;
        private TimeHandle               m_TimeHandle;

        // =======================================================================
        private void Awake()
        {
            m_TimeHandle = GetComponent<TimeHandle>();
        }

        private void OnEnable()
        {
            m_TimeHandle.enabled = false;
        }

        public void Invoke()
        {
            StartCoroutine(_waitTimeHandleUnscaled(m_TimeScale, m_Duration, m_TimeHandle));
        }

        public void InvokeTimeEffect()
        {
            Invoke();
        }

        // =======================================================================
        private IEnumerator _waitTimeHandleUnscaled(Optional<AnimationCurve> curve, float duration, TimeHandle handle)
        {
            handle.enabled = true;

            var currentTime = 0f;

            if (curve)
                handle.Scale = curve.Value.Evaluate(0f);

            while (currentTime < duration)
            {
                yield return null;
                currentTime += Time.unscaledDeltaTime;

                if (curve)
                    handle.Scale = curve.Value.Evaluate((currentTime / duration) * curve.Value.Duration());
            }
            
            handle.enabled = false;
        }
    }
}