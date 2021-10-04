using UnityEngine;

namespace CoreLib
{
    public class LifeTimeUnscaled : MonoBehaviour
    {
        public float TimeLeft;

        // =======================================================================
        private void Update()
        {
            if (TimeLeft <= 0)
                Destroy(gameObject);

            TimeLeft -= Time.unscaledDeltaTime;
        }
    }
}