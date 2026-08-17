using CaptainPinkTurd.AudioSystem;
using CaptainPinkTurd.Core;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CaptainPinkTurd.UI.Components
{
    public abstract class ButtonBase : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [Header("Button Dropdown Base Properties")]
        [SerializeField] protected SoundData hoverSfx;
        [SerializeField] protected SoundData buttonClickSfx;
        
        internal GameEvent<ButtonBase> onButtonHover = new GameEvent<ButtonBase>();
        internal bool isInactive;

        protected bool turnOffHoverStateWhenExit = true;
        
        private void OnEnable()
        {
            ButtonHoverEvent(false);
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            ButtonHoverEvent(true);
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            if (!turnOffHoverStateWhenExit) return;
            
            ButtonHoverEvent(false);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnButtonClickEvent();
        }
        public abstract void OnButtonClickEvent();
        public abstract void ButtonHoverEvent(bool isHovering, bool playSfx = true);
    }
}