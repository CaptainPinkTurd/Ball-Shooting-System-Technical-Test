using UnityEngine;

namespace CaptainPinkTurd.Core.Extensions
{
    public static class CameraExtensions
    {
        public static Vector3 ScreenToWorld2D(this Camera camera, Vector3 position)
        {
            position.z = camera.nearClipPlane;
            return camera.ScreenToWorldPoint(position);
        }
        public static bool IsWorldPointInView(this Camera camera, Vector3 worldPosition)
        {
            Vector3 viewportPoint = camera.WorldToViewportPoint(worldPosition);

            return viewportPoint.x is >= 0f and <= 1f &&
                   viewportPoint.y is >= 0f and <= 1f;
        }
        public static bool IsRegionInView(this Camera camera, Vector3 center, Vector2 regionSize)
        {
            // Create a 3D bounds from a 2D region
            Bounds bounds = new Bounds(
                center,
                new Vector3(regionSize.x, regionSize.y, 0.01f)
            );

            Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
            return GeometryUtility.TestPlanesAABB(planes, bounds);
        }
        
        /// <summary>
        /// Calculates the world-space bounds of an orthographic camera's visible area.
        /// </summary>
        /// <param name="cam">
        /// The orthographic camera whose visible bounds should be calculated.
        /// </param>
        /// <returns>
        /// A <see cref="Bounds"/> representing the rectangular area currently visible
        /// to the camera in world space.
        /// </returns>
        public static Bounds GetOrthographicCameraBounds(this Camera cam)
        {
            float height = 2f * cam.orthographicSize;
            float width = height * cam.aspect;

            return new Bounds(
                cam.transform.position,
                new Vector3(width, height, 0f)
            );
        }
    }
}