using System;

namespace UnityHelper
{
    [Serializable]
    public enum ActionVisualKey
    {
        None = EnumValue.DOTweenAction,
        PlayForward = None + 1,
        PlayBackward = None + 2,
        Rewind = None + 3,
        Restart = None + 4,
        Pause = None + 5,
        DestroyGameObject = None + 6,
    }

    [Serializable]
    public enum AxisValue
    {
        XYZ,
        XY,
        YZ,
        ZX,
        X,
        Y,
        Z
    }
}