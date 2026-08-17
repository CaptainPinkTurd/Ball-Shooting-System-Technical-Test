using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace CaptainPinkTurd.UI.Components
{
    public class CustomButton : CustomUIComponent
    {
        [SerializeField] internal UnityEvent onClick;

        private Button button;
        private CustomText buttonText;

        protected override void Setup()
        {
            button = GetComponentInChildren<Button>();
            buttonText = GetComponentInChildren<CustomText>();
        }

        protected override void OnApplyTheme(ThemeData theme) 
        {
            if (theme.TryGetButtonBlock(styleId, out ColorBlock cb))
            {
                button.colors = cb;
            }
            else
            {
                Debug.LogError($"No button color block found for style {styleId}");
            }

            if (didAwake) return;
            
            buttonText.Init();
        }

        public void OnClick() 
        {
            onClick.Invoke();
        }
    }
}
