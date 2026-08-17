using UnityEngine;

namespace CaptainPinkTurd.Core.Extensions
{
    public static class FloatExtensions
    {
        /// <summary>
        /// Maps a value from one range to another.
        /// </summary>
        /// <param name="value">The value to map.</param>
        /// <param name="from1">Lower bound of the original range.</param>
        /// <param name="to1">Upper bound of the original range.</param>
        /// <param name="from2">Lower bound of the target range.</param>
        /// <param name="to2">Upper bound of the target range.</param>
        /// <returns>The value remapped to the target range.</returns>
        public static float Remap(this float value, float from1, float to1, float from2, float to2) 
        {
            float t = Mathf.InverseLerp(from1, to1, value);
            return Mathf.Lerp(from2, to2, t);
        }
    }
}
