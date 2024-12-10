#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace UnityHelper
{
    public class DefineSymbolTool : EditorWindow
    {
        private bool[]   symbolStates;
        private string[] symbols;
        private bool     hasChanges = false;

        [MenuItem("NGaMing/Utility Tools/Define Symbol Tool")]
        public static void ShowWindow()
        {
            GetWindow<DefineSymbolTool>("Define Symbol Tool");
        }

        private void OnEnable()
        {
            symbols      = DefineSymbolData.DefineSymbol;
            symbolStates = new bool[symbols.Length];
            LoadSymbolStates();
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Define Symbols", EditorStyles.boldLabel);

            for (var i = 0; i < symbols.Length; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(symbols[i], GUILayout.Width(200));

                var buttonStyle = new GUIStyle(GUI.skin.button);

                buttonStyle.normal.background =
                        symbolStates[i] ? MakeTex(2, 2, Color.green) : MakeTex(2, 2, Color.gray);
                buttonStyle.normal.textColor = symbolStates[i] ? Color.white : Color.black;

                if (GUILayout.Button(symbolStates[i] ? "Enable" : "Disable", buttonStyle, GUILayout.Width(100)))
                {
                    symbolStates[i] = !symbolStates[i];
                    hasChanges      = true;
                }

                EditorGUILayout.EndHorizontal();
            }

            if (hasChanges)
            {
                if (GUILayout.Button("Apply Changes"))
                {
                    ApplyChanges();
                    hasChanges = false;
                }
            }
        }

        private void LoadSymbolStates()
        {
            string currentSymbols =
                    PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);

            for (int i = 0; i < symbols.Length; i++)
            {
                symbolStates[i] = currentSymbols.Contains(symbols[i]);
            }
        }

        private void ApplyChanges()
        {
            string currentSymbols =
                    PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            var symbolList = new List<string>(currentSymbols.Split(';'));

            for (int i = 0; i < symbols.Length; i++)
            {
                if (symbolStates[i])
                {
                    if (!symbolList.Contains(symbols[i]))
                    {
                        symbolList.Add(symbols[i]);
                    }
                }
                else
                {
                    if (symbolList.Contains(symbols[i]))
                    {
                        symbolList.Remove(symbols[i]);
                    }
                }
            }

            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup,
                    string.Join(";", symbolList));
        }

        private Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];

            for (int i = 0; i < pix.Length; i++)
            {
                pix[i] = col;
            }

            var result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();

            return result;
        }
    }
}
#endif