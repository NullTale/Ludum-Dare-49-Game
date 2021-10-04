using System.Collections.Generic;
using UnityEngine;

namespace CoreLib.Steer
{
    public abstract class Vehicle : MonoBehaviour
    {
        [SerializeField] 
        private float m_MinSpeedForTurning = 0.1f;

        [SerializeField] 
        private float m_TurnTime = 0.25f;

        [SerializeField]
        private float m_Mass = 1;

        [SerializeField] 
        private float m_ArrivalRadius = 0.25f;


        [SerializeField] 
        private float m_MaxSpeed = 1;

        [SerializeField] 
        private float m_MaxForce = 10;

        public Vector3 DesiredVelocity { get; protected set; }

        public float Mass
        {
            get => m_Mass;
            set => m_Mass = Mathf.Max(0, value);
        }

        public float MaxForce
        {
            get => m_MaxForce;
            set => m_MaxForce = Mathf.Clamp(value, 0, float.MaxValue);
        }

        public float MaxSpeed
        {
            get => m_MaxSpeed;
            set => m_MaxSpeed = Mathf.Clamp(value, 0, float.MaxValue);
        }

        public float MinSpeedForTurning => m_MinSpeedForTurning;

        public float ArrivalRadius
        {
            get => m_ArrivalRadius;
            set
            {
                m_ArrivalRadius       = Mathf.Clamp(value, 0.01f, float.MaxValue);
                SquaredArrivalRadius = m_ArrivalRadius * m_ArrivalRadius;
            }
        }

        public float SquaredArrivalRadius { get; private set; }

        /// <summary>
        /// Last raw force applied to the vehicle. It is expected to be set 
        /// by the subclasses.
        /// </summary>
        public Vector3 LastRawForce { get; protected set; }


        public float TurnTime
        {
            get => m_TurnTime;
            set => m_TurnTime = Mathf.Max(0, value);
        }

        public List<Steering> Steerings              { get; } = new List<Steering>();
        public List<Steering> SteeringPostprocessors { get; } = new List<Steering>();
        public Vector3        Orientation            { get; protected set; }

        public float TargetSpeed { get; protected set; }

        public abstract float Speed { get; }
        public abstract Vector3 Velocity { get; }

        //////////////////////////////////////////////////////////////////////////
        protected virtual void Awake()
        {
            SquaredArrivalRadius = ArrivalRadius * ArrivalRadius;
        }

        public abstract Vector3 CalculateOffset(float deltaTime);

        //////////////////////////////////////////////////////////////////////////
        public Vector3 PredictFuturePosition(float predictionTime)
        {
            return transform.position + (Velocity * predictionTime);
        }

        public Vector3 PredictFutureDesiredPosition(float predictionTime)
        {
            return transform.position + (DesiredVelocity * predictionTime);
        }

        public bool IsInNeighborhood(Vehicle other, float minDistance, float maxDistance, float cosMaxAngle)
        {
            var result = false;
            if (other != this)
            {
                var offset          = other.transform.position - transform.position;
                var distanceSquared = offset.sqrMagnitude;

                // definitely in neighborhood if inside minDistance sphere
                if (distanceSquared < (minDistance * minDistance))
                {
                    result = true;
                }
                else
                {
                    // definitely not in neighborhood if outside maxDistance sphere
                    if (distanceSquared <= (maxDistance * maxDistance))
                    {
                        // otherwise, test angular offset from forward axis
                        var unitOffset  = offset / Mathf.Sqrt(distanceSquared);
                        var forwardness = Vector3.Dot(transform.forward, unitOffset);
                        result = forwardness > cosMaxAngle;
                    }
                }
            }

            return result;
        }

        public Vector3 GetSeekVector(Vector3 target, bool considerVelocity = false)
        {
            /*
		     * First off, we calculate how far we are from the target, If this
		     * distance is smaller than the configured vehicle radius, we tell
		     * the vehicle to stop.
		     */
            var force = Vector3.zero;

            var difference = target - transform.position;
            var d          = difference.sqrMagnitude;
            if (d > SquaredArrivalRadius)
            {
                /*
			     * But suppose we still have some distance to go. The first step
			     * then would be calculating the steering force necessary to orient
			     * ourselves to and walk to that point.
			     * 
			     * It doesn't apply the steering itself, simply returns the value so
			     * we can continue operating on it.
			     */
                force = considerVelocity ? difference - Velocity : difference;
            }

            return force;
        }

        public Vector3 GetTargetSpeedVector(float targetSpeed)
        {
            var mf         = MaxForce;
            var speedError = targetSpeed - Speed;
            return transform.forward * Mathf.Clamp(speedError, -mf, +mf);
        }

        public float PredictNearestApproachTime(Vehicle other)
        {
            // imagine we are at the origin with no velocity,
            // compute the relative velocity of the other vehicle
            var otherVelocity = other.Velocity;
            var relVelocity   = otherVelocity - Velocity;
            var relSpeed      = relVelocity.magnitude;

            // for parallel paths, the vehicles will always be at the same distance,
            // so return 0 (aka "now") since "there is no time like the present"
            if (Mathf.Approximately(relSpeed, 0))
            {
                return 0;
            }

            // Now consider the path of the other vehicle in this relative
            // space, a line defined by the relative position and velocity.
            // The distance from the origin (our vehicle) to that line is
            // the nearest approach.

            // Take the unit tangent along the other vehicle's path
            var relTangent = relVelocity / relSpeed;

            // find distance from its path to origin (compute offset from
            // other to us, find length of projection onto path)
            var relPosition = transform.position - other.transform.position;
            var projection  = Vector3.Dot(relTangent, relPosition);

            return projection / relSpeed;
        }

        public float ComputeNearestApproachPositions(Vehicle other, float time, out Vector3 ourPosition, out Vector3 hisPosition)
        {
            return ComputeNearestApproachPositions(other, time, out ourPosition, out hisPosition, Speed,
                                                   transform.forward);
        }

        public float ComputeNearestApproachPositions(Vehicle other, float time, out Vector3 ourPosition, out Vector3 hisPosition, float ourSpeed, Vector3 ourForward)
        {
            var myTravel    = ourForward * ourSpeed * time;
            var otherTravel = other.transform.forward * other.Speed * time;

            ourPosition = transform.position + myTravel;
            hisPosition = other.transform.position + otherTravel;

            return Vector3.Distance(ourPosition, hisPosition);
        }
    }
}