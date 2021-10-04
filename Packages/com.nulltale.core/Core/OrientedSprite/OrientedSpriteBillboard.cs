using UnityEngine;

namespace CoreLib
{
    public class OrientedSpriteBillboard : MonoBehaviour
    {
        // =======================================================================
        private void OnWillRenderObject()
        {
            // set rotation to target
            transform.LookAt(Core.Instance.Camera.transform);
        }
    }
}