using System.Collections.Generic;
using UnityEngine;

namespace CaptainPinkTurd.Core.Extensions
{
	public static class TransformExtensions
    {
	    #region Position Related Methods

	    public static Vector3 ChangeXPos (this Transform transform, float x) 
	    {
		    Vector3 position = transform.position;
		    position.x = x;
		    transform.position = position;
		    return position;
	    }
	    public static Vector3 ChangeYPos (this Transform transform, float y) 
	    {
		    Vector3 position = transform.position;
		    position.y = y;
		    transform.position = position;
		    return position;
	    }
	    public static Vector3 ChangeZPos (this Transform transform, float z) 
	    {
		    Vector3 position = transform.position;
		    position.z = z;
		    transform.position = position;
		    return position;
	    }
	    public static bool IsRightSideOfTransform(this Transform transform, Vector3 targetPosition) 
		    => transform.position.x <= targetPosition.x;
	    public static bool IsOnTopOfTransform(this Transform transform, Vector3 targetPosition) 
		    => transform.position.y <= targetPosition.y;

	    #endregion

        #region Rotate Related Methods

        /// <summary>
        /// Rotates a transform to face a target position (2D).
        /// </summary>
        public static void LookAt2D(this Transform transform, Vector3 targetPosition, float lookSpeed, bool snap = false)
        {
            Vector3 dir = targetPosition - transform.position;
            Quaternion targetRotation = dir.ToRotationZ();

            transform.rotation = snap ? targetRotation :
                Quaternion.RotateTowards(transform.rotation, targetRotation,
	                lookSpeed * Time.deltaTime); 
        }
        public static void SmoothLookAt(this Transform t, Transform targetTransform, float rotationSpeed, float delta)
        {
	        Quaternion rot = t.GetLookAtRotation(targetTransform);

	        t.rotation =  Quaternion.RotateTowards(t.rotation, rot, rotationSpeed * delta );
        }
	
        public static void SmoothLookAt(this Transform t, Vector3 targetPosition, float rotationSpeed, float delta)
        {
	        Quaternion rot = t.GetLookAtRotation(targetPosition);

	        t.rotation =  Quaternion.RotateTowards(t.rotation, rot, rotationSpeed * delta );
        }
        
        /// <summary>
		/// Find the rotation to look at a Vector3
		/// </summary>
		/// <param name="self"></param>
		/// <param name="target">The thing to look at</param>
		/// <returns></returns>
		public static Quaternion GetLookAtRotation(this Transform self, Vector3 target)
		{
			return Quaternion.LookRotation(target - self.position);
		}
	
		/// <summary>
		/// Find the rotation to look at a Transform
		/// </summary>
		/// <param name="self"></param>
		/// <param name="target">The thing to look at</param>
		/// <returns></returns>
		public static Quaternion GetLookAtRotation(this Transform self, Transform target)
		{
			return GetLookAtRotation(self, target.position);
		}
	
		/// <summary>
		/// Find the rotation to look at a GameObject
		/// </summary>
		/// <param name="self"></param>
		/// <param name="target">The thing to look at</param>
		/// <returns></returns>
		public static Quaternion GetLookAtRotation(this Transform self, GameObject target)
		{
			return GetLookAtRotation(self, target.transform.position);
		}
	
		/// <summary>
		/// Instantly look away from a target Vector3
		/// </summary>
		/// <param name="self"></param>
		/// <param name="target">The thing to look away from</param>
		public static void LookAwayFrom(this Transform self, Vector3 target)
		{
			self.rotation = GetLookAwayFromRotation(self, target);
		}
	
		/// <summary>
		/// Instantly look away from a target transform
		/// </summary>
		/// <param name="self"></param>
		/// <param name="target">The thing to look away from</param>
		public static void LookAwayFrom(this Transform self, Transform target)
		{
			self.rotation = GetLookAwayFromRotation(self, target);
		}
	
		/// <summary>
		/// Instantly look away from a target GameObject
		/// </summary>
		/// <param name="self"></param>
		/// <param name="target">The thing to look away from</param>
		public static void LookAwayFrom(this Transform self, GameObject target)
		{
			self.rotation = GetLookAwayFromRotation(self, target);
		}
	
	
		/// <summary>
		/// Find the rotation to look away from a target Vector3
		/// </summary>
		/// <param name="self"></param>
		/// <param name="target">The thing to look away from</param>
		public static Quaternion GetLookAwayFromRotation(this Transform self, Vector3 target)
		{
			return Quaternion.LookRotation(self.position - target);
		}
	
		/// <summary>
		/// Find the rotation to look away from a target transform
		/// </summary>
		/// <param name="self"></param>
		/// <param name="target">The thing to look away from</param>
		public static Quaternion GetLookAwayFromRotation(this Transform self, Transform target)
		{
			return GetLookAwayFromRotation(self, target.position);
		}
	
		/// <summary>
		/// Find the rotation to look away from a target GameObject
		/// </summary>
		/// <param name="self"></param>
		/// <param name="target">The thing to look away from</param>
		public static Quaternion GetLookAwayFromRotation(this Transform self, GameObject target)
		{
			return GetLookAwayFromRotation(self, target.transform.position);
		}

        #endregion

        #region Utility Methods

        /// <summary>
        /// Retrieves all the children of a given Transform.
        /// </summary>
        /// <remarks>
        /// This method can be used with LINQ to perform operations on all child Transforms. For example,
        /// you could use it to find all children with a specific tag, to disable all children, etc.
        /// Transform implements IEnumerable and the GetEnumerator method which returns an IEnumerator of all its children.
        /// </remarks>
        /// <param name="parent">The Transform to retrieve children from.</param>
        /// <returns>An IEnumerable&lt;Transform&gt; containing all the child Transforms of the parent.</returns>    
        public static IEnumerable<Transform> Children(this Transform parent)
        {
	        foreach (Transform child in parent) 
	        {
		        yield return child;
	        }
        }
        /// <summary>
        /// Deletes all children objects from target transform
        /// </summary>
        /// <param name="t">Transform reference</param>
        /// <returns>World Space Coordinates of rect transform</returns>
        public static void DeleteChildren(this Transform t)
        {
	        foreach(Transform child in t)
	        {
		        Object.Destroy(child.gameObject);
	        }
        }
        
        public static string GetHierarchyPath(this Transform t)
        {
	        var path = t.name;

	        while (t.parent)
	        {
		        t = t.parent;
		        path = $"{t.name}/{path}";
	        }

	        return path;
        }

        #endregion
    }
}
