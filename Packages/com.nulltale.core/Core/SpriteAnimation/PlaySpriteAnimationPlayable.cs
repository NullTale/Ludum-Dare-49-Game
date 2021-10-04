using System;
using System.Collections;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using UnityEngine.UI;

namespace CoreLib.SpriteAnimation
{
    public sealed class PlaySpriteAnimationPlayable : SpriteAnimationPlayer
    {
        private PlayableGraph   m_Playable;
        private Animator        m_Animator;

        private bool      m_AnimatorOwner;
        private Coroutine m_Update;

        public override SpriteAnimationAsset Animation
        {
            get => base.Animation;
            set
            {
                if (m_Playable.IsValid() && m_Playable.IsPlaying())
                {
                    _releaseGraph();
                    base.Animation = value;
                    _run();
                }
                else
                    base.Animation = value;
            }
        }

        // =======================================================================
        private void OnEnable()
        {
            _run();
                                                 
            // play
            switch (m_UpdateMethod)
            {
                case UpdateMethod.None:
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
                    var delta = UnityEngine.Time.deltaTime * Speed;
                    m_Time += delta;
                    m_Playable.Evaluate(delta);
                    yield return null;
                }

            }
            IEnumerator _unscaled()
            {
                yield return Core.k_WaitForEndOfFrame;
                while (true)
                {
                    var delta = UnityEngine.Time.unscaledDeltaTime * Speed;
                    m_Time += delta;
                    m_Playable.Evaluate(delta);
                    yield return null;
                }
            }
            
            IEnumerator _physics()
            {
                yield return Core.k_WaitForEndOfFrame;
                while (true)
                {
                    var delta = UnityEngine.Time.fixedDeltaTime * Speed;
                    m_Time += delta;
                    m_Playable.Evaluate(delta);
                    yield return Core.k_WaitForFixedUpdate;
                }
            }
        }

        private void OnDisable()
        {
            StopCoroutine(m_Update);
            m_Update = null;
        }

        private void OnDestroy()
        {
            if (m_AnimatorOwner)
                Destroy(m_Animator);

            _releaseGraph();
        }

        // =======================================================================
        private void _run()
        {
            // require playable
            if (m_Playable.IsValid() == false)
            {
                m_Playable = PlayableGraph.Create();

                if (m_Target.Enabled == false)
                    m_Target.Value = gameObject;

                if (m_Target.Value.TryGetComponent(out m_Animator) == false)
                {
                    m_AnimatorOwner = true;
                    m_Animator      = m_Target.Value.gameObject.AddComponent<Animator>();
                }

                if (m_Target.Value.GetComponent<SpriteRenderer>() != null && base.Animation.m_CreateSpriteAnimation)
                    _setupPlayable(base.Animation.m_SpriteAnimationClip);
                else if (m_Target.Value.GetComponent<Image>() != null && base.Animation.m_CreateImageAnimation)
                    _setupPlayable(base.Animation.m_ImageAnimationClip);

                m_Playable.SetTimeUpdateMode(DirectorUpdateMode.Manual);

                // ===================================
                void _setupPlayable(AnimationClip clip)
                {
                    var clipPlayable   = AnimationClipPlayable.Create(m_Playable, clip);
                    var playableOutput = AnimationPlayableOutput.Create(m_Playable, "Out", m_Animator);

                    // Connect the Playable to an output
                    playableOutput.SetSourcePlayable(clipPlayable);
                }
            }
        }

        private void _releaseGraph()
        {
            if (m_Playable.IsValid())
                m_Playable.Destroy();
        }

        [Button]
        public void test()
        {
            m_Animation.Play(gameObject);
        }

        [Button]
        public void test2()
        {
            m_Animation.PlayOneShot(gameObject);
        }
        [Button]
        public void Play()
        {
            Stop();

            // require playable
            m_Playable = PlayableGraph.Create();

            GameObject target = null;
            if (m_Target.Enabled == false)
                target = gameObject;

            if (target.TryGetComponent(out m_Animator) == false)
            {
                m_AnimatorOwner = true;
                m_Animator = target.gameObject.AddComponent<Animator>();
            }

            if (target.GetComponent<SpriteRenderer>() != null && base.Animation.m_CreateSpriteAnimation)
                _setupPlayable(base.Animation.m_SpriteAnimationClip);
            else
            if (target.GetComponent<Image>() != null && base.Animation.m_CreateImageAnimation)
                _setupPlayable(base.Animation.m_ImageAnimationClip);

            m_Playable.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
            m_Playable.Play();

            // ===================================
            void _setupPlayable(AnimationClip clip)
            {
                var clipPlayable   = AnimationClipPlayable.Create(m_Playable, clip);
                var playableOutput = AnimationPlayableOutput.Create(m_Playable, "Out", m_Animator);

                // Connect the Playable to an output
                playableOutput.SetSourcePlayable(clipPlayable);
            }
        }

        [Button]
        public void Stop()
        {
            if (m_Playable.IsValid() && m_Playable.IsPlaying())
                m_Playable.Destroy();
        }
    }
}