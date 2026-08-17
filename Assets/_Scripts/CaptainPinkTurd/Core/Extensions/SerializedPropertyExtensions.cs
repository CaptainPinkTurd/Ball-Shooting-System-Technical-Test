#if UNITY_EDITOR
using CaptainPinkTurd.Core.Utils;
using UnityEditor;

namespace CaptainPinkTurd.Core.Extensions
{
    public static class SerializedPropertyExtensions
    {
        /// <summary>
        /// Returns the object that CONTAINS this property.
        /// For path "a.b.c", this returns the object for "a.b".
        /// Used for normal ShowIf behavior.
        /// </summary>
        public static object GetContainingObject(this SerializedProperty property)
        {
            if (property == null) return null;

            object obj = property.serializedObject.targetObject;
            string path = property.propertyPath.Replace(".Array.data[", "[");

            string[] parts = path.Split('.');
            int last = parts.Length - 1; // parent

            for (int i = 0; i < last; i++)
            {
                obj = SerializedPropertyUtils.Walk(obj, parts[i]);
                if (obj == null) return null;
            }

            return obj;
        }

        /// <summary>
        /// Returns the actual RUNTIME VALUE of the property,
        /// especially useful for managed reference fields.
        /// Use for applying ShowIf for serialized interfaces since those are custom-made drawer
        /// </summary>
        public static object GetUnderlyingValue(this SerializedProperty property)
        {
            if (property == null) return null;

            object obj = property.serializedObject.targetObject;
            string path = property.propertyPath.Replace(".Array.data[", "[");

            foreach (var part in path.Split('.'))
            {
                obj = SerializedPropertyUtils.Walk(obj, part);
                if (obj == null) return null;
            }

            return obj;
        }
    }
}
#endif