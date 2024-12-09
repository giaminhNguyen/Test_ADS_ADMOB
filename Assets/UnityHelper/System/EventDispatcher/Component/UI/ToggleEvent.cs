using UnityEngine;
using UnityEngine.Events;
using VInspector;

namespace UnityHelper
{
    public class ToggleEvent : ToggleBase
    {
        [Tab("Toggle Event")]
        [SerializeField]
        private EventDataInfo _eventOnInfo;
        [SerializeField]
        private EventDataInfo _eventOffInfo;
        
        [EndTab]
        private void HandleEvent(bool state)
        {
            if (state)
            {
                _eventOnInfo.PostEvent();
            }
            else
            {
                _eventOffInfo.PostEvent();
            }
        }

    
        public override void OnChangeValue<T>(T value)
        {
            if(value is not bool boolValue) return;
            HandleEvent(boolValue);
        }
    }
}

