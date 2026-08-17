using UnityEngine;

namespace CaptainPinkTurd.Core.Extensions
{
    public static class LayerMaskExtensions
    {
        /// <summary>
        /// Returns true if the LayerMask contains the given layer index.
        /// Example:
        ///     if (mask.Contains(LayerMask.NameToLayer("Enemy"))) { ... }
        /// </summary>
        public static bool Contains(this LayerMask mask, int layer)
        {
            return (mask.value & (1 << layer)) != 0;
        }
        
        /// <summary>
        /// Adds a layer index to the LayerMask.
        /// Example:
        ///     mask = mask.AddLayer(LayerMask.NameToLayer("Enemy"));
        /// Note: adding the same layer twice won't change anything
        /// | = OR
        /// If either bit is 1, then the result is 1.
        /// </summary>
        public static LayerMask AddLayer(this LayerMask mask, int layer)
        {
            mask.value |= (1 << layer);
            return mask;
        }
        
        /// <summary>
        /// Removes a layer index from the LayerMask.
        /// Example Usage:
        ///     mask = mask.RemoveLayer(LayerMask.NameToLayer("Enemy"));
        /// Visual Example: 
        /// mask:        0101 0000 //if layer value is 0001 0000
        /// remove:      1110 1111 //then the not version of that layer will be the result of this remove mask
        /// ----------------------
        /// result:      0100 0000
        /// </summary>
        public static LayerMask RemoveLayer(this LayerMask mask, int layer)
        {
            mask.value &= ~(1 << layer);
            return mask;
        }
        
        /// <summary>
        /// Adds an entire LayerMask to this LayerMask.
        /// </summary>
        public static LayerMask AddMask(this LayerMask mask, LayerMask maskToAdd)
        {
            mask.value |= maskToAdd.value;
            return mask;
        }

        /// <summary>
        /// Removes an entire LayerMask from this LayerMask.
        /// </summary>
        public static LayerMask RemoveMask(this LayerMask mask, LayerMask maskToRemove)
        {
            mask.value &= ~maskToRemove.value;
            return mask;
        }
        
        /// <summary>
        /// Toggles the layer index in the mask.
        /// Example:
        ///     mask = mask.ToggleLayer(LayerMask.NameToLayer("Walls"));
        /// </summary>
        public static LayerMask ToggleLayer(this LayerMask mask, int layer)
        {
            mask.value ^= (1 << layer);
            return mask;
        }
        
        /// <summary>
        /// Ensures the mask contains only this layer.
        /// Example:
        ///     mask = mask.Only(LayerMask.NameToLayer("Player"));
        /// </summary>
        public static LayerMask Only(this LayerMask mask, int layer)
        {
            mask.value = (1 << layer);
            return mask;
        }
    }
}