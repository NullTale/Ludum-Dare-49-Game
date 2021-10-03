using System;
using CoreLib;
using UnityEngine;

namespace Game.PigStates
{
    public class Conflict : PigStateComponent
    {
        public override PigState Label => PigState.Conflict;

        public TinyTimer m_Duration;
        public TinyTimer m_SteerTimer;
        public float     m_SteerForce;

        private Vector2 m_SteerPower;
        public float    m_MaxSpeed;

        // =======================================================================
        public override void OnEnter()
        {
            base.OnEnter();
            Owner.IsDraggabel.Off();
            Owner.m_StressBar.gameObject.SetActive(false);
            m_Duration.Reset();
            m_Duration.AddTime(0.5f.Range());
            _resetDirection();
            Owner.Squeal();
        }

        private void Update()
        {
            if (m_Duration.AddDeltaTime())
                Owner.SetState(PigState.Dead);
        }
        
        private void _resetDirection()
        {
            m_SteerPower = UnityRandom.Normal2D(m_SteerForce);
        }

        private void FixedUpdate()
        {
            if (m_SteerTimer.AddFixedTime())
                _resetDirection();

            if (Owner.RigidBody.velocity.magnitude < m_MaxSpeed)
                Owner.RigidBody.AddForce(m_SteerPower.To3DXZ(), ForceMode.Force);
        }
    }
}