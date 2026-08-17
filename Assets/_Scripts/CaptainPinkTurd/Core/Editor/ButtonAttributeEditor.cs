using CaptainPinkTurd.Core.Utils;
using UnityEditor;
using UnityEngine;

namespace CaptainPinkTurd.Core.Editor
{
#if UNITY_EDITOR
    [CustomEditor(typeof(MonoBehaviour), true)]
    public class ButtonAttributeEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            // Draw the default inspector first
            DrawDefaultInspector();
            ButtonAttributeUtils.DrawButton(target);
        }
    }
    #endif
}