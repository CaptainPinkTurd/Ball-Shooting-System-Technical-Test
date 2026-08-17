using System.Collections.Generic;
using CaptainPinkTurd.Core.Attributes;
using UnityEditor;
using UnityEngine;

namespace CaptainPinkTurd.Core.Editor
{
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(InlineScriptableObjectAttribute))]
    public class InlineScriptableObjectDrawer : PropertyDrawer
    {
        private static readonly Dictionary<string, bool> FoldoutStates = new Dictionary<string, bool>();

        private static bool showCreatePopup = false;
        private static string pendingAssetName = "";
        private static string pendingFolderPath = "Assets";

        private const int ButtonWidth = 70;

        private string GetKey(SerializedProperty property)
            => property.serializedObject.targetObject.GetInstanceID() + ":" + property.propertyPath;

        private bool GetFoldout(SerializedProperty property)
        {
            string key = GetKey(property);
            if (!FoldoutStates.TryGetValue(key, out bool state))
            {
                state = false;
                FoldoutStates[key] = false;
            }
            return state;
        }

        private void SetFoldout(SerializedProperty property, bool value)
            => FoldoutStates[GetKey(property)] = value;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float line = EditorGUIUtility.singleLineHeight;
            float spacing = EditorGUIUtility.standardVerticalSpacing;

            float total = line;

            if (!property.objectReferenceValue)
            {
                if (!((InlineScriptableObjectAttribute)attribute).AllowCreate)
                    return total;

                if (!showCreatePopup)
                    return total + spacing + line;

                float popupHeight =
                    (line * 3) +       // Name, Folder Row, Buttons Row
                    (spacing * 4) +    // Spacing between rows
                    16f;               // Padding inside box

                return total + popupHeight;
            }

            if (GetFoldout(property))
            {
                var so = new SerializedObject(property.objectReferenceValue);
                var it = so.GetIterator();
                bool enterChildren = true;

                while (it.NextVisible(enterChildren))
                {
                    if (it.name == "m_Script")
                    {
                        enterChildren = false;
                        continue;
                    }

                    total += EditorGUIUtility.standardVerticalSpacing +
                             EditorGUI.GetPropertyHeight(it, true);

                    enterChildren = false;
                }
            }

            return total;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            float line = EditorGUIUtility.singleLineHeight;
            float spacing = EditorGUIUtility.standardVerticalSpacing;
            var attr = (InlineScriptableObjectAttribute)attribute;

            var foldRect = new Rect(position.x, position.y, 15, line);
            var fieldRect = new Rect(position.x + 15, position.y, position.width - 15 - ButtonWidth - 6, line);
            var nullButtonRect = new Rect(position.x + position.width - ButtonWidth, position.y, ButtonWidth, line);

            bool foldout = GetFoldout(property);
            SetFoldout(property, EditorGUI.Foldout(foldRect, foldout, GUIContent.none, true));

            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(fieldRect, property, label);

            if (GUI.Button(nullButtonRect, "Set Null"))
            {
                property.objectReferenceValue = null;
                property.serializedObject.ApplyModifiedProperties();
                SetFoldout(property, false);
                RepaintAllInspectors();
                return;
            }

            if (EditorGUI.EndChangeCheck())
            {
                property.serializedObject.ApplyModifiedProperties();
                RepaintAllInspectors();
            }

            float yOffset = position.y + line + spacing;

            // CREATE POPUP
            if (!property.objectReferenceValue)
            {
                if (!attr.AllowCreate)
                    return;

                var createBtnRect = new Rect(position.x, yOffset, position.width, line);

                if (!showCreatePopup)
                {
                    if (GUI.Button(createBtnRect, "Create Asset..."))
                    {
                        pendingAssetName = GetFieldType().Name;
                        pendingFolderPath = "Assets";
                        showCreatePopup = true;
                    }
                    return;
                }

                // Draw clean popup background (no helpbox)
                float popupHeight = (line * 3) + (spacing * 4) + 16f;
                var boxRect = new Rect(position.x, yOffset, position.width, popupHeight);

                GUI.Box(boxRect, GUIContent.none, EditorStyles.helpBox);

                float py = yOffset + 8;

                // Asset Name
                pendingAssetName = EditorGUI.TextField(
                    new Rect(position.x + 10, py, position.width - 20, line),
                    "Asset Name",
                    pendingAssetName
                );
                py += line + spacing;

                // Folder
                EditorGUI.LabelField(
                    new Rect(position.x + 10, py, position.width - ButtonWidth - 20, line),
                    "Folder: " + pendingFolderPath
                );

                if (GUI.Button(new Rect(position.x + position.width - ButtonWidth - 10, py, ButtonWidth, line), "Browse"))
                {
                    string folder = EditorUtility.OpenFolderPanel("Choose Folder", "Assets", "");
                    if (!string.IsNullOrEmpty(folder) && folder.StartsWith(Application.dataPath))
                        pendingFolderPath = "Assets" + folder.Substring(Application.dataPath.Length);
                }
                py += line + spacing;

                // Create / Cancel
                var createRect = new Rect(position.x + 10, py, (position.width / 2) - 15, line);
                var cancelRect = new Rect(position.x + (position.width / 2) + 5, py, (position.width / 2) - 15, line);

                if (GUI.Button(createRect, "Create"))
                {
                    string finalPath = AssetDatabase.GenerateUniqueAssetPath($"{pendingFolderPath}/{pendingAssetName}.asset");

                    ScriptableObject so = ScriptableObject.CreateInstance(GetFieldType());
                    AssetDatabase.CreateAsset(so, finalPath);
                    AssetDatabase.SaveAssets();

                    property.objectReferenceValue = so;
                    property.serializedObject.ApplyModifiedProperties();

                    SetFoldout(property, true);
                    showCreatePopup = false;

                    RepaintAllInspectors();
                    GUIUtility.ExitGUI();
                }

                if (GUI.Button(cancelRect, "Cancel"))
                    showCreatePopup = false;

                return;
            }

            // INLINE INSPECTOR
            if (GetFoldout(property))
            {
                EditorGUI.indentLevel++;
                var so = new SerializedObject(property.objectReferenceValue);
                so.Update();

                var it = so.GetIterator();
                bool enterChildren = true;
                float y = yOffset;

                while (it.NextVisible(enterChildren))
                {
                    if (it.name == "m_Script")
                    {
                        enterChildren = false;
                        continue;
                    }

                    float h = EditorGUI.GetPropertyHeight(it, true);
                    EditorGUI.PropertyField(new Rect(position.x, y, position.width, h), it, true);

                    y += h + spacing;
                    enterChildren = false;
                }

                so.ApplyModifiedProperties();
                EditorGUI.indentLevel--;
            }
        }

        private System.Type GetFieldType() => fieldInfo.FieldType;

        private static void RepaintAllInspectors()
        {
            var inspectorType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.InspectorWindow");
            var inspectors = Resources.FindObjectsOfTypeAll(inspectorType);
            foreach (var i in inspectors)
                ((EditorWindow)i).Repaint();
        }
    }
#endif
}