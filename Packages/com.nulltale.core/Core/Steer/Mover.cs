using System;
using UnityEngine;

namespace CoreLib.Steer
{
    public abstract class Mover : MonoBehaviour
    {
        protected Vehicle m_Vehicle;
        protected bool UpdateRotation => m_Vehicle.TargetSpeed > m_Vehicle.MinSpeedForTurning && m_Vehicle.Velocity != Vector3.zero;
        
        [SerializeField]
        protected Vector3 m_AxisScale = Vector3.one;

        [Serializable]
        public enum Space
        {
            XYZ,
            XY,
            XZ,
            ZY
        }

        //////////////////////////////////////////////////////////////////////////
        protected virtual void Awake()
        {
            m_Vehicle = GetComponentInParent<Vehicle>();
        }

        protected Vector3 _calculateOffset(float deltaTime)
        {
            return Vector3.Scale(m_Vehicle.CalculateOffset(deltaTime), m_AxisScale);
        }
        protected Vector3 _calculateForward(in Vector3 current, float deltaTime)
        {
            return Vector3.Scale(m_Vehicle.TurnTime > 0 ? Vector3.Slerp(current, m_Vehicle.Orientation, Time.deltaTime / m_Vehicle.TurnTime) : m_Vehicle.Orientation, m_AxisScale);
        }
    }
}