using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CoreLib;
using CoreLib.Sound;
using CoreLib.StateMachine;
using Game.PigStates;
using RotaryHeart.Lib.SerializableDictionary;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Playables;
using UnityEventBus;

namespace Game
{
    public interface IStressSource
    {
        internal float GetStress();
    }

    public interface IIncidentReactor : IHandle<IIncidentReactor>
    {
        internal void IncidentDead();
    }

    [DefaultExecutionOrder(-5)]
    public class Pig : EventBus, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler,
                       IIncidentReactor
    {
        public TinyState IsDraggabel;
        public TinyState IsKinematic;
        public bool      IsDead;

        public StressBar m_StressBar;
        public ParticleSystem  m_Dirt;
        
        public AudioPlayer  m_Audio;

        [AudioKey]
        public string   m_DirtAudio;
        [AudioKey]
        public string   m_HitAudio;
        
        [AudioKey]
        public string   m_DragAudio;
        [AudioKey]
        public string   m_ReleaseAudio;
        [AudioKey]
        public string   m_SquealAudio;


        private void OnCollisionEnter(Collision other)
        {
            if (other.impulse.magnitude < 0.3f)
                return;

            m_Audio.Play(m_HitAudio);
        }

        public void Dirt()
        {
            m_Audio.Play(m_DirtAudio);
            m_Dirt.Play(true);
        }

        public void Squeal()
        {
            m_Audio.Play(m_SquealAudio);
        }

        public void Dead()
        {
            Paddock?.SendAction<IIncidentReactor>(reactor => reactor.IncidentDead());
        }

        public float RelationValue
        {
            set
            {
                if (value >= 0)
                {
                    foreach (var mat in View.GetComponentsInChildren<Renderer>().Select(n => n.material))
                    {
                        mat.SetInt(s_NotationEnable, 1);
                        mat.SetFloat(s_NotationWeight, value.Clamp01());
                    }
                }
                else
                {
                    foreach (var mat in View.GetComponentsInChildren<Renderer>().Select(n => n.material))
                        mat.SetInt(s_NotationEnable, 0);
                }
            }
        }

        [NonSerialized]
        public StateMachine<PigState> m_StateMachine;
        public PigState State => m_StateMachine.CurrentLabel();
        public Animator View;

        [Range(0, 1)]
        public float    m_Stress;
        public float    m_StressMaximum;
        public float    m_StressMul;

        [SerializeField]
        private Paddock m_Paddock;
        public Paddock Paddock
        {
            get => m_Paddock;
            set
            {
                if (m_Paddock == value)
                    return;

                m_Paddock?.UnSubscribe(this as IEventBus);
                m_Paddock = value;

                if (m_Paddock != null)
                {
                    m_Paddock?.Subscribe(this as IEventBus);

                    // build relationships
                    foreach (var pig in m_Paddock.m_Pigs.Except(this))
                    {
                        if (m_Relationship.ContainsKey(pig) == false)
                        {
                            m_Relationship.Add(pig, 0f);
                            pig.m_Relationship.Add(this, 0f);
                        }
                    }
                }
            }
        }

        public IEnumerable<Pig>             Neighbors     => m_Paddock == null ? Enumerable.Empty<Pig>() : Paddock.m_Pigs.Except(this);
        public IReadOnlyList<IStressSource> StressSources => m_StressSources;
        public Rigidbody                    RigidBody     { get; set; }
        public int                          Scores = 1;

        public PigGender    m_Gender;
        public PigBreed     m_Breed;

        public  DirectorState                      m_StressView;
        public  PlayableDirector                   m_Explotion;
        private IStressSource[]                    m_StressSources;
        public  SerializableDictionary<Pig, float> m_Relationship;

        public                  float m_DeadStressPreSec;
        public                  float m_RelationshipStressPreSec;
        public                  float m_RelationshipPreSec;
        public                  float m_RelationshipCoolPreSec;
        private static readonly int   s_NotationEnable = Shader.PropertyToID("_NotationEnable");
        private static readonly int   s_NotationWeight = Shader.PropertyToID("_NotationWeight");

        // =======================================================================
        protected override void Awake()
        {
            base.Awake();

            RigidBody = GetComponent<Rigidbody>();

            m_StressSources = GetComponentsInChildren<IStressSource>();
            var states = GetComponentsInChildren<PigStateComponent>(true);
            foreach (var state in states)
                state.Owner = this;
            m_StateMachine = new StateMachine<PigState>();
            m_StateMachine.AddStates(states);

            IsKinematic.OnEnable += () =>
            {
                var rb =GetComponent<Rigidbody>();
                rb.isKinematic = true;
                rb.useGravity = false;
                rb.velocity = Vector3.zero;
            };
            IsKinematic.OnDisable += () =>
            {
                var rb =GetComponent<Rigidbody>();
                rb.isKinematic = false;
                rb.useGravity = true;
                rb.velocity = Vector3.zero;
            };
        }

        public void AddStress(float stress)
        {
            if (GamePreferences.Instance.Pause.IsOpen)
                return;

            if (IsDead)
                return;

            var stressResult = (m_Stress + stress * m_StressMul * GamePreferences.Instance.StressMul).Clamp01();
            if (stressResult == m_Stress)
                return;

            m_Stress       = stressResult;
            m_StressMaximum = m_StressMaximum.Max(m_Stress);

            if (m_Stress >= 1f)
                m_StateMachine.SetState(PigState.Conflict);

            m_StressView.DesiredTime    = m_Stress;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (IsDraggabel)
                m_StateMachine.SetState(PigState.Drag);
        }

        public void OnDrag(PointerEventData eventData)
        {
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            m_StateMachine.CurrentState<Drag>()?.Release();
        }

        public void SetState(PigState state)
        {
            m_StateMachine.SetState(state);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            GlobalList<Pig>.Add(this);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            GlobalList<Pig>.Remove(this);
        }

        private void Update()
        {
            if (GamePreferences.Instance.Pause.IsOpen)
                return;

            if (IsDead)
                return;

            var hasPaddock = Paddock != null;

            // add relationship stress from paddock every frame
            if (hasPaddock)
            {
                foreach (var pig in Paddock.m_Pigs.Except(this))
                {
                    if (pig.IsDead)
                        AddStress(m_DeadStressPreSec * Time.deltaTime);
                    else
                        AddStress(m_RelationshipStressPreSec * m_Relationship[pig] * Time.deltaTime);
                }
            }

            // add stress from traits
            AddStress(StressSources.Select(n => n.GetStress()).Sum() * Time.deltaTime);

            // update relations every frame
            foreach (var pig in m_Relationship.Keys.ToArray())
            {
                if (pig.State == PigState.Socialization)
                {
                    var value = m_Relationship[pig];
                    if (hasPaddock && m_Paddock.m_Pigs.Contains(pig))
                    {
                        m_Relationship[pig] = (value + m_RelationshipPreSec * Time.deltaTime).Clamp01();
                        continue;
                    }

                    m_Relationship[pig] = (value - m_RelationshipCoolPreSec * Time.deltaTime).Clamp01();
                }
            }

        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (IsDead)
                return;

            RelationVisual.Instance.Target = this;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (RelationVisual.Instance.Target != this)
                return;

            RelationVisual.Instance.Target = null;
        }

        // -----------------------------------------------------------------------
        void IIncidentReactor.IncidentDead()
        {
            AddStress(GamePreferences.Instance.DeadStress * GamePreferences.Instance.StressMul);
        }
    }

    [Serializable]
    public enum PigState
    {
        Socialization,
        Conflict,
        Dead,
        Drag,
    }

    [Serializable]
    public enum PigGender
    {
        Male,
        Female
    }

    [Serializable]
    public enum PigBreed
    {
        Common,
        BadBoy,
        Lady,
    }

    public abstract class PigStateComponent : StateMachine<PigState>.StateObjectMonoBehaviour
    {
        public Pig  Owner  { get; set; }

        // =======================================================================
        protected virtual void Awake()
        {
            Owner = GetComponentInParent<Pig>();
        }

        protected virtual void OnEnable()
        {
            Owner.m_StateMachine.SetState(this);
        }

        public override void OnEnter()
        {
            gameObject.SetActive(true);
        }

        public override void OnExit()
        {
            gameObject.SetActive(false);
        }

        public override void OnReEnter() { }
    }

    
    public class PigModule : MonoBehaviour
    {
        [NonSerialized]
        public Pig       Owner;

        protected virtual void Awake()
        {
            Owner = GetComponentInParent<Pig>();
        }
    }
}