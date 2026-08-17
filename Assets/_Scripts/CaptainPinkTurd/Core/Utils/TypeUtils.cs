using System;
using System.Reflection;
using UnityEngine;

namespace CaptainPinkTurd.Core.Utils
{
    public static class TypeUtils
    {
        /// <summary>
        /// Copies all field values from one instance of an object to another of the same type
        /// using reflection. This performs a shallow copy of all fields (including private ones).
        /// </summary>
        /// <remarks>
        /// This method uses reflection to iterate through all fields of the object's type and
        /// assign each field value from <paramref name="source"/> to <paramref name="copy"/>.
        /// </remarks>
        public static void CopyValues<T>(T source, T copy)
        {
            Type type = source.GetType();
            
            // Iterate through the inheritance chain (base classes)
            // STOP automatically if we hit a Unity base class to avoid overwriting native pointers
            while (type != null && type != typeof(UnityEngine.Object) && type != typeof(ScriptableObject))
            {
                // Get all instance fields, including private ones
                FieldInfo[] fields = type.GetFields(BindingFlags.Public 
                    | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly); 
                //DeclareOnly will get Only the field in the current type, no other explicit type from base or child i think
                
                foreach (FieldInfo field in fields)
                {
                    field.SetValue(copy, field.GetValue(source));
                }
                
                type = type.BaseType;
            }
        }
    }
}
