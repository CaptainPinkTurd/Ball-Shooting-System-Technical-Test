using System;
using UnityEngine;

namespace CaptainPinkTurd.Core.Extensions
{
    public static class Vector3Extensions 
    {
        public static Vector3 GetInverseVector(this Vector3 vector, bool flipX = false, bool flipY = false,
            bool flipZ = false)
        {
            if(flipX) vector.x *= -1;
            if(flipY) vector.y *= -1;
            if(flipZ) vector.z *= -1;
            
            return vector;
        }
        /// <summary>
        /// Sets any x y z values of a Vector3
        /// </summary>
        public static Vector3 With(this Vector3 vector, float? x = null, float? y = null, float? z = null) 
        {
            return new Vector3(x ?? vector.x, y ?? vector.y, z ?? vector.z);
        }
        
        public static Vector3 MultiplyFloat(this Vector3 vector, 
            float multiplier, bool multiplyX = true, bool multiplyY = true, bool multiplyZ = true)
        {
            return new Vector3(multiplyX ? vector.x * multiplier : vector.x, 
                multiplyY ? vector.y * multiplier : vector.y, 
                multiplyZ ? vector.z * multiplier : vector.z);
        }
        public static Vector3 MultiplyVector(this Vector3 vector, Vector3 multiplier)
        {
            return new Vector3(vector.x * multiplier.x, vector.y * multiplier.y, vector.z * multiplier.z);
        }
        /// <summary>
        /// Adds to any x y z values of a Vector3
        /// </summary>
        public static Vector3 Add(this Vector3 vector, float x = 0, float y = 0, float z = 0) 
        {
            return new Vector3(vector.x + x, vector.y + y, vector.z + z);
        }
        
        /// <summary>
        /// Returns a Boolean indicating whether the current Vector3 is in a given range from another Vector3
        /// </summary>
        /// <param name="current">The current Vector3 position</param>
        /// <param name="target">The Vector3 position to compare against</param>
        /// <param name="range">The range value to compare against</param>
        /// <returns>True if the current Vector3 is in the given range from the target Vector3, false otherwise</returns>
        public static bool InRangeOf(this Vector3 current, Vector3 target, float range) 
        {
            return (current - target).sqrMagnitude <= range * range;
        }
        public static Quaternion ToRotationZ(this Vector3 direction, float zOffsetDegrees = 0)
        {
            if (direction.sqrMagnitude < Mathf.Epsilon)
                return Quaternion.identity;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            return Quaternion.Euler(0f, 0f, angle + zOffsetDegrees);
        }
        
        //Cardinal directions means 4-way direction
        public static Vector3 ToCardinalNormalized(this Vector3 dir)
        {
            if (dir == Vector3.zero) return Vector3.zero;
            
            dir.y = 0f;

            return Mathf.Abs(dir.x) > Mathf.Abs(dir.z) ? 
                new Vector3(Mathf.Sign(dir.x), 0f, 0f) : 
                new Vector3(0f, 0f, Mathf.Sign(dir.z));
        }
        public static Vector2 ToCardinalNormalized2D(this Vector3 dir)
        {
            if (dir == Vector3.zero) return Vector3.zero;
            
            dir.z = 0f;

            return Mathf.Abs(dir.x) > Mathf.Abs(dir.y) ? 
                new Vector2(Mathf.Sign(dir.x), 0f) : 
                new Vector2(0f, Mathf.Sign(dir.y));
        }
        
        public static Vector3 NearestPointOnAxis(this Vector3 axisDirection, Vector3 point, bool isNormalized = false)
        {
            if (!isNormalized) axisDirection.Normalize();
            float d = Vector3.Dot(point, axisDirection);
            
            return axisDirection * d;
        }
        
        public static Vector3 GetClosestVector3From(this Vector3 vector, Vector3[] otherVectors)
        {
            if (otherVectors.Length == 0) throw new Exception("The list of other vectors is empty");
            float minDistance = Vector3.Distance(vector, otherVectors[0]);
            Vector3 minVector = otherVectors[0];
            for (int i = otherVectors.Length - 1; i > 0; i--)
            {
                float newDistance = Vector3.Distance(vector, otherVectors[i]);
		    
                if (!(newDistance < minDistance)) continue;
		    
                minDistance = newDistance;
                minVector = otherVectors[i];
            }
            return minVector;
        }
    
        public static T GetClosestMonoBehaviourOfType<T>(this Vector3 vector, T[] otherMonoBehaviours) where T : MonoBehaviour
        {
            if (otherMonoBehaviours.Length == 0) throw new Exception("The list of other transforms is empty");
	    
            float minDistance = Vector3.Distance(vector, otherMonoBehaviours[0].transform.position);
            T closestMonoBehaviour = otherMonoBehaviours[0];
            for (int i = otherMonoBehaviours.Length - 1; i > 0; i--)
            {
                float newDistance = Vector3.Distance(vector, otherMonoBehaviours[i].transform.position);
		    
                if (!(newDistance < minDistance)) continue;
		    
                minDistance = newDistance;
                closestMonoBehaviour = otherMonoBehaviours[i];
            }
            return closestMonoBehaviour;
        }
        public static T GetFurthestMonoBehaviourOfType<T>(this Vector3 vector, T[] otherMonoBehaviours) where T : MonoBehaviour
        {
            if (otherMonoBehaviours.Length == 0) throw new Exception("The list of other transforms is empty");
	    
            float maxDistance = Vector3.Distance(vector, otherMonoBehaviours[0].transform.position);
            T furthestMonoBehaviour = otherMonoBehaviours[0];
            for (int i = otherMonoBehaviours.Length - 1; i > 0; i--)
            {
                float newDistance = Vector3.Distance(vector, otherMonoBehaviours[i].transform.position);
		    
                if (!(newDistance > maxDistance)) continue;
		    
                maxDistance = newDistance;
                furthestMonoBehaviour = otherMonoBehaviours[i];
            }
            return furthestMonoBehaviour;
        }
    }
}