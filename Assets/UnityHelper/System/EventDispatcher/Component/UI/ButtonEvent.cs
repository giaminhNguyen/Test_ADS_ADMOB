using System;
using UnityEngine;
using UnityEngine.Events;
using VInspector;

namespace UnityHelper
{
    public class ButtonEvent : ButtonBase
    {
            [Tab("Button Event")]
            //One Way
            [SerializeField]
            private EventDataInfo _eventOnInfo;
            //Two Way
            [SerializeField,ShowIf("_buttonTypeWay","Two Way")]
            private EventDataInfo _eventOffInfo;
            [EndIf]
            //
            [EndTab]
            
            private void HandleEvent(bool state = false)
            {
                var eventAction = _eventOnInfo;

                if (_buttonTypeWay.Equals("Two Way") && !state)
                {
                    eventAction = _eventOffInfo;
                }
                eventAction.PostEvent();
            }

            public override void OnClick()
            {
                HandleEvent();
            }

            public override void OnChangeValue<T>(T value)
            {
                if (value is not bool boolValue) return;
                HandleEvent(boolValue);
            }
    }

    [Serializable]
    public struct EventDataInfo
    {
        public DispatcherEventInfo dispatcherEventInfo;
        public UnityEvent unityEvent;
        
        public void PostEvent()
        {
            dispatcherEventInfo.PostEvent();
            unityEvent?.Invoke();
        }
    }


    [Serializable]
    public struct DispatcherEventInfo
    {
        public EventID        eventID;
        public EventPostValue eventValue;
        
        public bool PostEvent()
        {
            if (eventID == EventID.None) return false;
            EventDispatcher.Instance.PostEvent(eventID,eventValue.GetValuePost());
            return true;
        }
    }

    [Serializable]
    public struct EventPostValue
    {
        public PrimitiveDataType valuePostType;
        public int @int;
        public float @float;
        public string @string;
        public bool @bool;

        public object GetValuePost()
        {
            object value = valuePostType switch
            {
                    PrimitiveDataType.Int => @int,
                    PrimitiveDataType.Float => @float,
                    PrimitiveDataType.String => @string,
                    PrimitiveDataType.Bool => @bool,
                    _                     => null
            };

            return value;
        }
    }
}