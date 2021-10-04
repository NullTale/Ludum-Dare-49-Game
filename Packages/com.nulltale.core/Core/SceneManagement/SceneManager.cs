using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;


namespace CoreLib.SceneManagement
{
    [Serializable]
    public class SceneManager
    {
        public float      m_SceneArgsLifetime = 1.0f;

        // scene name, data dictionary
        private Dictionary<string, Queue<SceneArgs>> m_SceneArguments = new Dictionary<string, Queue<SceneArgs>>();

        // =======================================================================
        public AsyncOperationHandle<SceneInstance> LoadSceneAsync(AssetReference sceneAsset, SceneArgs args, LoadSceneMode loadMode, bool activate, int priority = 100)
        {
            var ao = Addressables.LoadSceneAsync(sceneAsset, loadMode, activate, priority);
            // add args
            //_addArgs(sceneAsset.editorAsset.name, args);

            ao.Completed += aoh =>
            {
                if (aoh.Status != AsyncOperationStatus.Succeeded)
                {
                    TakeArgs<SceneArgs>(aoh.Result.Scene.name);
                    return;
                }

                // take args if not taken
                Core.Instance.Delayed(() => TakeArgs<SceneArgs>(aoh.Result.Scene.name), m_SceneArgsLifetime);
            };

            return ao;
        }

        public AsyncOperation UnloadSceneAsync(Scene scene)
        {
            return UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(scene);
        }
        
        public AsyncOperationHandle<SceneInstance> UnloadSceneAsync(AsyncOperationHandle<SceneInstance> scene, bool autoRelease = true)
        {
            return Addressables.UnloadSceneAsync(scene, autoRelease);
        }

        public TArgs TakeArgs<TArgs>(string sceneName) where TArgs : SceneArgs
        {
            // pop up arguments from dictionary
            if (m_SceneArguments.TryGetValue(sceneName, out var queue))
            {
                var args = queue.Dequeue();
                if (queue.Count == 0)
                    m_SceneArguments.Remove(sceneName);

                return args as TArgs;
            }

            return default;
        }

        // =======================================================================
        private void _addArgs(string sceneName, SceneArgs args)
        {
            if (args == null)
                return;

            if (m_SceneArguments.TryGetValue(sceneName, out var queue) == false)
            {
                queue = new Queue<SceneArgs>();
                m_SceneArguments.Add(sceneName, queue);
            }

            queue.Enqueue(args);
        }
    }
}