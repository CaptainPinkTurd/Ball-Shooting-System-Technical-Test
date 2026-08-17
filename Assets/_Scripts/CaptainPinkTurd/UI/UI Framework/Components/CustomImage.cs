using UnityEngine;
using UnityEngine.UI;

namespace CaptainPinkTurd.UI.Components
{
    [RequireComponent(typeof(Image))]
    public class CustomImage : CustomUIComponent
    {
        private Image image;
        protected override void Setup()
        {
            image = GetComponent<Image>();
        }
        protected override void OnApplyTheme(ThemeData theme) 
        {
            if (theme.TryGetColor(styleId, out Color color))
            {
                image.color = color;
            }
            else
            {
                Debug.LogError($"No color found for style {styleId}");
            }
        }
    }
}