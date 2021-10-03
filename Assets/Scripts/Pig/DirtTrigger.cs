using UnityEngine;

namespace Game
{
    public class DirtTrigger : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            other.attachedRigidbody?.GetComponent<Pig>()?.Dirt();
        }
    }
}