using CaptainPinkTurd.UI.Theming;
using TMPro;
using UnityEngine;

namespace CaptainPinkTurd.UI.Components
{
    public class CustomText : CustomUIComponent
    {
        private TMP_Text tmpText;
        
        protected override void Setup()
        {
            tmpText = GetComponentInChildren<TMP_Text>();
        }

        protected override void OnApplyTheme(ThemeData theme)
        {
            if (theme.TryGetTypography(styleId, out STypography textData))
            {
                tmpText.color = textData.color;
                tmpText.font = textData.fontAsset;
                tmpText.fontSize = textData.fontSize;
                tmpText.fontStyle = textData.fontStyle;
            }
            else
            {
                Debug.LogError($"No typography found for style {styleId}");
            }
        }
    }
}