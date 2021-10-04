using UnityEngine;

namespace CoreLib
{
    public class LifeTime : MonoBehaviour
    {
        public float TimeLeft;

        // =======================================================================
        private void Update()
        {
            if (TimeLeft <= 0)
                Destroy(gameObject);

            TimeLeft -= Time.deltaTime;
        }
    }
}