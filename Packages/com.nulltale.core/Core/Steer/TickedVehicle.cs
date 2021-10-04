using UnityEngine;

namespace CoreLib.Steer
{
    public abstract class TickedVehicle : Vehicle, ITicked
    {
        [SerializeField]
        private TickedQueue m_SteeringQueue;

        [SerializeField] [Tooltip("How often will this Vehicle's steering calculations be ticked.")]
        private float m_TickInterval = ATicked.k_DefaultTickLength;

        public  TickedQueue SteeringQueue => m_SteeringQueue;

        public float TickLength { get => m_TickInterval; set => m_TickInterval = value; }


        //////////////////////////////////////////////////////////////////////////
        protected virtual void OnEnable()
        {
            m_SteeringQueue.Add(this);
        }

        protected void OnDisable()
        {
            _deQueue();
        }

        public override Vector3 CalculateOffset(float deltaTime)
        {
            return _calculateOffset(deltaTime);
        }

        public void Stop()
        {
            _zeroVelocity();
        }

        public void OnTicked()
        {
            if (enabled)
                _calculateForces();
            else
                _deQueue();
        }

        //////////////////////////////////////////////////////////////////////////
        protected abstract void _updateVelocity(Vector3 velocity);

        protected abstract Vector3 _calculateOffset(float deltaTime);

        protected abstract void _zeroVelocity();

        protected void _calculateForces()
        {
            if (MaxForce.IsAproximatlyZero() || MaxSpeed.IsAproximatlyZero())
                return;

            UnityEngine.Profiling.Profiler.BeginSample("Calculating vehicle forces");

            var force = Vector3.zero;
            foreach (var s in Steerings)
                if (s.enabled)
                    force += s.WeighedForce;

            LastRawForce = force;

            // Enforce speed limit.  Steering behaviors are expected to return a
            // final desired velocity, not a acceleration, so we apply them directly.
            var newVelocity = Vector3.ClampMagnitude(force / Mass, MaxForce);

            if (newVelocity.sqrMagnitude == 0)
            {
                _zeroVelocity();
                DesiredVelocity = Vector3.zero;
            }
            else
                DesiredVelocity = newVelocity;

            // Adjusts the velocity by applying the post-processing behaviors.
            //
            // This currently is not also considering the maximum force, nor 
            // blending the new velocity into an accumulator. We *could* do that,
            // but things are working just fine for now, and it seems like
            // overkill. 
            var adjustedVelocity = Vector3.zero;
            foreach (var s in SteeringPostprocessors)
                if (s.enabled)
                    adjustedVelocity += s.WeighedForce;


            if (adjustedVelocity != Vector3.zero)
            {
                adjustedVelocity = Vector3.ClampMagnitude(adjustedVelocity, MaxSpeed);
                newVelocity = adjustedVelocity;
            }

            // Update vehicle velocity
            _updateVelocity(newVelocity);
            UnityEngine.Profiling.Profiler.EndSample();
        }

        private void _deQueue()
        {
            m_SteeringQueue.Remove(this);
        }
    }
}