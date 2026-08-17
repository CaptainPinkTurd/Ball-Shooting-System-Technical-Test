using System.Collections;
using CaptainPinkTurd.Core.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace CaptainPinkTurd.UI
{
    public class HotkeyButton : Selectable, IPointerClickHandler, ISubmitHandler
    {
        [Header("Input Conditionals")] 
        [SerializeField] private InputActionReference assignedHotkeyButton;
        [SerializeField] private bool isDynamicButton;

        [Header("Visuals Setup")] 
        [SerializeField] private TMP_Text hotkeyLabel;
        [SerializeField] private float longLabelFontSize = 1;
        
        [Header("Click Events")] 
        public UnityEvent OnClick;
        
        //IsClicked should be manually toggled to true in the method subscribed to OnClick to check
        //whether functionality does get triggered by the button or not
        public bool IsClicked { get; set; } 
        
        private Coroutine _resetRoutine;
        private InputDevice _lastDevice;
        private WaitForSeconds _waitTimeFadeDuration;

        private float _initialLabelFontSize = 1;
        
        #if UNITY_EDITOR
        protected override void Reset()
        {
            base.Reset();
            
            var imageComponent = GetComponent<Image>();
            if (!imageComponent)
                imageComponent = gameObject.AddComponent<Image>();

            targetGraphic = imageComponent;
            
            hotkeyLabel = GetComponentInChildren<TMP_Text>();
        }
        #endif

        public void AssignInputActionReference(InputActionReference reference)
        {
            assignedHotkeyButton = reference;
            SetLabelText(_lastDevice);
            
            if(!gameObject.activeSelf) gameObject.SetActive(true);
        }

        public void UnassignInputActionReference()
        {
            gameObject.SetActive(false);
            assignedHotkeyButton = null;
            OnClick.RemoveAllListeners();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            StartCoroutine(CoroutineUtils.WaitForCondition(() => assignedHotkeyButton,
                () => assignedHotkeyButton.action.performed += HotkeyClicked));
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            
            if(assignedHotkeyButton) assignedHotkeyButton.action.performed -= HotkeyClicked;
        }

        protected override void Start()
        {
            base.Start();
            
            _waitTimeFadeDuration = new WaitForSeconds(colors.fadeDuration);

            //InputManager.Instance.OnUpdatedInputDevice.Subscribe(SetLabelText);
            SetInitialDevice();
            SetLabelText(_lastDevice);
        }
        
        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            if(assignedHotkeyButton) assignedHotkeyButton.action.performed -= HotkeyClicked;
            OnClick.RemoveAllListeners();
            
            if (!gameObject.scene.isLoaded) return;
            
            //InputManager.Instance.OnUpdatedInputDevice.Unsubscribe(SetLabelText);
        }
        
        private void SetInitialDevice()
        {
            if (_lastDevice != null) return;

            if (Gamepad.current != null)
                _lastDevice = Gamepad.current;
            else if (Keyboard.current != null)
                _lastDevice = Keyboard.current;
            else if (Mouse.current != null)
                _lastDevice = Mouse.current;
        }

        #region Labeling

        public void SetLabelText(InputDevice inputDevice)
        {
            _lastDevice = inputDevice;
            hotkeyLabel.SetText(GetAssignedButton());
        }
        
        private string GetAssignedButton()
        {
            if (assignedHotkeyButton && assignedHotkeyButton.action != null)
            {
                var action = assignedHotkeyButton.action;
                foreach (var binding in action.bindings)
                {
                    if (binding.isPartOfComposite || binding.isComposite) 
                        continue;
                    
                    if (IsCurrentDeviceBinding(binding))
                    {
                        return InputControlPath.ToHumanReadableString(
                            binding.effectivePath,
                            InputControlPath.HumanReadableStringOptions.OmitDevice);
                    }
                }
            }
            
            return "Not found.";
        }
        
        private bool IsCurrentDeviceBinding(InputBinding binding)
        {
            if (_lastDevice == null)
                return string.IsNullOrEmpty(binding.groups);

            if (binding.groups.Contains("Gamepad") && _lastDevice is Gamepad)
            {
                hotkeyLabel.fontSize = longLabelFontSize;
                return true;
            }

            if (binding.groups.Contains("Keyboard") && (_lastDevice is Keyboard || _lastDevice is Mouse))
            {
                hotkeyLabel.fontSize = _initialLabelFontSize;
                return true;
            }

            return string.IsNullOrEmpty(binding.groups);
        }
        #endregion

        #region ClickFunctionality
        private void Clicked()
        {
            OnClick?.Invoke();
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            Clicked();
        }
        
        public void OnSubmit(BaseEventData eventData)
        {
            DoStateTransition(SelectionState.Pressed, true);
            
            Clicked();
            
            if (_resetRoutine != null)
                StopCoroutine(OnFinishSubmit());
            
            _resetRoutine = StartCoroutine(OnFinishSubmit());
        }

        private void HotkeyClicked(InputAction.CallbackContext obj)
        {
            //if(GameManager.Instance.currentGameState != EGameState.PlayerTurn) return;
            
            DoStateTransition(SelectionState.Pressed, true);
            
            Clicked();

            //just in case button disappear instantly after pressed
            if (!gameObject.activeInHierarchy || !gameObject.activeSelf)
            {
                OnFinishSubmitEvent();
                return; 
            }
            
            if (_resetRoutine != null)
                StopCoroutine(OnFinishSubmit());
           
            _resetRoutine = StartCoroutine(OnFinishSubmit());
        }
        
        private IEnumerator OnFinishSubmit()
        {
            yield return _waitTimeFadeDuration;

            OnFinishSubmitEvent();
        }
        private void OnFinishSubmitEvent()
        {
            DoStateTransition(currentSelectionState, false);
            
            //buttons may have been registered as clicked but have not raised any actual OnClick event will not have IsClicked set to true
            if (!isDynamicButton || !IsClicked) return;
            
            UnassignInputActionReference(); 
            IsClicked = false;
        }
        #endregion
    }
}