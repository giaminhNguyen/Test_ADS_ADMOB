#if DOTweenCustom
using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using VInspector;

namespace UnityHelper
{
    [RequireComponent(typeof(DOTweenScale))]
    public class LayerPopup : LayerBase
    {
        #region Properties
        [EndFoldout]
        [Header("References")]

        [SerializeField]
        private DOTweenScale _doTweenScale;
        
        [SerializeField]
        private Image _dimImage;

        [SerializeField]
        private Button _btnClose;

        [Header("--")]
        [SerializeField]
        private float _dimAlpha = 0.8f;
        [SerializeField]
        private float _animTimeDim = 0.12f;
        [SerializeField]
        private float   _timeAnimScale;
        private Tweener _dimTweener;

        
        
        
        #endregion

        protected override void OnValidate()
        {
            base.OnValidate();

            if (!_doTweenScale)
            {
                _doTweenScale = GetComponent<DOTweenScale>();
            }
            _timeAnimScale  = _doTweenScale.Duration;
        }

        protected override void InitAwake()
        {
            var color = _dimImage.color;
            color.a         = 0;
            _dimImage.color = color;
            _dimTweener     = _dimImage.DOFade(_dimAlpha, _animTimeDim);
            _dimTweener.SetAutoKill(false);
            _dimTweener.Pause();
            _btnClose.onClick.AddListener(Close);
            _dimImage.enabled = false;
            _content.SetActive(false);
        }

        private void OnDestroy()
        {
            _btnClose.onClick.RemoveListener(Close);
        }


        protected override void InitOnEnable()
        {

        }


        protected override void InitStart()
        {
            
        }

        public override void Init()
        {
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public override void Open()
        {
            base.Open();
            _dimImage.enabled = true;
            _content.SetActive(true);
            _dimTweener.PlayForward();
            DOVirtual.DelayedCall(_animTimeDim * 0.8f, () =>
            {
                _doTweenScale.Restart();
                DOVirtual.DelayedCall(_timeAnimScale, () => PlayEventOpen(true));
            });
        }

        public override void Close()
        {
            base.Close();
            _doTweenScale.PlayBackward();
            DOVirtual.DelayedCall(_timeAnimScale * 0.8f, () =>
            {
                _dimTweener.PlayBackwards();
                DOVirtual.DelayedCall(_animTimeDim, () =>
                {
                    PlayEventClose(true);
                    _dimImage.enabled = false;
                    _content.SetActive(false);
                });
            });
        }
    }
}
#endif