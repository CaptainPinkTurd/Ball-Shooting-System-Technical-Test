#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System;
using System.Reflection;
using CaptainPinkTurd.Core.Attributes;
using CaptainPinkTurd.Core.Enum;
using CaptainPinkTurd.Core.Extensions;

namespace CaptainPinkTurd.Core.Editor
{
    [CustomPropertyDrawer(typeof(ShowIfAttribute))]
    public class ShowIfDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return ShouldShow(property)
                ? EditorGUI.GetPropertyHeight(property, label, true) + EditorGUIUtility.standardVerticalSpacing
                : 0f;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (ShouldShow(property))
                EditorGUI.PropertyField(position, property, label, true);
        }

        private bool ShouldShow(SerializedProperty property)
        {
            var attr = (ShowIfAttribute)attribute;

            // Only evaluate against the immediate containing object
            object owner = property.GetContainingObject();
            if (owner == null)
                return true;

            return EvaluateConditions(owner, attr);
        }

        private static MemberInfo FindMember(Type type, string name)
        {
            while (type != null)
            {
                var field = type.GetField
                (
                    name,
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
                );
                if (field != null)
                    return field;
        
                var property = type.GetProperty
                (
                    name,
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
                );
                if (property != null)
                    return property;
        
                type = type.BaseType;
            }
        
            return null;
        }
        private bool EvaluateConditions(object target, ShowIfAttribute attr)
        {
            bool result = attr.Logic == EConditionalLogic.And;

            foreach (var (name, expected, comparison) in attr.Conditions)
            {
                var member = FindMember(target.GetType(), name);

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