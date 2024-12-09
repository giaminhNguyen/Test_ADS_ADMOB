#if DOTweenCustom
using System;
using DG.Tweening;
using UnityEngine;
using VInspector;

namespace UnityHelper
{
    [AddComponentMenu("DOTween Animation Custom/DOTween Move")]
    public class DOTweenMove : DOTweenAnimationBase
    {
        [SerializeField, Variants("From","To", "From - To")]
        private string _moveWay;
        [SerializeField]
        private AxisValue _axisValue;
    
        [SerializeField, ShowIfAny("_moveWay", "From","_moveWay","From - To")]
        private Vector3 _from;
        [SerializeField, ShowIfAny("_moveWay", "To","_moveWay","From - To")]
        private Vector3 _to;

        protected override void Start()
        {
            GenerationTween();
            base.Start();
        }
        
        protected override void GenerationTween()
        {
            if(!_hasTarget || _tweenGenerationCalled) return;

            switch (_moveWay)
            {
                case "To":
                    _from = _isRectTransform ? _targetRt.anchoredPosition3D : _targetTf.position;
                    break;
                case "From":
                    _to = _isRectTransform ? _targetRt.anchoredPosition3D : _targetTf.position;
                    break;
            }
        
            var oldValue = _isRectTransform ? _targetRt.anchoredPosition3D : _targetTf.position;
        
            _from = LoadValueAxis(oldValue, _from, _axisValue);
            _to   = LoadValueAxis(oldValue, _to, _axisValue);
            
            //
            if (_isRectTransform)
            {
                _targetRt.anchoredPosition3D = _from;
                _currentTween                = _targetRt.DOAnchorPos3D(_to, _duration);
            }
            else
            {
                _targetTf.position = _from;
                _currentTween      = _targetTf.DOMove(_to, _duration);
            }
            
            if(_currentTween == null) return;
            
            _currentTween.SetDelay(_delay).SetUpdate(_ignoreTimeScale).SetAutoKill(_autoKill).OnKill(() =>
            {
                _currentTween = null;
            });
            
            if (_easeType == Ease.INTERNAL_Custom)
            {
                _currentTween.SetEase(_easeCurve);
            }
            else
            {
                _currentTween.SetEase(_easeType);
            }

            _currentTween.Pause();
            _tweenGenerationCalled = true;

        }
    
        private Vector3 LoadValueAxis(Vector3 oldValue, Vector3 newValue, AxisValue axisValue)
        {
            var axisValueStr = axisValue.ToString();
            if (!axisValueStr.Contains("X"))
            {
                newValue.x = oldValue.x;
            }
            if (!axisValueStr.Contains("Y"))
            {
                newValue.y = oldValue.y;
            }
            if (!axisValueStr.Contains("Z"))
            {
                newValue.z = oldValue.z;
            }
        
            return newValue;
        }
    }
}
#endif