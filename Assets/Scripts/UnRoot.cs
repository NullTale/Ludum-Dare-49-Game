using UnityEngine;

namespace Game
{
    public class UnRoot : MonoBehaviour
    {
        public void Unroot()
        {
            gameObject.transform.SetParent(null);
        }
    }
}