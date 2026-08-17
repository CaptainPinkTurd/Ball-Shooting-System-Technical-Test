using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CaptainPinkTurd.Core.Attributes
{
    /// <summary>
    /// Makes a field appear read-only in the Unity Inspector.
    /// </summary>
    public class ReadOnlyAttribute : PropertyAttribute { }
    
    #if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.enabled = false; // Disable editing
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = true;  // Re-enable GUI
        }
    }
    #endif
}
