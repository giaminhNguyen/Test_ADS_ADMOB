using UnityEngine;

namespace UnityHelper
{
    public class EventSender : MonoBehaviour
    {
        #region Properteis

        [SerializeField]
        private DispatcherEventInfo _event;

        #endregion

        public void SendEvent()
        {
            _event.PostEvent();
        }
        
    }
}