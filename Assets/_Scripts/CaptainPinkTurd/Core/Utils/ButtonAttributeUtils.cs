using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CaptainPinkTurd.Core.Attributes;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CaptainPinkTurd.Core.Utils
{
#if UNITY_EDITOR
    public static class ButtonAttributeUtils
    {
        private const BindingFlags MethodFlags =
            BindingFlags.Instance |
            BindingFlags.Public |
            BindingFlags.NonPublic |
            BindingFlags.DeclaredOnly;

        #region Public API
        public static void DrawButton(Object target)
        {
            if (!target)
                return;

            foreach (var entry in GetButtonMethods(target.GetType()))
            {
                if (GUILayout.Button(entry.Label))
                {
                    InvokeSafely(entry.Method, new[] { target });
                }
            }
        }
        
        public static void DrawButtons(UnityEditor.Editor editor)
        {
            var targets = editor.targets;
            if (targets == null || targets.Length == 0)
                return;

            var type = editor.target.GetType();

            foreach (var entry in GetButtonMethods(type))
            {
                if (GUILayout.Button(entry.Label))
                {
                    InvokeSafely(entry.Method, targets);
                }
            }
        }
        
        #endregion
        
        #region Internal logic

        private static IEnumerable<ButtonEntry> GetButtonMethods(Type type)
        {
            var entries = new List<ButtonEntry>();

            while (type != null && type != typeof(MonoBehaviour))
            {
                var methods = type.GetMethods(MethodFlags);

                foreach (var method in methods)
                {
                    var button = method.GetCustomAttribute<ButtonAttribute>();
                    if (button == null)
                        continue;

                    if (method.GetParameters().Length > 0)
                        continue;

                    entries.Add(new ButtonEntry(
                        method,
                        string.IsNullOrEmpty(button.Label)
                            ? ObjectNames.NicifyVariableName(method.Name)
                            : button.Label
                    ));
                }

                type = type.BaseType;
            }

            // Stable order (important for UX)
            return entries.OrderBy(e => e.Label);
        }

        private static void InvokeSafely(MethodInfo method, Object[] targets)
        {
            Undo.RecordObjects(targets, method.Name);

            foreach (var target in targets)
            {
                try
                {
                    method.Invoke(target, null);
                    EditorUtility.SetDirty(target);
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            }
        }
        #endregion
        
        #region Helper struct

        private readonly struct ButtonEntry
        {
            public readonly MethodInfo Method;
            public readonly string Label;

            public ButtonEntry(MethodInfo method, string label)
            {
                Method = method;
                Label = label;
            }
        }
        
        #endregion
    }
#endif
}