using System;
using UnityEngine;
using UnityEngine.Events;

namespace UnityHelper
{
    public class EventReceiver : MonoBehaviour
    {
        #region Properties

        [SerializeField]
        private EventID _eventIDCatch;
        [SerializeField]
        private EventPostValue _eventValueCompare;
        [SerializeField]
        private UnityEvent _eventTrue;
        [SerializeField]
        private UnityEvent _eventFalse;

        #endregion
        
        private void OnEnable()
        {
            EventDispatcher.Instance.RegisterListener(_eventIDCatch,OnCatchEvent);
        }

        private void OnDisable()
        {
            EventDispatcher.Instance.RemoveListener(_eventIDCatch,OnCatchEvent);
        }

        private void OnCatchEvent(object obj)
        {
            var isCatch = true;

            if (_eventValueCompare.valuePostType != PrimitiveDataType.None)
            {
                try
                {
                    switch (_eventValueCompare.valuePostType)
                    {
                        case PrimitiveDataType.Int:
                            var objInt       = (int)obj;
                            isCatch = objInt.Equals(_eventValueCompare.@int);
                            break;
                        case PrimitiveDataType.Float:
                            var objFloat       = (float)obj;
                            isCatch = objFloat.Equals(_eventValueCompare.@float);
                            break;
                        case PrimitiveDataType.String:
                            var objString       = (string)obj;
                            isCatch = objString.Equals(_eventValueCompare.@string);
                            break;
                        case PrimitiveDataType.Bool:
                            var objBool       = (bool)obj;
                            isCatch = objBool.Equals(_eventValueCompare.@bool);
                            break;
                    }
                }
                catch (Exception)
                {
                    Debug.Log("Ép kiểu lỗi" + transform.name + " || parent : " + transform.parent.name);
                    isCatch = false;
                }
            }

            try
            {
                if (isCatch)
                {
                    _eventTrue?.Invoke();
                }
                else
                {
                    _eventFalse?.Invoke();
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"{transform.name} - {transform.parent.name} - {e} - {_eventTrue}");
            }
            
        }
    }
}