using System;
using UnityEngine;
using UnityEngine.Events;
using VInspector;

namespace UnityHelper
{
    public abstract class LayerBase : MonoBehaviour
    {
        #region Properties
        
        [Foldout("Event")]
        public UnityEvent eventUnityOpen;
        public UnityEvent eventUnityCompleteOpen;
        public Action     actionOpen;
        public Action     actionCompleteOpen;
        
        public UnityEvent eventUnityClose;
        public UnityEvent eventUnityCompleteClose;
        public Action     actionClose;
        public Action     actionCompleteClose;
        [EndFoldout]
        
        [SerializeField]
        protected GameObject _content;
        
        protected bool _hasContent;
        
        #endregion
        
        protected virtual void Awake()
        {
            _hasContent = _content;
            InitAwake();
        }

        protected virtual void OnValidate()
        {
            if (!_content)
            {
                _content = gameObject;
            }
        }

        protected virtual void OnEnable()
        {
            InitOnEnable();
        }

        protected virtual void Start()
        {
            InitStart();
        }

        protected abstract void InitAwake();
        protected abstract void InitOnEnable();
        protected abstract void InitStart();
        
        public abstract void Init();

        public virtual void Open()
        {
            PlayEventOpen();
        }

        public virtual void Close()
        {
            PlayEventClose();
        }

        protected virtual void PlayEventOpen(bool isComplete = false)
        {
            if (isComplete)
            {
                eventUnityCompleteOpen?.Invoke();
                actionCompleteOpen?.Invoke();
            }
            else
            {
                eventUnityOpen?.Invoke();
                actionOpen?.Invoke();
            }
        }

        protected virtual void PlayEventClose(bool isComplete = false)
        {
            if (isComplete)
            {
                eventUnityCompleteClose?.Invoke();
                actionCompleteClose?.Invoke();
            }
            else
            {
                eventUnityClose?.Invoke();
                actionClose?.Invoke();
            }
        }

    }
}