using UnityEngine;

namespace Game
{
    [DefaultExecutionOrder(-1000)]
    public class IglonreColision : MonoBehaviour
    {
        public Collider A;
        public Collider B;

        // =======================================================================
        private void Awake()
        {
            Physics.IgnoreCollision(A, B, true);
        }
    }
}