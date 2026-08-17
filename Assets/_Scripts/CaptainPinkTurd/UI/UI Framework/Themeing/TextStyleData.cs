using CaptainPinkTurd.Core.CustomDataStructure;
using CaptainPinkTurd.Core.Extensions;
using CaptainPinkTurd.Core.Utilities;
using UnityEngine;

namespace CaptainPinkTurd.UI.Theming
{
    [CreateAssetMenu(fileName = "Text Style", 
        menuName = "Scriptable Objects/Custom UI/Theming/Styles/Text Style Sheet")]
    public class TextStyleData : ScriptableObject
    {
        [Header("Text Styles")]
        public SerializeKeyValuePair<StyleIdentifier, STypography>[] styles;
        
        public STypography Get(StyleIdentifier id)
        {
            if (id && styles.TryGetValue(id, out STypography t)) return t;
            return default;
        }
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            // When you change a Color in this asset, find the manager and tell it to update.
            // We use FindAnyObjectByType because Singletons don't always work reliably in Edit Mode.
            var manager = FindAnyObjectByType<ThemeManager>();
            if (manager)
            {
                manager.EditorForceRefresh();
            }
        }
#endif
    }
}