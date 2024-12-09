#if UNITY_EDITOR
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using System.Reflection;
using System.Linq;
using System.Text.RegularExpressions;
using static VInspector.Libs.VUtils;
using static VInspector.Libs.VGUI;



namespace VInspector
{
    [CustomPropertyDrawer(typeof(VariantsAttribute))]
    public class VIVariantsDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect rect, SerializedProperty prop, GUIContent label)
        {
            var variantsAttr = ((VariantsAttribute)attribute);
            var variants     = variantsAttr.variants;
            var stringProp   = "";

            switch (variantsAttr.type)
            {
                case VariantsType.@string: stringProp = prop.stringValue;
                    break;
                case VariantsType.@int: stringProp = prop.intValue.ToString();
                    break;
                case VariantsType.@float: stringProp = prop.floatValue.ToString();
                    break;
            }
            
            EditorGUI.BeginProperty(rect, label, prop);

            var iCur = prop.hasMultipleDifferentValues ? -1 : variants.ToList().IndexOf(stringProp);

            var iNew = EditorGUI.IntPopup(rect, label.text, iCur, variants, Enumerable.Range(0, variants.Length).ToArray());

            stringProp = null;
            
            if (iNew != -1)
                stringProp = variants[iNew];
            else if (!prop.hasMultipleDifferentValues)
                stringProp = variants[0];

            if (stringProp != null)
            {
                switch (variantsAttr.type)
                {
                    case VariantsType.@string: 
                        prop.stringValue = stringProp; 
                        break;
                    case VariantsType.@int: 
                        int a = int.Parse(stringProp);
                        prop.intValue = int.Parse(stringProp); 
                        break;
                    case VariantsType.@float: 
                        prop.floatValue = float.Parse(stringProp); 
                        break;
                    // case VariantsType.vector2: 
                    //     prop.vector2Value = valueProp is Vector2 vt2 ? vt2 : Vector2.zero; 
                    //     break;
                    // case VariantsType.vector3: 
                    //     prop.vector3Value = valueProp is Vector3 vt3 ? vt3 : Vector3.zero; 
                    //     break;
                    // case VariantsType.vector4: 
                    //     prop.vector4Value = valueProp is Vector4 vt4 ? vt4 : Vector4.zero; 
                    //     break;
                    
                }
            }
            
            EditorGUI.EndProperty();

        }
    }
}
#endif