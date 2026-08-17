using UnityEngine;

namespace CaptainPinkTurd.Core.Extensions
{
    public static class GradientExtensions
    {
        public static GradientColorKey GetNearestColorKey(this Gradient gradient, float targetTime)
        {
            GradientColorKey[] keys = gradient.colorKeys;
            GradientColorKey nearest = keys[0];
            float minDiff = Mathf.Abs(targetTime - keys[0].time);

            foreach (var key in keys) 
            {
                float diff = Mathf.Abs(targetTime - key.time);
                
                if (!(diff < minDiff)) continue;
                
                minDiff = diff;
                nearest = key;
            }
            return nearest;
        }
    }
}