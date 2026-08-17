using CaptainPinkTurd.UI.Popup;
using UnityEngine;

namespace CaptainPinkTurd.UnitSystem
{
    [CreateAssetMenu(fileName = "UnitSharedUiSettings", menuName = "Scriptable Objects/Unit Info/Shared UI Settings")]
    public class UnitSharedUiSettings : ScriptableObject
    {
        [SerializeField] internal Color unitOutlineColor;
        [SerializeField] internal Color unitOutlineDefaultColor;
        [SerializeField] internal PopupText fatiguePopupText;
        [SerializeField] internal float popupTextSpawnOffset;
    }
}
