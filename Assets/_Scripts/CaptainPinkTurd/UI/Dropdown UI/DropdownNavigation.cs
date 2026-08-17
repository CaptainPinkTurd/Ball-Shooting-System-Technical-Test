using System.Collections;
using System.Collections.Generic;
using CaptainPinkTurd.Core.Enum;
using CaptainPinkTurd.Input;
using CaptainPinkTurd.UI.Components;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CaptainPinkTurd.UI.Dropdown
{
    public class DropdownNavigation : MonoBehaviour //keep this class mono so we could use coroutine for scroll in here
    {
        [SerializeField] private float navigationHoldDelayTime = 0.2f;
        
        private readonly List<ButtonBase> buttons = new List<ButtonBase>();
        
        private InputSystemActions playerInputs;
        private ButtonBase currentButton;
        private int currentIndex = 0;
        private bool isNavigating = false;

        private void Awake()
        {
            playerInputs = new InputSystemActions();
        }
        
        private void OnDisable()
        {
            DisableInput();
        }

        public void ReverseButtonsList()
        {
            buttons.Reverse();
            currentIndex = buttons.Count - currentIndex - 1;
        }
        
        internal void AddNewButton(ButtonBase button)
        {
            buttons.Add(button);
            button.onButtonHover.Subscribe(OnExternalNavigationChange);
        }

        private void RemoveOldButtons()
        {
            foreach(var button in buttons)
            {
                button.ButtonHoverEvent(false);
                button.onButtonHover.Unsubscribe(OnExternalNavigationChange);
            }
            buttons.Clear();
            currentIndex = 0;
        }
        public void EnableInput()
        {
            RemoveOldButtons();
            
            playerInputs.Enable();
            playerInputs.Player.Move.started += OnNavigationStarted;
            playerInputs.Player.Move.canceled += OnNavigationCanceled;
            
            // spaceBarAction.Enable();
            // spaceBarAction.performed += SpaceBarActionOnPerformed;
        }

        public void DisableInput()
        {
            currentIndex = 0;
            
            playerInputs.Player.Move.started -= OnNavigationStarted;
            playerInputs.Player.Move.canceled -= OnNavigationCanceled;
            playerInputs.Disable();
            
            // spaceBarAction.performed -= SpaceBarActionOnPerformed;
            // spaceBarAction.Disable();
        }

        internal void SetCurrentNavigation(EInputDirection inputDirection)
        {
            if(currentButton) currentButton.ButtonHoverEvent(false);

            switch (inputDirection)
            {
                case EInputDirection.Up:
                    do
                    {
                        currentIndex++;
                        if (currentIndex >= buttons.Count) currentIndex = 0;
                    } while (buttons[currentIndex].isInactive);
                    break;
                case EInputDirection.Down:
                    do
                    {
                        currentIndex--;
                        if (currentIndex < 0) currentIndex = buttons.Count - 1;
                    } while (buttons[currentIndex].isInactive);
                    break;
            }
            
            currentButton = buttons[currentIndex];
            currentButton.ButtonHoverEvent(true);
        }

        public void InitialNavigationSetup()
        {
            while (buttons[currentIndex].isInactive)
            {
                currentIndex++;
                if (currentIndex >= buttons.Count) currentIndex = 0;
            }
            
            currentButton = buttons[currentIndex];
            currentButton.ButtonHoverEvent(true, false);
        }
        private void OnNavigationStarted(InputAction.CallbackContext ctx)
        {
            if (isNavigating) return;
            
            isNavigating = true;
            Vector2 input = ctx.ReadValue<Vector2>();
            switch (input.y)
            {
                case > 0:
                    StartCoroutine(OnNavigationHold(EInputDirection.Up));
                    break;
                case < 0:
                    StartCoroutine(OnNavigationHold(EInputDirection.Down));
                    break;
            }
        }
        private void OnNavigationCanceled(InputAction.CallbackContext ctx)
        {
            isNavigating = false;
        }

        private IEnumerator OnNavigationHold(EInputDirection inputDirection)
        {
            float timer = 0f;

            while (isNavigating)
            {
                if (timer <= 0f)
                {
                    SetCurrentNavigation(inputDirection);
                    timer = navigationHoldDelayTime;
                }

                timer -= Time.deltaTime;
                yield return null; 
                //check every frame to ensure the while loop will break instantly if the condition turns to false in a split second
            }
        }
        private void SpaceBarActionOnPerformed(InputAction.CallbackContext _)
        {
            buttons[currentIndex].OnButtonClickEvent();
        }

        private void OnExternalNavigationChange(ButtonBase newButton)
        {
            if (!currentButton || currentButton == newButton) return;
            
            currentButton.ButtonHoverEvent(false);
            currentButton = newButton; 
            currentIndex = buttons.FindIndex(button => button == newButton);
        }
    }
}