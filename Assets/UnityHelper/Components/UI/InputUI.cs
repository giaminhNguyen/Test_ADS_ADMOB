using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UnityHelper
{
    public class InputUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        #region Properties
        [SerializeField]
        private float _shakeThreshold = 0.2f;
        //
        private Vector2 _startPosition;
        private Vector2 _previousPosition;
        private bool    _isInteracting;
        private Vector3 _lastAcceleration;

        #endregion
        
        #region implement
        private void OnEnable()
        {
            EventManager.onMouseInteract += OnMouseInteract;
        }

        private void OnDisable()
        {
            EventManager.onMouseInteract -= OnMouseInteract;
        }

        private void Update()
        {
            CheckForShake();
        }
        #endregion
        

        private bool OnMouseInteract()
        {
            return _isInteracting;
        }

        /// <summary>
        /// Được gọi khi phát hiện sự kiện nhấn chuột.
        /// </summary>
        /// <param name="eventData">Dữ liệu sự kiện chuột.</param>
        public void OnPointerDown(PointerEventData eventData)
        {
            _startPosition    = eventData.position;
            _previousPosition = eventData.position;
            _isInteracting    = true;
            EventManager.onPointerDown?.Invoke(eventData.position);
        }

        /// <summary>
        /// Được gọi khi phát hiện sự kiện thả chuột.
        /// </summary>
        /// <param name="eventData">Dữ liệu sự kiện chuột.</param>
        public void OnPointerUp(PointerEventData eventData)
        {
            _isInteracting = false;
            EventManager.onPointerUp?.Invoke(eventData.position);
        }
        /// <summary>
        /// Được gọi khi phát hiện sự kiện kéo chuột.
        /// </summary>
        /// <param name="eventData">Dữ liệu sự kiện chuột.</param>
        public void OnDrag(PointerEventData eventData)
        {
            var currentPosition = eventData.position;
            EventManager.onDrag?.Invoke(_startPosition, _previousPosition, currentPosition);
            _previousPosition = currentPosition;
        }
        
        private void CheckForShake()
        {
            Vector3 acceleration      = Input.acceleration;
            Vector3 deltaAcceleration = acceleration - _lastAcceleration;
            _lastAcceleration = acceleration;

            if (deltaAcceleration.sqrMagnitude >= _shakeThreshold * _shakeThreshold)
            {
                EventManager.onShake?.Invoke();
            }
        }
        
    }
}