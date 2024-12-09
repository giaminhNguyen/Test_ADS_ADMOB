using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using UnityEngine;
#if UNITY_EDITOR
using static VInspector.Libs.VUtils;
#endif


namespace VInspector
{
    public class ButtonAttribute : System.Attribute
    {
        public string name;

        public ButtonAttribute() => this.name = "";
        public ButtonAttribute(string name) => this.name = name;
    }

    public class ButtonSizeAttribute : System.Attribute
    {
        public float size;

        public ButtonSizeAttribute(float size) => this.size = size;
    }

    public class ButtonSpaceAttribute : System.Attribute
    {
        public float space;

        public ButtonSpaceAttribute() => this.space = 10;
        public ButtonSpaceAttribute(float space) => this.space = space;
    }


    public class RangeResettable : PropertyAttribute
    {
        public float min;
        public float max;

        public RangeResettable(float min, float max)
        {
            this.min = min;
            this.max = max;
        }
    }


    public class VariantsAttribute : PropertyAttribute
    {
        public string[]     variants;
        public VariantsType type;

        public VariantsAttribute(params string[] variants)
        {
            this.variants = variants;
            type          = VariantsType.@string;
        }

        public VariantsAttribute(params int[] variants)
        {
            this.variants = new String[variants.Length];

            for (int i = 0; i < variants.Length; i++)
            {
                this.variants[i] = variants[i].ToString();
            }

            type = VariantsType.@int;
        }

        public VariantsAttribute(params float[] variants)
        {
            this.variants = new String[variants.Length];

            for (var i = 0; i < variants.Length; i++)
            {
                this.variants[i] = variants[i].ToString(CultureInfo.InvariantCulture);
            }

            type = VariantsType.@float;
        }

        // public VariantsAttribute(params Vector2[] variants){
        //     this.variants = new String[variants.Length];
        //
        //     for (var i = 0; i < variants.Length; i++)
        //     {
        //         this.variants[i] = variants[i].ToString();
        //     }
        //     type = VariantsType.vector2;
        // }
        //
        // public VariantsAttribute(params Vector3[] variants){
        //     this.variants = new String[variants.Length];
        //
        //     for (var i = 0; i < variants.Length; i++)
        //     {
        //         this.variants[i] = variants[i].ToString();
        //     }
        //     type = VariantsType.vector3;
        // }
        //
        // public VariantsAttribute(params Vector4[] variants){
        //     this.variants = new String[variants.Length];
        //
        //     for (var i = 0; i < variants.Length; i++)
        //     {
        //         this.variants[i] = variants[i].ToString();
        //     }
        //     type = VariantsType.vector4;
        // }
    }

    [Serializable]
    public enum VariantsType
    {
        @string,
        @int,
        @float,
        @vector2,
        @vector3,
        @vector4
    }


    public class TabAttribute : System.Attribute
    {
        public string name;

        public TabAttribute(string name) => this.name = name;
    }

    public class EndTabAttribute : System.Attribute
    {
    }


    public class FoldoutAttribute : System.Attribute
    {
        public string name;

        public FoldoutAttribute(string name) => this.name = name;
    }

    public class EndFoldoutAttribute : System.Attribute
    {
    }


    public abstract class IfAttribute : System.Attribute
    {
        public  String[] variableNames  = new string[5];
        public  object[] variableValues = new object[5];
        private int      varialbeSize;

        #if UNITY_EDITOR
        public bool Evaluate(object target)
        {
            for (int i = 0; i < varialbeSize; i++)
            {
                var rs = CheckValue(target, variableNames[i], variableValues[i]);

                if (!rs)
                {
                    return false;
                }
            }

            return true;
        }

        public bool CheckValue(object target, string variableName, object variableValue)
        {
            var name = variableName;

            if (name.Length == 0)
            {
                return false;
            }

            var value     = variableValue;
            var keepValue = true;

            if (name.IndexOf("!", StringComparison.Ordinal) == 0)
            {
                keepValue = false;
                name      = name.Substring(1);
            }

            return target.GetMemberValue(name, false) is object curValue && keepValue.Equals(curValue.Equals(value));
        }

        #endif

        private void SetValueOnVariableNameArray(params string[] arr)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                variableNames[i] = arr[i];
            }
        }

        private void SetValueOnVariableValueArray(params object[] arr)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                variableValues[i] = arr[i];
            }
        }

        public IfAttribute(string variableName, object variableValue)
        {
            varialbeSize = 1;
            SetValueOnVariableNameArray(variableName);
            SetValueOnVariableValueArray(variableValue);
        }

        public IfAttribute(string variableName1, object variableValue1, string variableName2, object variableValue2)
        {
            varialbeSize = 2;
            SetValueOnVariableNameArray(variableName1, variableName2);
            SetValueOnVariableValueArray(variableValue1, variableValue2);
        }

        public IfAttribute(string variableName1, object variableValue1, string variableName2, object variableValue2,
                           string variableName3, object variableValue3)
        {
            varialbeSize = 3;
            SetValueOnVariableNameArray(variableName1, variableName2, variableName3);
            SetValueOnVariableValueArray(variableValue1, variableValue2, variableValue3);
        }

        public IfAttribute(string variableName1, object variableValue1, string variableName2, object variableValue2,
                           string variableName3, object variableValue3, string variableName4, object variableValue4)
        {
            varialbeSize = 4;
            SetValueOnVariableNameArray(variableName1, variableName2, variableName3, variableName4);
            SetValueOnVariableValueArray(variableValue1, variableValue2, variableValue3, variableValue4);
        }

        public IfAttribute(string variableName1, object variableValue1, string variableName2, object variableValue2,
                           string variableName3, object variableValue3, string variableName4, object variableValue4,
                           string variableName5, object variableValue5)
        {
            varialbeSize = 5;
            SetValueOnVariableNameArray(variableName1, variableName2, variableName3, variableName4, variableName5);
            SetValueOnVariableValueArray(variableValue1, variableValue2, variableValue3, variableValue4,
                    variableValue5);
        }
    }

    public abstract class IfAnyAttribute : System.Attribute
    {
        public  String[] variableNames  = new string[5];
        public  object[] variableValues = new object[5];
        private int      varialbeSize;

        #if UNITY_EDITOR
        public bool Evaluate(object target)
        {
            for (int i = 0; i < varialbeSize; i++)
            {
                var rs = CheckValue(target, variableNames[i], variableValues[i]);

                if (rs)
                {
                    return true;
                }
            }

            return false;
        }

        public bool CheckValue(object target, string variableName, object variableValue)
        {
            var name = variableName;

            if (name.Length == 0)
            {
                return false;
            }

            var value     = variableValue;
            var keepValue = true;

            if (name.IndexOf("!", StringComparison.Ordinal) == 0)
            {
                keepValue = false;
                name      = name.Substring(1);
            }

            return target.GetMemberValue(name, false) is object curValue && keepValue.Equals(curValue.Equals(value));
        }

        #endif

        private void SetValueOnVariableNameArray(params string[] arr)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                variableNames[i] = arr[i];
            }
        }

        private void SetValueOnVariableValueArray(params object[] arr)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                variableValues[i] = arr[i];
            }
        }

        public IfAnyAttribute(string variableName1, object variableValue1, string variableName2, object variableValue2)
        {
            varialbeSize = 2;
            SetValueOnVariableNameArray(variableName1, variableName2);
            SetValueOnVariableValueArray(variableValue1, variableValue2);
        }

        public IfAnyAttribute(string variableName1, object variableValue1, string variableName2, object variableValue2,
                              string variableName3, object variableValue3)
        {
            varialbeSize = 3;
            SetValueOnVariableNameArray(variableName1, variableName2, variableName3);
            SetValueOnVariableValueArray(variableValue1, variableValue2, variableValue3);
        }

        public IfAnyAttribute(string variableName1, object variableValue1, string variableName2, object variableValue2,
                              string variableName3, object variableValue3, string variableName4, object variableValue4)
        {
            varialbeSize = 4;
            SetValueOnVariableNameArray(variableName1, variableName2, variableName3, variableName4);
            SetValueOnVariableValueArray(variableValue1, variableValue2, variableValue3, variableValue4);
        }

        public IfAnyAttribute(string variableName1, object variableValue1, string variableName2, object variableValue2,
                              string variableName3, object variableValue3, string variableName4, object variableValue4,
                              string variableName5, object variableValue5)
        {
            varialbeSize = 5;
            SetValueOnVariableNameArray(variableName1, variableName2, variableName3, variableName4, variableName5);
            SetValueOnVariableValueArray(variableValue1, variableValue2, variableValue3, variableValue4,
                    variableValue5);
        }
    }

    public class HideIfAttribute : IfAttribute
    {
        public HideIfAttribute(string variableName, object variableValue) : base(variableName, variableValue)
        {
        }

        public HideIfAttribute(string variableName1, object variableValue1, string variableName2, object variableValue2)
                : base(variableName1, variableValue1, variableName2, variableValue2)
        {
        }

        public HideIfAttribute(string variableName1, object variableValue1, string variableName2, object variableValue2,
                               string variableName3, object variableValue3) : base(variableName1, variableValue1,
                variableName2, variableValue2, variableName3, variableValue3)
        {
        }

        public HideIfAttribute(string variableName1, object variableValue1, string variableName2, object variableValue2,
                               string variableName3, object variableValue3, string variableName4,
                               object variableValue4) : base(variableName1, variableValue1, variableName2,
                variableValue2, variableName3, variableValue3, variableName4, variableValue4)
        {
        }

        public HideIfAttribute(string variableName1, object variableValue1, string variableName2, object variableValue2,
                               string variableName3, object variableValue3, string variableName4, object variableValue4,
                               string variableName5, object variableValue5) : base(variableName1, variableValue1,
                variableName2, variableValue2, variableName3, variableValue3, variableName4, variableValue4,
                variableName5, variableValue5)
        {
        }
    }

    public class ShowIfAttribute : IfAttribute
    {
        public ShowIfAttribute(string variableName, object variableValue) : base(variableName, variableValue)
        {
        }

        public ShowIfAttribute(string variableName1, object variableValue1, string variableName2, object variableValue2)
                : base(variableName1, variableValue1, variableName2, variableValue2)
        {
        }

        public ShowIfAttribute(string variableName1, object variableValue1, string variableName2, object variableValue2,
                               string variableName3, object variableValue3) : base(variableName1, variableValue1,
                variableName2, variableValue2, variableName3, variableValue3)
        {
        }

        public ShowIfAttribute(string variableName1, object variableValue1, string variableName2, object variableValue2,
                               string variableName3, object variableValue3, string variableName4,
                               object variableValue4) : base(variableName1, variableValue1, variableName2,
                variableValue2, variableName3, variableValue3, variableName4, variableValue4)
        {
        }

        public ShowIfAttribute(string variableName1, object variableValue1, string variableName2, object variableValue2,
                               string variableName3, object variableValue3, string variableName4, object variableValue4,
                               string variableName5, object variableValue5) : base(variableName1, variableValue1,
                variableName2, variableValue2, variableName3, variableValue3, variableName4, variableValue4,
                variableName5, variableValue5)
        {
        }
    }

    public class EnableIfAttribute : IfAttribute
    {
        public EnableIfAttribute(string variableName, object variableValue) : base(variableName, variableValue)
        {
        }

        public EnableIfAttribute(string variableName1, object variableValue1, string variableName2,
                                 object variableValue2) : base(variableName1, variableValue1, variableName2,
                variableValue2)
        {
        }

        public EnableIfAttribute(string variableName1,  object variableValue1, string variableName2,
                                 object variableValue2, string variableName3,  object variableValue3) : base(
                variableName1, variableValue1, variableName2, variableValue2, variableName3, variableValue3)
        {
        }

        public EnableIfAttribute(string variableName1,  object variableValue1, string variableName2,
                                 object variableValue2, string variableName3,  object variableValue3,
                                 string variableName4,  object variableValue4) : base(variableName1, variableValue1,
                variableName2, variableValue2, variableName3, variableValue3, variableName4, variableValue4)
        {
        }

        public EnableIfAttribute(string variableName1,  object variableValue1, string variableName2,
                                 object variableValue2, string variableName3,  object variableValue3,
                                 string variableName4,  object variableValue4, string variableName5,
                                 object variableValue5) : base(variableName1, variableValue1, variableName2,
                variableValue2, variableName3, variableValue3, variableName4, variableValue4, variableName5,
                variableValue5)
        {
        }
    }

    public class DisableIfAttribute : IfAttribute
    {
        public DisableIfAttribute(string variableName, object variableValue) : base(variableName, variableValue)
        {
        }

        public DisableIfAttribute(string variableName1, object variableValue1, string variableName2,
                                  object variableValue2) : base(variableName1, variableValue1, variableName2,
                variableValue2)
        {
        }

        public DisableIfAttribute(string variableName1,  object variableValue1, string variableName2,
                                  object variableValue2, string variableName3,  object variableValue3) : base(
                variableName1, variableValue1, variableName2, variableValue2, variableName3, variableValue3)
        {
        }

        public DisableIfAttribute(string variableName1,  object variableValue1, string variableName2,
                                  object variableValue2, string variableName3,  object variableValue3,
                                  string variableName4,  object variableValue4) : base(variableName1, variableValue1,
                variableName2, variableValue2, variableName3, variableValue3, variableName4, variableValue4)
        {
        }

        public DisableIfAttribute(string variableName1,  object variableValue1, string variableName2,
                                  object variableValue2, string variableName3,  object variableValue3,
                                  string variableName4,  object variableValue4, string variableName5,
                                  object variableValue5) : base(variableName1, variableValue1, variableName2,
                variableValue2, variableName3, variableValue3, variableName4, variableValue4, variableName5,
                variableValue5)
        {
        }
    }

    public class HideIfAnyAttribute : IfAnyAttribute
    {
        public HideIfAnyAttribute(string variableName1, object variableValue1, string variableName2,
                                  object variableValue2) : base(variableName1, variableValue1, variableName2,
                variableValue2)
        {
        }

        public HideIfAnyAttribute(string variableName1,  object variableValue1, string variableName2,
                                  object variableValue2, string variableName3,  object variableValue3) : base(
                variableName1, variableValue1, variableName2, variableValue2, variableName3, variableValue3)
        {
        }

        public HideIfAnyAttribute(string variableName1,  object variableValue1, string variableName2,
                                  object variableValue2, string variableName3,  object variableValue3,
                                  string variableName4,  object variableValue4) : base(variableName1, variableValue1,
                variableName2, variableValue2, variableName3, variableValue3, variableName4, variableValue4)
        {
        }

        public HideIfAnyAttribute(string variableName1,  object variableValue1, string variableName2,
                                  object variableValue2, string variableName3,  object variableValue3,
                                  string variableName4,  object variableValue4, string variableName5,
                                  object variableValue5) : base(variableName1, variableValue1, variableName2,
                variableValue2, variableName3, variableValue3, variableName4, variableValue4, variableName5,
                variableValue5)
        {
        }
    }

    public class ShowIfAnyAttribute : IfAnyAttribute
    {
        public ShowIfAnyAttribute(string variableName1, object variableValue1, string variableName2,
                                  object variableValue2) : base(variableName1, variableValue1, variableName2,
                variableValue2)
        {
        }

        public ShowIfAnyAttribute(string variableName1,  object variableValue1, string variableName2,
                                  object variableValue2, string variableName3,  object variableValue3) : base(
                variableName1, variableValue1, variableName2, variableValue2, variableName3, variableValue3)
        {
        }

        public ShowIfAnyAttribute(string variableName1,  object variableValue1, string variableName2,
                                  object variableValue2, string variableName3,  object variableValue3,
                                  string variableName4,  object variableValue4) : base(variableName1, variableValue1,
                variableName2, variableValue2, variableName3, variableValue3, variableName4, variableValue4)
        {
        }

        public ShowIfAnyAttribute(string variableName1,  object variableValue1, string variableName2,
                                  object variableValue2, string variableName3,  object variableValue3,
                                  string variableName4,  object variableValue4, string variableName5,
                                  object variableValue5) : base(variableName1, variableValue1, variableName2,
                variableValue2, variableName3, variableValue3, variableName4, variableValue4, variableName5,
                variableValue5)
        {
        }
    }

    public class EnableIfAnyAttribute : IfAnyAttribute
    {
        public EnableIfAnyAttribute(string variableName1, object variableValue1, string variableName2,
                                    object variableValue2) : base(variableName1, variableValue1, variableName2,
                variableValue2)
        {
        }

        public EnableIfAnyAttribute(string variableName1,  object variableValue1, string variableName2,
                                    object variableValue2, string variableName3,  object variableValue3) : base(
                variableName1, variableValue1, variableName2, variableValue2, variableName3, variableValue3)
        {
        }

        public EnableIfAnyAttribute(string variableName1,  object variableValue1, string variableName2,
                                    object variableValue2, string variableName3,  object variableValue3,
                                    string variableName4,  object variableValue4) : base(variableName1, variableValue1,
                variableName2, variableValue2, variableName3, variableValue3, variableName4, variableValue4)
        {
        }

        public EnableIfAnyAttribute(string variableName1,  object variableValue1, string variableName2,
                                    object variableValue2, string variableName3,  object variableValue3,
                                    string variableName4,  object variableValue4, string variableName5,
                                    object variableValue5) : base(variableName1, variableValue1, variableName2,
                variableValue2, variableName3, variableValue3, variableName4, variableValue4, variableName5,
                variableValue5)
        {
        }
    }

    public class DisableIfAnyAttribute : IfAnyAttribute
    {
        public DisableIfAnyAttribute(string variableName1, object variableValue1, string variableName2,
                                     object variableValue2) : base(variableName1, variableValue1, variableName2,
                variableValue2)
        {
        }

        public DisableIfAnyAttribute(string variableName1,  object variableValue1, string variableName2,
                                     object variableValue2, string variableName3,  object variableValue3) : base(
                variableName1, variableValue1, variableName2, variableValue2, variableName3, variableValue3)
        {
        }

        public DisableIfAnyAttribute(string variableName1,  object variableValue1, string variableName2,
                                     object variableValue2, string variableName3,  object variableValue3,
                                     string variableName4,  object variableValue4) : base(variableName1, variableValue1,
                variableName2, variableValue2, variableName3, variableValue3, variableName4, variableValue4)
        {
        }

        public DisableIfAnyAttribute(string variableName1,  object variableValue1, string variableName2,
                                     object variableValue2, string variableName3,  object variableValue3,
                                     string variableName4,  object variableValue4, string variableName5,
                                     object variableValue5) : base(variableName1, variableValue1, variableName2,
                variableValue2, variableName3, variableValue3, variableName4, variableValue4, variableName5,
                variableValue5)
        {
        }
    }

    public class EndIfAttribute : System.Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class DrawGreenDividingLine : System.Attribute
    {
        public float TopSpacing    { get; }
        public float BottomSpacing { get; }

        public DrawGreenDividingLine() : this(0, 0)
        {
        }

        public DrawGreenDividingLine(float topSpacing, float bottomSpacing)
        {
            TopSpacing    = topSpacing;
            BottomSpacing = bottomSpacing;
        }
    }
}