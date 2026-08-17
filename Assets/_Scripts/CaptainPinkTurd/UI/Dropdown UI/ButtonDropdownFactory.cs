using CaptainPinkTurd.Core;
using CaptainPinkTurd.Core.DesignPattern.Command;
using CaptainPinkTurd.Core.Enum;
using CaptainPinkTurd.Core.Extensions;
using CaptainPinkTurd.Core.Utilities;
using CaptainPinkTurd.UI.Dropdown;
using UnityEngine;

namespace CaptainPinkTurd.UI.CombatUI
{
    public class ButtonDropdownFactory : MonoBehaviour
    {
        [SerializeField] private ActionDropdownButton actionDropdownButtonPrefab;
        [SerializeField] public DropdownNavigation dropdownNavigation;
        
        private GameEvent onActionChose = new GameEvent();
        private RectTransform rectTransform;
        private Vector3 originalScale;
        private Vector3 inverseVerticalScale;
        
        private bool isTopToBottom = true;
        private float dropdownYPosition;
        
        public bool IsTopToBottom => isTopToBottom;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            originalScale = rectTransform.localScale;
            inverseVerticalScale = new Vector3(originalScale.x, -originalScale.y, originalScale.z);
            dropdownYPosition = rectTransform.anchoredPosition.y;
        }

        public void ActivateButtonDropdown(string actionName, ICommand<EGlobalDirection> actionCommand,
            GameEvent<ICommand<EGlobalDirection>> onButtonClick, int siblingIndex, bool isOnCooldown)
        {
            ActionDropdownButton actionDropdownButton = ObjectPoolManager.Instance.SpawnObject(
                actionDropdownButtonPrefab.gameObject, gameObject.transform).GetComponent<ActionDropdownButton>();
            actionDropdownButton.SetButtonCooldown(isOnCooldown);
            
            actionDropdownButton.transform.SetSiblingIndex(siblingIndex);
            actionDropdownButton.actionText.text = actionName;
            actionDropdownButton.actionCommand = actionCommand;
            actionDropdownButton.onButtonClick = new GameEvent<ICommand<EGlobalDirection>>(onButtonClick);
            dropdownNavigation.AddNewButton(actionDropdownButton);

            RectTransform dropdownRectTransform = actionDropdownButton.GetComponent<RectTransform>();
            float currentYScale = dropdownRectTransform.localScale.y;
            bool needsInversion = (isTopToBottom && currentYScale < 0) || (!isTopToBottom && currentYScale > 0);
    
            if (needsInversion)
            {
                dropdownRectTransform.localScale = dropdownRectTransform.localScale.GetInverseVector(false, true, false);
            }
            

            onActionChose.Subscribe(() =>
            {
                ObjectPoolManager.Instance.ReturnObjectToPool(actionDropdownButton.gameObject);
            });
        }

        public void DeactivateDropdown()
        {
            dropdownNavigation.DisableInput();
            onActionChose.Raise();
            onActionChose.Clear();
        }

        public void SetLayoutGroupPositionDirection(bool buttonIsSouthSide, bool isFlatEdgeOfMap, bool unitInSouthSide)
        {
            if ((!buttonIsSouthSide && !isFlatEdgeOfMap) || (unitInSouthSide && isFlatEdgeOfMap))
            {
                rectTransform.localScale = inverseVerticalScale;
                rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, -dropdownYPosition);
                isTopToBottom = false;
            }
            else
            {
                rectTransform.localScale = originalScale;
                rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, dropdownYPosition);
                isTopToBottom = true;
            }
        }
    }
}
