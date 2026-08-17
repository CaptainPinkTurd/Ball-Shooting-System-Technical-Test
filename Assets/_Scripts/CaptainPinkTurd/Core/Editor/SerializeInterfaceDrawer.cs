#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System;
using System.Linq;
using System.Reflection;
using CaptainPinkTurd.Core.Attributes;
using CaptainPinkTurd.Core.Enum;
using CaptainPinkTurd.Core.Extensions;

namespace CaptainPinkTurd.Core.Editor
{
    [CustomPropertyDrawer(typeof(SerializeInterfaceAttribute))]
    public class SerializeInterfaceDrawer : PropertyDrawer
    {
        private Type[] cachedTypes;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!ShouldShow(property))
                return 0f;

            if (property.managedReferenceValue == null)
                return EditorGUIUtility.singleLineHeight;

            return EditorGUIUtility.singleLineHeight + EditorGUI.GetPropertyHeight(property, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!ShouldShow(property))
                return;

            EditorGUI.BeginProperty(position, label, property);

            Rect header = new(position.x, position.y, position.width - 80, EditorGUIUtility.singleLineHeight);
            Rect button = new(position.x + position.width - 75, position.y, 75, EditorGUIUtility.singleLineHeight);

            EditorGUI.LabelField(header, label);

            if (GUI.Button(button, "Set Type"))
                ShowMenu(property);

            if (property.managedReferenceValue != null)
            {
                EditorGUI.indentLevel++;
                Rect body = new(
                    position.x,
                    position.y + EditorGUIUtility.singleLineHeight + 2,
                    position.width,
                    EditorGUI.GetPropertyHeight(property, true)
                );
                EditorGUI.PropertyField(body, property, GUIContent.none, true);
                EditorGUI.indentLevel--;
            }

            EditorGUI.EndProperty();
        }

        private void ShowMenu(SerializedProperty property)
        {
            var attr = (SerializeInterfaceAttribute)attribute;

            cachedTypes ??= AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a =>
                {
                    try { return a.GetTypes(); }
                    catch { return Array.Empty<Type>(); }
                })
                .Where(t => attr.TargetType.IsAssignableFrom(t) && t.IsClass && !t.IsAbstract)
                .ToArray();

            GenericMenu menu = new();

            foreach (Type t in cachedTypes)
            {
                menu.AddItem(new GUIContent(t.FullName), false, () =>
                {
                    property.managedReferenceValue = Activator.CreateInstance(t);
                    property.serializedObject.ApplyModifiedProperties();
                });
            }

            menu.ShowAsContext();
        }

        // --------- ShowIf evaluation ---------

        private bool ShouldShow(SerializedProperty property)
        {
            var showIf = fieldInfo.GetCustomAttribute<ShowIfAttribute>();
            if (showIf == null)
                return true;

            var instance = property.managedReferenceValue ?? property.GetUnderlyingValue();
            if (instance == null)
                return true;

            return EvaluateConditions(instance, showIf);
        }

        private bool EvaluateConditions(object target, ShowIfAttribute attr)
        {
            bool result = attr.Logic == EConditionalLogic.And;

            foreach (var (name, expected, comparison) in attr.Conditions)
            {
                var member = FindMemberInHierarchy(target.GetType(), name);

                if (member == null)
                {
                    Debug.LogWarning($"[ShowIf] '{name}' not found on {target.GetType().Name}");
                    continue;
                }

                object value = member is FieldInfo fi
                    ? fi.GetValue(target)
                    : ((PropertyInfo)member).GetValue(target);

                bool met = Compare(value, expected, comparison);

                if (attr.Logic == EConditionalLogic.And)
                    result &= met;
                else
                    result |= met;
            }

            return result;
        }

        private static MemberInfo FindMemberInHierarchy(Type type, string name)
        {
            while (type != null)
            {
                var field = type.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (field != null)
                    return field;

                var prop = type.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (prop != null)
                    return prop;

                type = type.BaseType;
            }
            return null;
        }

        private bool Compare(object a, object b, EComparisonType comp)
        {
            try
            {
                float fa = Convert.ToSingle(a);
                float fb = Convert.ToSingle(b);
                return comp switch
                {
                    EComparisonType.Equals => Mathf.Approximately(fa, fb),
                    EComparisonType.NotEquals => !Mathf.Approximately(fa, fb),
                    EComparisonType.Greater => fa > fb,
                    EComparisonType.Less => fa < fb,
                    EComparisonType.GreaterOrEqual => fa >= fb,
                    EComparisonType.LessOrEqual => fa <= fb,
                    _ => false,
                };
            }
            catch
            {
                return comp switch
                {
                    EComparisonType.Equals => Equals(a, b),
                    EComparisonType.NotEquals => !Equals(a, b),
                    _ => false,
                };
            }
        }
    }
}
#endif