using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace CoreLib.SpriteAnimation
{
    public class PlaySpriteAnimation : SpriteAnimationPlayer
    {
        private         IAdapter  m_Adapter;
        private         Coroutine m_Update;

        public override float Time
        {
            get => m_Time;
            set
            {
                m_Time = value;
                if (isActiveAndEnabled)
                    m_Adapter.Set(Animation.SpriteAt(m_Time, m_Loop));
            }
        }

        // =======================================================================
        private void OnValidate()
        {
            if (isActiveAndEnabled)
                m_Adapter?.Set(Animation?.SpriteAt(m_Time, m_Loop));
        }

        private void OnDisable()
        {
            if (m_Update != null)
            {
                StopCoroutine(m_Update);
                m_Update = null;
            }
        }

        private void OnEnable()
        {
            var target = m_Target.GetValue(gameObject);

            // require adapter
            if (m_Adapter == null)
                if (target.TryGetComponent<SpriteRenderer>(out var sr))
                    m_Adapter = new SpriteAdapter(sr);
                else
                if (target.TryGetComponent<Image>(out var im))
                    m_Adapter = new ImageAdapter(im);

            if (m_Adapter == null)
            {
                Debug.LogWarning("SpriteRenderer or image component required to play sprite animation");
                return;
            }

            switch (m_UpdateMethod)
            {
                case UpdateMethod.None:
                    if (Animation.IsNull() == false)
                        m_Adapter.Set(Animation.SpriteAt(m_Time, m_Loop));
                    break;
                case UpdateMethod.Time:
                    m_Update = StartCoroutine(_time());
                    break;
                case UpdateMethod.Unscaled:
                    m_Update = StartCoroutine(_unscaled());
                    break;
                case UpdateMethod.Physics:
                    m_Update = StartCoroutine(_physics());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            // ===================================
            IEnumerator _time()
            {
                yield return Core.k_WaitForEndOfFrame;
                while (true)
                {
                    m_Adapter.Set(Animation.SpriteAt(m_Time, m_Loop));
                    m_Time += UnityEngine.Time.deltaTime * Speed;
                    yield return null;
                }

            }
            IEnumerator _unscaled()
            {
                yield return Core.k_WaitForEndOfFrame;
                while (true)
                {
                    m_Adapter.Set(Animation.SpriteAt(m_Time, m_Loop));
                    m_Time += UnityEngine.Time.unscaledDeltaTime * Speed;
                    yield return null;
                }
            }
            
            IEnumerator _physics()
            {
                yield return Core.k_WaitForEndOfFrame;
                while (true)
                {
                    m_Adapter.Set(Animation.SpriteAt(m_Time, m_Loop));
                    m_Time += UnityEngine.Time.fixedDeltaTime * Speed;
                    yield return Core.k_WaitForFixedUpdate;
                }
            }
        }
    }
}