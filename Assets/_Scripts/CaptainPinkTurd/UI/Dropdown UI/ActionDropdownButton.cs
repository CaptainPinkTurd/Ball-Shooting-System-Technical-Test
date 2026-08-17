using CaptainPinkTurd.AudioSystem;
using CaptainPinkTurd.Core;
using CaptainPinkTurd.Core.DesignPattern.Command;
using CaptainPinkTurd.Core.Enum;
using CaptainPinkTurd.UI.Components;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CaptainPinkTurd.UI.Dropdown
{
    public class ActionDropdownButton : ButtonBase
    {
        [Header("Action Dropdown Button Properties")]
        [SerializeField] private Image leftIcon;
        [SerializeField] private Image middleFill;
        [SerializeField] private Image rightIcon;
        [SerializeField] internal TMP_Text actionText;
        [SerializeField] private Color textSelectedColor;
        [SerializeField] private Color textOnCooldownColor;

        internal GameEvent<ICommand<EGlobalDirection>> onButtonClick;
        internal ICommand<EGlobalDirection> actionCommand;

        protected void Awake()
        {
            turnOffHoverStateWhenExit = false;
        }

        internal void SetButtonCooldown(bool isOnCooldown)
        {
            isInactive = isOnCooldown;
            actionText.color = isOnCooldown ? textOnCooldownColor : Color.white;
        }
        public override void ButtonHoverEvent(bool isHovering, bool playSfx = true)
        {
            if (isInactive) return;
            
            if(isHovering)
            {
                if (playSfx)
                {
                    SoundManager.Instance.CreateSoundBuilder()
                        .WithPosition(transform.position)
                        .WithRandomPitch().Play(hoverSfx);
                }
                
                onButtonHover.Raise(this);
            }
            
            //Instant snap cause DOFade is kinda slow
            Color middleFillColor = middleFill.color;
            middleFillColor.a = isHovering ? 1f : 0f;
            middleFill.color = middleFillColor;
            
            leftIcon.gameObject.SetActive(isHovering);
            rightIcon.gameObject.SetActive(isHovering);
            actionText.color = isHovering ? textSelectedColor : Color.white;
        }
        public override void OnButtonClickEvent()
        {
            if (isInactive) return;
            
            SoundManager.Instance.CreateSoundBuilder()
                .WithPosition(transform.position)
                .WithRandomPitch().Play(buttonClickSfx);
                        
            onButtonClick.Raise(actionCommand);
            actionCommand = null;
            onButtonClick.Clear();
        }
    }
}
