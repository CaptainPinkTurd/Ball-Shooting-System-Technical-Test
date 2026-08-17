using CaptainPinkTurd.Core.Attributes;
using CaptainPinkTurd.Core.Utils;
using CaptainPinkTurd.UI.Theming;
using UnityEditor;
using UnityEngine;

namespace CaptainPinkTurd.UI.Components
{
    public abstract class CustomUIComponent : MonoBehaviour
    {
        [Header("Base UI Component Properties")]
        [SerializeField] protected StyleIdentifier styleId;

        protected virtual void OnEnable()
        {
            if (ThemeManager.Instance)
                ThemeManager.Instance.OnThemeChanged.AddListener(ApplyTheme);
            
            Init();
        }

        protected virtual void OnDisable()
        {
            if(!gameObject.scene.isLoaded) return;
            
            if (ThemeManager.Instance)
                ThemeManager.Instance.OnThemeChanged.RemoveListener(ApplyTheme);
        }
        
        protected abstract void Setup();
        protected abstract void OnApplyTheme(ThemeData theme);

        [Button("Validate Changes")]
        internal void Init()
        {
            Setup();
            ApplyTheme();
        }

        protected void ApplyTheme()
        {
            if (!ThemeManager.Instance || !ThemeManager.Instance.CurrentTheme || !styleId) return;
            
            OnApplyTheme(ThemeManager.Instance.CurrentTheme);
        }
    }
    
    #if UNITY_EDITOR
    [CustomEditor(typeof(CustomUIComponent), true)]
    [CanEditMultipleObjects]
    public class CustomUIComponentEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            ButtonAttributeUtils.DrawButtons(this);
        }
    }
    #endif
}