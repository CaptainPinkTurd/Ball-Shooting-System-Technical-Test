using CaptainPinkTurd.Core.CustomDataStructure;
using UnityEngine.InputSystem;

namespace CaptainPinkTurd.Core.Utilities
{
    [System.Serializable]
    public class InputActionHandlerWithDeviceChange<T>
    {
        public SerializeKeyValuePair<T, InputActionReference>[] inputBindingReference;
        
        public void OnAwake()
        {
            foreach (var input in inputBindingReference)
            {
                input.Value?.action.Enable();
            }
        }

        public void OnDestroy()
        {
            foreach (var input in inputBindingReference)
            {
                input.Value?.action.Disable();
            }
        }
    }
}