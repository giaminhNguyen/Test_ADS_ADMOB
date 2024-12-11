using System;

namespace UnityHelper
{
    [Serializable]
    public enum EventID
    {
        None = EnumValue.EventDispatcher,
        OnRetryCheckInternet = None + 1,
        FetchRemoteConfigComplete = None + 2,
    }
    
    //
    
    [Serializable]
    public enum PrimitiveDataType
    {
        Int,
        Float,
        String,
        Bool,
        None
    }
}