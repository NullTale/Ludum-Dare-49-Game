using System;

namespace CoreLib
{
    public class OnDestroyCallback : CallbackBase
    {
        public Action					Action;

        // =======================================================================
        private void OnDestroy()
        {
            Action?.Invoke();
        }
    }
}