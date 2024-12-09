using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnityHelper
{
    public class LoadingSceneCtrl : Singleton<LoadingSceneCtrl>
    {
        #region Properties

        public bool LoadQueue
        {
            get => _isLoadQueue;
            set
            {
                _isLoadQueue = value;

                if (!_isLoadQueue)
                {
                    _sceneLoadQueue.Clear();
                }
            }
        }
        public float  Progress            => _loadingOperation?.progress ?? 0f;
        public bool   CanDone             => Progress >= 0.9f;
        public string CurrentSceneLoading { get; set; }
        public bool   IsLoading           { get; set; }
        
        private event Action _onSceneLoadStarted;
        private event Action _onSceneLoadCompleted;
        private event Action _onSceneActivated;

        private bool                _isLoadQueue;
        private AsyncOperation      _loadingOperation;
        private bool                _allowSceneActivation;
        private List<SceneLoadInfo> _sceneLoadQueue = new List<SceneLoadInfo>();
        
        #endregion
        
        public async void LoadSceneAsync(string sceneName, bool allowSceneActivation = true, Action onSceneLoadStarted = null, Action onSceneLoadCompleted = null, Action onSceneActivated = null)
        {
            if (IsLoading)
            {
                if (_isLoadQueue)
                {
                    _sceneLoadQueue.Add(new SceneLoadInfo
                    {
                        SceneName            = sceneName,
                        AllowSceneActivation = allowSceneActivation,
                        OnSceneLoadStarted   = onSceneLoadStarted,
                        OnSceneLoadCompleted = onSceneLoadCompleted,
                        OnSceneActivated     = onSceneActivated
                    });
                }
                return;
            }

            IsLoading             = true;
            _allowSceneActivation = allowSceneActivation;
            _onSceneLoadStarted   = onSceneLoadStarted;
            _onSceneLoadCompleted = onSceneLoadCompleted;
            _onSceneActivated     = onSceneActivated;

            CurrentSceneLoading                    = sceneName;
            _loadingOperation                      = SceneManager.LoadSceneAsync(sceneName);
            _loadingOperation.allowSceneActivation = false;
            while (_loadingOperation.progress < 0.9f)
            {
                await Task.Yield();
            }

            _onSceneLoadCompleted?.Invoke();

            if (_allowSceneActivation)
            {
                ActivateScene();
            }
        }
        
        public void LoadSceneAsync(SceneLoadInfo sceneLoadInfo)
        {
            LoadSceneAsync(sceneLoadInfo.SceneName, sceneLoadInfo.AllowSceneActivation, sceneLoadInfo.OnSceneLoadStarted, sceneLoadInfo.OnSceneLoadCompleted, sceneLoadInfo.OnSceneActivated);
        }
        
        public void ActivateScene()
        {
            if (!IsLoading || _loadingOperation.allowSceneActivation) return;

            _loadingOperation.allowSceneActivation = true;
            _loadingOperation.completed += _ =>
            {
                _onSceneActivated?.Invoke();
                IsLoading           = false;
                if (_sceneLoadQueue.Count > 0)
                {
                    LoadSceneAsync(_sceneLoadQueue[0]);
                    _sceneLoadQueue.RemoveAt(0);
                }
            };
        }


        #region Struct
        
        [Serializable]
        public struct SceneLoadInfo
        {
            public string SceneName;
            public bool   AllowSceneActivation;
            public Action OnSceneLoadStarted;
            public Action OnSceneLoadCompleted;
            public Action OnSceneActivated;
        }

        #endregion
    }

    public static class LoadingSceneCtrlExtension
    {
        public static void LoadScene_LoadAsync(this MonoBehaviour monoBehaviour, string sceneName, bool allowSceneActivation = true, Action onSceneLoadStarted = null, Action onSceneLoadCompleted = null, Action onSceneActivated = null)
        {
            LoadingSceneCtrl.Instance.LoadSceneAsync(sceneName, allowSceneActivation, onSceneLoadStarted, onSceneLoadCompleted, onSceneActivated);
        }
        
        public static void LoadScene_LoadAsync(this MonoBehaviour monoBehaviour, LoadingSceneCtrl.SceneLoadInfo sceneLoadInfo)
        {
            LoadingSceneCtrl.Instance.LoadSceneAsync(sceneLoadInfo);
        }
        
        public static void LoadScene_Activate(this MonoBehaviour monoBehaviour)
        {
            LoadingSceneCtrl.Instance.ActivateScene();
        }
        
        public static float LoadScene_Progress(this MonoBehaviour monoBehaviour)
        {
            return LoadingSceneCtrl.Instance.Progress;
        }
        
        public static bool LoadScene_CanDone(this MonoBehaviour monoBehaviour)
        {
            return LoadingSceneCtrl.Instance.CanDone;
        }
        
        public static string LoadScene_CurrentScene(this MonoBehaviour monoBehaviour)
        {
            return LoadingSceneCtrl.Instance.CurrentSceneLoading;
        }
        
        public static bool LoadScene_IsLoading(this MonoBehaviour monoBehaviour)
        {
            return LoadingSceneCtrl.Instance.IsLoading;
        }
    }
    
}