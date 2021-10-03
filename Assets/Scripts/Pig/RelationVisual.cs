using CoreLib;
using UnityEngine;
using UnityEngine.Events;

namespace Game
{
    public class RelationVisual : MonoBehaviour
    {
        public static RelationVisual Instance;

        [SerializeField]
        private Pig m_Target;
        public Pig Target
        {
            get => m_Target;
            set
            {
                if (m_Target == value)
                    return;
                
                if (value?.m_StateMachine.CurrentLabel() == PigState.Drag)
                    value = null;

                m_Target = value;
                if (m_Target != null)
                {
                    OnSelected.Invoke();
                    foreach (var pig in GlobalList<Pig>.Content)
                    {
                        if (m_Target.m_Relationship.TryGetValue(pig, out var relationship) == false)
                            relationship = 0f; 

                        if (pig.IsDead)
                            relationship = 1f;

                        if (pig == m_Target)
                            relationship = -1f;

                        pig.RelationValue = relationship;
                    }
                }
                else
                {
                    OnDiselected.Invoke();
                    foreach (var pig in GlobalList<Pig>.Content)
                        pig.RelationValue = -1f;
                }
            }
        }

        public UnityEvent   OnSelected;
        public UnityEvent   OnDiselected;

        // =======================================================================
        private void Awake()
        {
            Instance = this;
        }
    }
}