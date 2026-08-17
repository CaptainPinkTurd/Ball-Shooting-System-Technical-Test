using CaptainPinkTurd.Core.Attributes;
using CaptainPinkTurd.Core.Enum;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace CaptainPinkTurd.UI.Popup
{
    public class PopupActivator : MonoBehaviour
    {
        [Header("Popup Activator Properties")]
        [SerializeField] private PopupIdentifier popupId;
        [SerializeField] private EPopupShowBehaviour behaviour;
        [SerializeField] private bool showCursorOnActive = true;
        [SerializeField] private bool hideCursorOnInactive = true;
        
        [Header("Input Conditionals")]
        [SerializeField] private bool useInputShortcut;
        [ShowIf(nameof(useInputShortcut))]
        [SerializeField] private InputActionReference inputAction;
        
        [Header("Popup Events")]
        [SerializeField] private UnityEvent onPopupShow;
        [SerializeField] private UnityEvent onPopupClose;
        
        private PopupManager popupManager;
        private bool isPopupActive;

        private void Awake()
        {
            popupManager = GetComponentInParent<PopupManager>();
        }
        private void OnEnable()
        {
            if (!useInputShortcut || !inputAction) return;
            
            inputAction.action.Enable();
            inputAction.action.performed += PopupTrigger;
        }

        private void OnDisable()
        {
            if (!useInputShortcut || !inputAction) return;

            inputAction.action.performed -= PopupTrigger;
            inputAction.action.Disable();
        }

        public void ShowPopup()
        {
            if (!popupManager.CanTriggerPopup) return;
            if (showCursorOnActive) Cursor.visible = true;
            
            isPopupActive = true;
            popupManager.ShowPopup(popupId, behaviour);
            onPopupShow.Invoke();
        }

        public void ClosePopup()
        {
            if(!popupManager.CanTriggerPopup) return;
            Cursor.visible = !hideCursorOnInactive;
            
            isPopupActive = false;
            popupManager.CloseLastPopup();
            onPopupClose.Invoke();
        }
        
        private void PopupTrigger(InputAction.CallbackContext obj)
        {
            if (isPopupActive)
            {
                ClosePopup();
            }
            else
            {
                ShowPopup();
            }
        }
    }
}