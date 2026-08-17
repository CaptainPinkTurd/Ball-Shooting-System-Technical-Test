using CaptainPinkTurd.Core.CustomDataStructure;
using CaptainPinkTurd.Core.Extensions;
using CaptainPinkTurd.Core.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace CaptainPinkTurd.UI.Theming
{
    [CreateAssetMenu(fileName = "Button Style", 
        menuName = "Scriptable Objects/Custom UI/Theming/Styles/Button Style Sheet")]
    public class ButtonStyleData : ScriptableObject
    {
        [Header("Button Styles")] 
        public SerializeKeyValuePair<StyleIdentifier, ColorBlock>[] styles;

        public bool TryGet(StyleIdentifier id, out ColorBlock block)
        {
            if (id && styles.TryGetValue(id, out ColorBlock b))
            {
                block = b;
                return true;
            }

            block = ColorBlock.defaultColorBlock;
            return false;
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