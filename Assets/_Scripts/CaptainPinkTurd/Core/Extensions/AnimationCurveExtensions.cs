using UnityEngine;

namespace CaptainPinkTurd.Core.Extensions
{
    public static class AnimationCurveExtensions
    {
		/// <summary>
        /// Calculates the area under an AnimationCurve using the Trapezoidal Rule.
        /// </summary>
        /// <param name="curve">The target AnimationCurve.</param>
        /// <param name="samples">Higher numbers increase accuracy but take more CPU time.</param>
        public static float CalculateArea(this AnimationCurve curve, int samples = 500)
        {
            if (curve == null || curve.length == 0) return 0f;
            
            // Get the start and end times of the curve
            float startTime = curve.keys[0].time;
            float endTime = curve.keys[curve.length - 1].time;
            float totalTime = endTime - startTime;
            
            if (totalTime <= 0f) return 0f;
            
            float step = totalTime / samples;
            float area = 0f;
            
            // Cache the initial evaluation
            float prevValue = curve.Evaluate(startTime);
            for (int i = 1; i <= samples; i++)
            {
                float currentTime = startTime + (i * step);
                float currentValue = curve.Evaluate(currentTime);
                
                // Calculate the area of the trapezoid for this slice
                float sliceArea = (prevValue + currentValue) * 0.5f * step;
                area += sliceArea;
                
                // Move to the next step
                prevValue = currentValue;
            }
            return area;
        }
    }
}