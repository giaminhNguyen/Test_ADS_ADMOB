#if DOTweenCustom
using System;
using UnityEngine;
using UnityEngine.Events;
using VInspector;
using DG.Tweening;



namespace UnityHelper
{
    public abstract class DOTweenAnimationBase : MonoBehaviour
    {
        #region Properties
        
        [Foldout("Manager")]
    
        [SerializeField]
        private ActionVisualKey _onEnable = ActionVisualKey.PlayForward;
        [SerializeField]
        private ActionVisualKey _onDisable = ActionVisualKey.Rewind;
        [SerializeField]
        private ActionVisualKey _onStart = ActionVisualKey.Restart;

        [EndFoldout]

        [Foldout("Event")]

        public UnityEvent eventPlayForward;

        public UnityEvent eventPlayBackward;

        public UnityEvent eventRewind;

        public Action actionPlayForward;
        public Action actionPlayBackward;
        public Action actionRewind;
        
        [EndFoldout]

        [Header("----")]
        [SerializeField]
        protected bool _autoKill = false;
        [Header("----")]
        
        [SerializeField]
        protected GameObject _target;
        [SerializeField]
        protected float _duration;
        [SerializeField]
        protected float _delay;
        [SerializeField]
        protected bool _ignoreTimeScale;
        [SerializeField]
        protected Ease _easeType = Ease.InOutQuad;
        
        [SerializeField,ShowIf("_easeType",Ease.INTERNAL_Custom)]
        protected AnimationCurve _easeCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
        
        protected bool    _hasTarget;
        protected Tweener _currentTween;
        protected bool    _tweenGenerationCalled;
        
        protected Transform     _targetTf;
        protected RectTransform _targetRt;
        protected bool          _isRectTransform;

        // PUBLIC
        public float Duration
        {
            get => _duration;
            set => _duration = value;
        }

        public float Delay
        {
            get => _delay;
            set => _delay = value;
        }

        public bool IgnoreTimeScale
        {
            get => _ignoreTimeScale;
            set => _ignoreTimeScale = value;
        }
        
        
        
        #endregion
        
        #region Unity Func

        protected virtual void Awake()
        {
            _hasTarget       = _target;

            if (!_hasTarget)
                return;
            _isRectTransform = _target.TryGetComponent(out _targetRt);
            _targetTf        = _target.transform;
        }

        protected virtual void OnValidate()
        {
            if (!_target)
            {
                _target = gameObject;
            }
        }

        protected virtual void OnEnable()
        {
            PlayActionVisual(_onEnable);
        }
    
        protected virtual void OnDisable()
        {
            PlayActionVisual(_onDisable);
        }

        protected virtual void Start()
        {
            PlayActionVisual(_onStart);
        }

    
    
        protected virtual void Update()
        {
        
        }
    
        protected virtual void LateUpdate()
        {
        
        }
    
        protected virtual void FixedUpdate()
        {
        
        }

        protected virtual void OnDestroy()
        {
        
        }

        #endregion

        protected abstract void GenerationTween();

    
        protected virtual void PlayActionVisual(ActionVisualKey actionVisualKey)
        {
            if(!_tweenGenerationCalled) return;
            if(actionVisualKey == ActionVisualKey.None) return;
            switch (actionVisualKey)
            {
                case ActionVisualKey.PlayForward:
                    PlayForward();
                    break;
                case ActionVisualKey.PlayBackward:
                    PlayBackward();
                    break;
                case ActionVisualKey.Rewind:
                    Rewind();
                    break;
                case ActionVisualKey.Restart:
                    Restart();
                    break;
                case ActionVisualKey.Pause:
                    Pause();
                    break;
                case ActionVisualKey.DestroyGameObject:
                    DestroyImmediate(gameObject);
                    break;
            }
        }
        

        #region Public Methods

        public virtual void PlayForward()
        {
            if(!_currentTween.IsActive()) return;
            _currentTween.PlayForward();
            eventPlayForward?.Invoke();
            actionPlayForward?.Invoke();
        }

        public virtual void PlayBackward()
        {
            if(!_currentTween.IsActive()) return;
            _currentTween.PlayBackwards(); 
            actionPlayBackward?.Invoke();
            eventPlayBackward?.Invoke();
        }

        public virtual void Rewind()
        {
            if(!_currentTween.IsActive()) return;
            _currentTween.Rewind(); 
            actionRewind?.Invoke();
            eventRewind?.Invoke();
        }

        public virtual void Restart()
        {
            Rewind();
            PlayForward();
        }

        public virtual void Pause()
        {
            if(!_currentTween.IsActive()) return;
            _currentTween.Pause(); 
        }

        #endregion

    }
}
#endif
