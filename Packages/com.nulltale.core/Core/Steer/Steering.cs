using System;
using UnityEngine;

namespace CoreLib.Steer
{
    public abstract class Steering : MonoBehaviour
    {
        /// <summary> Last force calculated </summary>
        private Vector3 m_LastForceCalculated = Vector3.zero;

        protected Vehicle m_Vehicle;

        [SerializeField]
        protected float m_Weight = 1;

        /// <summary>
        /// Calculates the force provided by this steering behavior and raises any
        /// arrival/start events.
        /// </summary>
        /// <remarks>
        /// Besides calculating the desired force, it will also notify the vehicle
        /// of when it started/stopped providing force, via the OnArrival and
        /// OnStartMoving events.  If an OnArrival even is raised, the receiving
        /// object can set the ShouldRetryForce property to TRUE to force the vehicle
        /// recalculating the force once.
        /// </remarks>
        public Vector3 Force
        {
            get
            {
                m_LastForceCalculated = _calculateForce();
                if (m_LastForceCalculated != Vector3.zero)
                {
                    if (!ReportedMove)
                    {
                        OnStartMoving?.Invoke(this);
                    }
                    ReportedArrival = false;
                    ReportedMove = true;
                }
                else if (!ReportedArrival)
                {
                    if (OnArrival != null)
                    {
                        OnArrival(this);
                        // It's possible that any of the OnArrival handlers indicated we should recalculate
                        // our forces.
                        if (ShouldRetryForce)
                        {
                            m_LastForceCalculated = _calculateForce();
                            ShouldRetryForce = false;
                        }
                    }
                    if (m_LastForceCalculated == Vector3.zero)
                    {
                        ReportedArrival = true;
                        ReportedMove = false;
                    }
                }
                return m_LastForceCalculated;
            }
        }

        public virtual bool IsPostProcess => false;

        /// <summary> Steering event handler for arrival notification </summary>
        public Action<Steering> OnArrival = delegate { };

        /// <summary> Steering event handler for arrival notification </summary>
        public Action<Steering> OnStartMoving { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this Steering should recalculate 
        /// its force.
        /// </summary>
        /// <value><c>true</c> if recalculate force; otherwise, <c>false</c>.</value>
        /// <remarks>
        /// This property is checked once after the steering behavior has raised an
        /// OnArrival event, and if it is true, the force is then recalculated. This
        /// is useful in cases of vehicles which do not recalculate their forces
        /// even frame, since we may want to provide some continuity of movement in
        /// some cases (for instance, when moving from one waypoint to another) 
        /// instead of having the vehicle stop at a point until the next time that
        /// the Force is explicitly queried.
        /// </remarks>
        public bool ShouldRetryForce { get; set; }

        /// <summary> Have we reported that we stopped moving? </summary>
        public bool ReportedArrival { get; protected set; } = true;

        /// <summary> Have we reported that we began moving? </summary>
        public bool ReportedMove { get; protected set; }


        /// <summary> Force vector modified by the assigned weight  </summary>
        public Vector3 WeighedForce => Force * m_Weight;

        /// <summary> Vehicle that this behavior will influence </summary>
        public Vehicle Vehicle => m_Vehicle;

        /// <summary> Weight assigned to this steering behavior </summary>
        /// <remarks>
        /// The weight is used by WeighedForce to return a modified force value to
        /// the vehicle, which will then blend all weighed forces from its steerings
        /// to calculate the desired force.
        /// </remarks>
        public float Weight
        {
            get => m_Weight;
            set => m_Weight = value;
        }

        //////////////////////////////////////////////////////////////////////////
        protected virtual void Awake()
        {
            m_Vehicle = GetComponentInParent<Vehicle>();
        }

        protected virtual void OnEnable()
        {
            (IsPostProcess ? m_Vehicle.SteeringPostprocessors : m_Vehicle.Steerings).Add(this);
        }

        protected virtual void OnDisable()
        {
            (IsPostProcess ? m_Vehicle.SteeringPostprocessors : m_Vehicle.Steerings).Remove(this);
        }

        /// <summary> Calculates the force supplied by this behavior </summary>
        /// <returns> A vector with the supplied force <see cref="Vector3"/> </returns>
        protected abstract Vector3 _calculateForce();
    }
}