using System.Collections.Generic;
using UnityEngine;

namespace RendererTool
{
    #if UNITY_EDITOR
    public static class DataRendererTool
    {
        public static bool                                          DrawUndoInHierarchy = false;
        public static int                                           MaxUndoCount        = 10;
        public static Dictionary<GameObject, StackCustom<Material>> UndoMaterials       = new();
    }
    #endif
}