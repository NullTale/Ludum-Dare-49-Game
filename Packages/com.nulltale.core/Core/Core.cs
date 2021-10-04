using System;
using System.Linq;
using Cinemachine;
using Malee;
using UnityEngine;
using NaughtyAttributes;

namespace CoreLib
{
    public interface IWorldPosition
    {
        Vector3 WorldPosition { get; }
    }

    public interface IDirtyUpdate
    {
        void DirtyUpdate();
    }
    
    public interface IContextHandler<in TContext>
    {
        void Handle(TContext context);
    }

    [DisallowMultipleComponent, DefaultExecutionOrder(Core.k_ManagerDefaultExecutionOrder)]
    public partial class Core : MonoBehaviour
    {
        public const           string             k_CoreModuleMenu               = "Core Module/";

        public static readonly WaitForFixedUpdate k_WaitForFixedUpdate           = new WaitForFixedUpdate();
        public static readonly WaitForEndOfFrame  k_WaitForEndOfFrame            = new WaitForEndOfFrame();
        public const           int                k_ManagerDefaultExecutionOrder = -10;

        public static Core s_Instance;
        public static Core Instance
        {
            get
            {
#if UNITY_EDITOR
                if (Application.isPlaying == false || s_Instance == null)
                    s_Instance = FindObjectOfType<Core>();
#endif
                return s_Instance;
            }
        }

        [SerializeField]
        private Camera				        m_Camera;
        public Camera				        Camera => m_Camera;
        private CinemachineBrain	        m_CameraBrain;
        public CinemachineBrain			    CameraBrain => m_CameraBrain;

        public bool						    m_DoNotDestroyOnLoad = true;
        
        [SerializeField]
        private ThreadPriority              m_LoadingPriority;

        [SerializeField]
        private Optional<int> m_FPS = new Optional<int>(60, false);
        public int FST
        {
            get => m_FPS.Value;
            set
            {
                Application.targetFrameRate = value;
                m_FPS.Value = value;
                m_FPS.Enabled = true;
            }
        }
	
        [SerializeField, Reorderable(elementNameProperty = "m_Module", surrogateType = typeof(Module), surrogateProperty = "m_Module")]
        private ModuleList                  m_Modules;

        // =======================================================================
        public interface IModule
        {
            void Init();
        }

        public abstract class Module : ScriptableObject, IModule
        {
            public abstract void Init();
        }

        //[CreateAssetMenu(fileName = nameof(Module), menuName = Core.k_CoreModuleMenu + nameof(Module))]
        public abstract class Module<T> : Module
            where T: Module
        {
            protected static T s_Instance;
            public static T Instance
            {
                get
                {
#if UNITY_EDITOR
                    // for unity editor tools
                    if (s_Instance == null && Application.isEditor && Application.isPlaying == false)
                        Instance = Resources.FindObjectsOfTypeAll<T>().FirstOrDefault();
#endif
                    return s_Instance;
                }

                private set => s_Instance = value;
            }
        }

        [Serializable]
        public class ModuleWrapper
        {
            [SerializeField]
            private bool                    m_Active;
            [SerializeField]
            [Expandable]
            private UnityEngine.Object      m_Module;

            private IModule                 m_Instance;

            public IModule                  Module => m_Active ? m_Instance : null;

            // =======================================================================
            public void Init()
            {
                if (m_Active == false)
                    return;

                if (m_Module == null)
                    return;

                switch (m_Module)
                {
                    case GameObject go:
                    {
                        // instantiate if instance not a scene object
                        m_Instance = go.gameObject.scene.name == null ? (Instantiate(go, Core.Instance.transform) as GameObject).GetComponent<IModule>() : go.GetComponent<IModule>();
                    } break;

                    case ScriptableObject so:
                        m_Instance = m_Module as IModule;
                        break;

                    default:
                        m_Instance = null;
                        break;
                }

                if (m_Instance != null)
                {
                    // set singleton instance
                    var mod = m_Instance.GetType().GetBaseTypes()
                                        .FirstOrDefault(n => n.IsGenericType &&
                                                             n.GetGenericTypeDefinition() == typeof(Module<>));
                    if (mod != null)
                    {
                        var prop = mod.GetProperty("Instance");
                        prop.SetValue(m_Instance, m_Instance);
                    }

                    // init module
                    m_Instance.Init();
                }
            }
        }

        [Serializable]
        public class ModuleList : ReorderableArray<ModuleWrapper> {}

        [Serializable]
        public enum ComparisonOperation
        {
            Less,
            Greater,
            Equal,
            NotEqual,
            LessOrEqual,
            GreaterOrEqual,
            Any,
            None
        }

        [Serializable]
        public enum ProjectSpace
        {
            XY,
            XZ
        }

        // =======================================================================
        private void Awake()
        {
            Application.backgroundLoadingPriority = m_LoadingPriority;
            
            // set instance
            s_Instance = this;

            // get main camera if not set
            m_Camera = m_Camera ? m_Camera : Camera.main;
            m_CameraBrain = m_Camera?.GetComponent<CinemachineBrain>();
            
            // set target fps
            if (m_FPS.Enabled)
                Application.targetFrameRate = m_FPS.Value;

            // set do not destroy on load
            if (m_DoNotDestroyOnLoad)
                DontDestroyOnLoad(gameObject);

            // init modules
            foreach (var module in m_Modules)
                module.Init();
        }

        private void Update() 
        {
#if UNITY_EDITOR
            if (UnityEngine.InputSystem.Keyboard.current.pauseKey.wasPressedThisFrame)
                Debug.Break();
#endif
        }

        private void OnValidate()
        {
            // set target fps
            if (m_FPS.Enabled)
                Application.targetFrameRate = m_FPS.Value;
        }

        public static void Log(string text)
        {
            Debug.Log(text);
        }

        public T GetModule<T>() where T : class, IModule
        {
            return m_Modules.FirstOrDefault(n => n.Module is T)?.Module as T;
        }
    }
}
