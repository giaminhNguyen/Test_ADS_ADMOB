#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace UnityHelper
{
    public class DefineSymbolTool : EditorWindow
    {
        private bool[]   symbolStates;
        private bool[]   initialSymbolStates;
        private string[] symbols;
        private bool     hasChanges = false;

        [MenuItem("NGaMing/Utility Tools/Define Symbol Tool")]
        public static void ShowWindow()
        {
            GetWindow<DefineSymbolTool>("Define Symbol Tool");
        }

        private void OnEnable()
        {
            symbols             = DefineSymbolData.DefineSymbol;
            symbolStates        = new bool[symbols.Length];
            initialSymbolStates = new bool[symbols.Length];
            LoadSymbolStates();
        }

        private void OnGUI()
        {
            var checkChanges = false;

            for (var i = 0; i < symbols.Length; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(symbols[i], EditorStyles.boldLabel, GUILayout.Width(200));

                var buttonStyle = new GUIStyle(GUI.skin.button)
                {
                        fontStyle = FontStyle.Bold
                };

                if (symbolStates[i])
                {
                    buttonStyle.normal.background = MakeTex(2, 2, new Color(0, 0, 0.5f)); // Dark blue background
                    buttonStyle.normal.textColor  = Color.white;                          // White text
                }
                else
                {
                    buttonStyle.normal.background = MakeTex(2, 2, Color.gray); // Gray background
                    buttonStyle.normal.textColor  = Color.white;               // White text
                }

                if (GUILayout.Button(symbolStates[i] ? "Enable" : "Disable", buttonStyle, GUILayout.Width(100)))
                {
                    checkChanges    = true;
                    symbolStates[i] = !symbolStates[i];
                }

                EditorGUILayout.EndHorizontal();
            }

            if (checkChanges)
            {
                hasChanges = CheckForChanges();
            }

            if (hasChanges)
            {
                if (GUILayout.Button("Revert Changes"))
                {
                    RevertChanges();
                }
                if (GUILayout.Button("Apply Changes"))
                {
                    ApplyChanges();
                }
            }
        }

        private void LoadSymbolStates()
        {
            string currentSymbols =
                    PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);

            for (var i = 0; i < symbols.Length; i++)
            {
                initialSymbolStates[i]        = currentSymbols.Contains(symbols[i]);
            }
            RevertChanges();
        }

        private bool CheckForChanges()
        {
            for (int i = 0; i < symbols.Length; i++)
            {
                if (symbolStates[i] != initialSymbolStates[i])
                {
                    return true;
                }
            }

            return false;
        }

        private void ApplyChanges()
        {
            var currentSymbols =
                    PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            var symbolList = new List<string>(currentSymbols.Split(';'));

            for (var i = 0; i < symbols.Length; i++)
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

            LoadChanges();

        }

        private void LoadChanges()
        {
            Array.Copy(symbolStates, initialSymbolStates, symbolStates.Length);
            hasChanges = false;
        }

        private void RevertChanges()
        {
            Array.Copy(initialSymbolStates, symbolStates, initialSymbolStates.Length);
            hasChanges = false;
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