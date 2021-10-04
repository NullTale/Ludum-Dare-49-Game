using System;

namespace CoreLib
{
    public sealed class OnFixedUpdateCallback : CallbackBase
    {
        public Action Action { get; set; }
	
        // =======================================================================
        private void FixedUpdate()
        {
            Action.Invoke();
        }
    }
}