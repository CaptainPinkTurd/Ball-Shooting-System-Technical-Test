using UnityEngine;

namespace CaptainPinkTurd.UI.Popup
{
    [CreateAssetMenu(fileName = "Popup ID", menuName = "Scriptable Objects/PopupID")]
    public class PopupIdentifier : ScriptableObject
    {
        [Tooltip("Split second to set popup active just so Awake could be called for setup like events registration")]
        public bool initOnAwake;
    }
}