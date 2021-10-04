using System;

namespace CoreLib
{
    public sealed class OnUpdateCallback : CallbackBase
    {
        public Action Action    { get; set; }
	
        // =======================================================================
        private void Update()
        {
            Action.Invoke();
        }
    }
}