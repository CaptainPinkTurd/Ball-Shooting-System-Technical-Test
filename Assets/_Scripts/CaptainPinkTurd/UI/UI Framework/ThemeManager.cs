using CaptainPinkTurd.Core.Attributes;
using CaptainPinkTurd.Core.DesignPattern.Singleton;
using CaptainPinkTurd.UI.Components;
using UnityEngine;
using UnityEngine.Events;

namespace CaptainPinkTurd.UI
{
    public class ThemeManager : Singleton<ThemeManager>
    {
        [SerializeField][InlineScriptableObject] private ThemeData currentTheme;
        
        public UnityEvent OnThemeChanged = new UnityEvent();

        public ThemeData CurrentTheme => currentTheme;

        public void SetTheme(ThemeData newTheme)
        {
            if (currentTheme == newTheme) return;
            
            currentTheme = newTheme;
            OnThemeChanged?.Invoke();
            
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            EditorForceRefresh();
        }

        public void EditorForceRefresh()
        {
            // In Play Mode, we use the Event system
            if (Application.isPlaying)
            {
                OnThemeChanged?.Invoke();
                return;
            }

            // In Edit Mode, we manually find everyone and force an update.
            // This is "brute force" but ensures 100% sync without memory leaks.
            CustomUIComponent[] allComponents = FindObjectsByType<CustomUIComponent>(FindObjectsSortMode.None); 
            foreach (var comp in allComponents)
            {
                comp.Init();
            }
        }
#endif
    }
}