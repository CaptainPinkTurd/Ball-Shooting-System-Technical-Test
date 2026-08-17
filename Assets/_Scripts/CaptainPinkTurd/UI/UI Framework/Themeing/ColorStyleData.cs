using CaptainPinkTurd.Core.CustomDataStructure;
using CaptainPinkTurd.Core.Extensions;
using CaptainPinkTurd.Core.Utilities;
using UnityEngine;

namespace CaptainPinkTurd.UI.Theming
{
    [CreateAssetMenu(fileName = "Color Style", 
            menuName = "Scriptable Objects/Custom UI/Theming/Styles/Color Style Sheet")]
    public class ColorStyleData : ScriptableObject
    {
        [Header("Color Palette")]
        public SerializeKeyValuePair<StyleIdentifier, Color>[] styles;

        public Color Get(StyleIdentifier id)
        {
            if (id && styles.TryGetValue(id, out Color c)) return c;
            return Color.magenta; // Debug color for missing style
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