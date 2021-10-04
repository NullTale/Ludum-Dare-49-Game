using System.Linq;
using UnityEngine;

namespace CoreLib.Module
{
    [CreateAssetMenu(fileName = nameof(TickedQueue), menuName = Core.k_CoreModuleMenu + nameof(TickedQueue))]
    public class TickedQueue : Core.Module
    {
		public ScriptableObjectCollection<CoreLib.TickedQueue> m_Queues;

        // =======================================================================
        public override void Init()
        {
			var go = new GameObject("TickedQueue");
			go.transform.SetParent(Core.Instance.gameObject.transform);

			var tickedManager = go.AddComponent<TickedQueueManager>();

			tickedManager.Init(m_Queues.Values.ToArray());
        }
    }

}