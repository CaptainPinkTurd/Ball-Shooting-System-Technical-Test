using CaptainPinkTurd.Core.Attributes;
using CaptainPinkTurd.UI.Theming;
using UnityEngine;
using UnityEngine.UI;

namespace CaptainPinkTurd.UI
{
    [CreateAssetMenu(fileName = "ThemeData", menuName = "Scriptable Objects/Custom UI/Theming/Theme Data")]
    public class ThemeData : ScriptableObject
    {
        [Header("Theme Data Properties")]
        [SerializeField] public string themeName;
        
        [Header("Theme Style Sheets")]
        [SerializeField][InlineScriptableObject] private ColorStyleData colorSheet;
        [SerializeField][InlineScriptableObject] private TextStyleData textSheet;
        [SerializeField][InlineScriptableObject] private ButtonStyleData buttonSheet;
        
        public bool TryGetColor(StyleIdentifier id, out Color color)
        {
            if (colorSheet)
            {
                color = colorSheet.Get(id);
                return true;
            }
            
            color = Color.white;
            return false;
        }
        public bool TryGetTypography(StyleIdentifier id, out STypography typography)
        {
            if (textSheet)
            {
                typography = textSheet.Get(id);
                return true;
            }
            
            typography = default;
            return false;
        }
        public bool TryGetButtonBlock(StyleIdentifier id, out ColorBlock block)
        {
            if (buttonSheet) return buttonSheet.TryGet(id, out block);
            
            block = ColorBlock.defaultColorBlock;
            return false;
        }
    }
}