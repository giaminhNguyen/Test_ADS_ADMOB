using System;

namespace UnityHelper
{
    [Serializable]
    public enum EventID
    {
        None = EnumValue.EventDispatcher,
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