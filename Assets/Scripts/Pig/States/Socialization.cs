using System;
using System.Linq;
using CoreLib;
using UnityEngine;

namespace Game.PigStates
{
    public class Socialization : PigStateComponent
    {
        public override PigState Label => PigState.Socialization;

        public TinyTimer m_SteerTimer;
        public float     m_SteerForce;
        public float     m_NoPaddockStress;
        public float     m_PaddockChill;

        private Vector2 m_SteerPower;
        public float    m_SteerDelay;
        public float    m_MaxSpeed;

        // =======================================================================
        public override void OnEnter()
        {
            base.OnEnter();

            m_SteerTimer.Reset();
            _resetDirection();
        }

        private void _resetDirection()
        {
            m_SteerPower = UnityRandom.Normal2D(m_SteerForce);
        }

        private void FixedUpdate()
        {
            if (Owner.Paddock == null)
                Owner.AddStress(m_NoPaddockStress * Time.fixedDeltaTime);
            else
                Owner.AddStress(-m_PaddockChill * GamePreferences.Instance.ChillMul * Time.fixedDeltaTime);

            if (m_SteerTimer.AddFixedTime())
                _resetDirection();

            if (m_SteerTimer.TimePassed > m_SteerDelay && Owner.RigidBody.velocity.magnitude < m_MaxSpeed)
                Owner.RigidBody.AddForce(m_SteerPower.To3DXZ(), ForceMode.Force);
        }
    }
}