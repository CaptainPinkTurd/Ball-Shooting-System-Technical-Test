using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CaptainPinkTurd.UI
{
    public class HoverFeedback : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private MMF_Player hoverFeedback;
        [SerializeField] private MMF_Player exitFeedback;

        public void OnPointerEnter(PointerEventData eventData)
        {
            hoverFeedback.PlayFeedbacks();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            exitFeedback.PlayFeedbacks();
        }
    }
}