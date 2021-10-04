using System;
using UnityEngine;

namespace CoreLib.Module
{
    [CreateAssetMenu(fileName = nameof(KeepAspect), menuName = Core.k_CoreModuleMenu + nameof(KeepAspect))]
    public class KeepAspect : Core.Module
    {
        private float                       m_AspectRatio;

        // =======================================================================
        public override void Init()
        {
            // save initial aspect
            m_AspectRatio = Screen.height / (float)Screen.width;
            
            Core.Instance.gameObject.AddComponent<OnUpdateCallback>().Action = _update;
        }

        // =======================================================================
        private void _update()
        {
            if (Screen.fullScreen == false && Application.platform != RuntimePlatform.WebGLPlayer)
            {
                if (Math.Abs(Core.Instance.Camera.aspect - m_AspectRatio) > 0.01f)
                    Screen.SetResolution(Screen.width, (int) (Screen.width * m_AspectRatio), FullScreenMode.Windowed);
            }
        }
    }
}