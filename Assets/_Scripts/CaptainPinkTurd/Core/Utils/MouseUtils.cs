using UnityEngine;
using UnityEngine.EventSystems;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CaptainPinkTurd.Core.Utils
{
    public static class MouseUtils
    {
        /// <summary>
        /// Returns the mouse position in world coordinates (2D).
        /// </summary>
        public static Vector3 GetMouseWorldPosition(Camera cam = null)
        {
            if (!cam) cam = Camera.main;

            Vector3 mouseWorld = cam.ScreenToWorldPoint(Input.mousePosition);
            mouseWorld.z = 0f;
            return mouseWorld;
        }

        public static bool MouseScreenCheck()
        {
#if UNITY_EDITOR
            if (Input.mousePosition.x <= 0 || Input.mousePosition.y <= 0 ||
                Input.mousePosition.x >= Handles.GetMainGameViewSize().x - 1 ||
                Input.mousePosition.y >= Handles.GetMainGameViewSize().y - 1)
            {
                return false;
            }
#else
        if (Input.mousePosition.x <= 0 || Input.mousePosition.y <= 0 || 
            Input.mousePosition.x >= Screen.width - 1 || Input.mousePosition.y >= Screen.height - 1)
        {
            return false;
        }
#endif
            else
            {
                return true;
            }
        }
        
        /// <summary>
        /// Performs a raycast from the mouse position and returns the hit information.
        /// </summary>
        public static bool RaycastFromMouse(out RaycastHit hit, LayerMask layerMask = default, Camera cam = null)
        {
            if (!cam) cam = Camera.main;

            // Create a ray from the camera to the mouse position
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            // Perform the raycast, considering the specified layer mask
            return Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask);
        }
        
        /// <summary>
        /// Checks if the pointer is currently over a UI element.
        /// </summary>
        public static bool IsPointerOverUI()
        {
            return EventSystem.current && EventSystem.current.IsPointerOverGameObject();
        }
    }
}
