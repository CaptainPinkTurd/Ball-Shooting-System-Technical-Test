using CaptainPinkTurd.Core.Enum;
using UnityEngine;

namespace CaptainPinkTurd.UI
{
    public class PopupButtonHandler : MonoBehaviour
    {
        public EPopupShowBehaviour popupType; // Enum selector in Inspector
        //public EPopupTypeEvent onPopupSelected; // Event with enum payload

        public void OnButtonClick()
        {
            //if (player.ActionQueueCount == 0 && player.IsOnCooldown(EActionType.Wait)) return;
            //onPopupSelected?.Invoke(popupType);
        }
    }
}
