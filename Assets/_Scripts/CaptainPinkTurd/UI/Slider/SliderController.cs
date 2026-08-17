using CaptainPinkTurd.Core.Attributes;
using CaptainPinkTurd.Core.Extensions;
using CaptainPinkTurd.UI.TextUI;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CaptainPinkTurd.UI.SliderUI
{
    public class SliderController : MonoBehaviour
    {
        [Header("Slider Animation Config")] 
        [SerializeField] private bool slideWithAnimation;
        
        [ShowIf(nameof(slideWithAnimation))]
        [SerializeField] private float slideDuration = 0.2f;
        [ShowIf(nameof(slideWithAnimation))]
        [SerializeField] private Ease slideEaseType = Ease.Linear;
        
        [Header("Slider Text Config")]
        [SerializeField] private bool hasText;
        
        [ShowIf(nameof(hasText))]
        [SerializeField] private TMP_Text sliderText;
        [ShowIf(nameof(hasText))]
        [SerializeField] private TextFormatRule textRule;
        
        public bool HasText => hasText;
        public bool IsAnimationOngoing { get; private set; } 
        public Slider Slider { get; private set; }
        
        protected virtual void Awake()
        {
            Slider = GetComponent<Slider>();
        }
        
        public void SliderUpdate(float value, float valueMin, float valueMax, bool useUnscaledTime = false, params object[] textValues)
        {
            var sliderRemapValue = value.Remap(valueMin, valueMax, 
                Slider.minValue, Slider.maxValue);

            IsAnimationOngoing = false;
            if (slideWithAnimation)
            {
                IsAnimationOngoing = true;
                Slider.DOValue(sliderRemapValue, slideDuration).SetEase(slideEaseType)
                    .OnComplete(() => IsAnimationOngoing = false).SetUpdate(useUnscaledTime);
            }
            else
            {
                Slider.value = sliderRemapValue;
            }
            
            if (hasText && textValues is { Length: > 0 }) sliderText.text = textRule.Format(textValues);
        }
    }
}