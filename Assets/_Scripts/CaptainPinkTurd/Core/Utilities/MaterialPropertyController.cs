using UnityEngine;
using UnityEngine.UI;

namespace CaptainPinkTurd.Core.Utilities
{
    /// <summary>
    /// Creates unique material instances for GameObjects and provides methods to modify material properties at runtime.
    /// </summary>
    public class MaterialPropertyController : MonoBehaviour
    {
        private Material uniqueMaterial;
        private Renderer targetRenderer;
        private Graphic targetGraphic;
        
        /// Check if this is controlling a UI element
        /// </summary>
        public bool IsUIElement => targetGraphic;
        
        /// <summary>
        /// Check if this is controlling a world space renderer
        /// </summary>
        public bool IsWorldRenderer => targetRenderer;
        
        void Awake()
        {
            // Try to get Renderer first (SpriteRenderer, MeshRenderer, etc.)
            targetRenderer = GetComponent<Renderer>();
            
            if (targetRenderer)
            {
                // This automatically creates a unique instance for this GameObject
                uniqueMaterial = targetRenderer.material;
            }
            else
            {
                // Try to get Graphic component (Image, RawImage, etc.)
                targetGraphic = GetComponent<Graphic>();
                
                if (targetGraphic)
                {
                    // This automatically creates a unique instance for this UI element
                    uniqueMaterial = new Material(targetGraphic.material);
                    targetGraphic.material = uniqueMaterial;
                }
                else
                {
                    Debug.LogError($"No Renderer or Graphic component found on {gameObject.name}");
                }
            }
        }
        
        /// <summary>
        /// Set a color property on the material
        /// </summary>
        public void SetColorProperty(string propertyName, Color color)
        {
            if (uniqueMaterial)
            {
                uniqueMaterial.SetColor(propertyName, color);
            }
        }
        
        /// <summary>
        /// Set a float property on the material
        /// </summary>
        public void SetFloatProperty(string propertyName, float value)
        {
            if (uniqueMaterial)
            {
                uniqueMaterial.SetFloat(propertyName, value);
            }
        }
        
        /// <summary>
        /// Set a texture property on the material
        /// </summary>
        public void SetTextureProperty(string propertyName, Texture texture)
        {
            if (uniqueMaterial)
            {
                uniqueMaterial.SetTexture(propertyName, texture);
            }
        }
        
        /// <summary>
        /// Get a color property from the material
        /// </summary>
        public Color GetColorProperty(string propertyName)
        {
            return uniqueMaterial ? uniqueMaterial.GetColor(propertyName) : Color.white;
        }
        
        /// <summary>
        /// Get a float property from the material
        /// </summary>
        public float GetFloatProperty(string propertyName)
        {
            return uniqueMaterial ? uniqueMaterial.GetFloat(propertyName) : 0f;
        }
        
        void OnDestroy()
        {
            // Clean up the automatically created instance
            if (uniqueMaterial)
            {
                DestroyImmediate(uniqueMaterial);
            }
        }
        
        /// <summary>
        /// Get the unique material instance (use with caution)
        /// </summary>
        public Material GetMaterialInstance()
        {
            return uniqueMaterial;
        }
    }
}