using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CaptainPinkTurd.Core.Interfaces
{
    [System.Serializable]
    public class InterfaceReference<T> where T : class
    {
        [SerializeField] private MonoBehaviour reference;
        public T Value => reference as T;
    }
    
#if UNITY_EDITOR
    
    [CustomPropertyDrawer(typeof(InterfaceReference<>), true)]
    public class InterfaceReferenceDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var refProp = property.FindPropertyRelative("reference");
            EditorGUI.PropertyField(position, refProp, label);

            if (refProp.objectReferenceValue is MonoBehaviour mb)
            {
                var targetType = fieldInfo.FieldType.GetGenericArguments()[0];
                if (!targetType.IsAssignableFrom(mb.GetType()))
                {
                    Debug.LogWarning(
                        $"{mb.GetType().Name} does not implement {targetType.Name}. Resetting.",
                        mb);

                    refProp.objectReferenceValue = null;
                }
            }

            EditorGUI.EndProperty();
        }
    }
    
#endif
}