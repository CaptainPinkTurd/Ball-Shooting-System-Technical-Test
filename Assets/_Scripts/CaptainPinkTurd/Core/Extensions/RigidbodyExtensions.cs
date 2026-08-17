using UnityEngine;

namespace CaptainPinkTurd.Core.Extensions
{
	public static class RigidbodyExtensions
    {
        public static void ChangeDirection(this Rigidbody rb, Vector3 direction)
        {
            rb.linearVelocity = direction.normalized * rb.linearVelocity.magnitude;
        }

        public static void ChangeDirection(this Rigidbody rb, Vector3 direction, float minMagnitude)
        {
            float magnitude = rb.linearVelocity.magnitude;

            if (magnitude < minMagnitude) magnitude = minMagnitude;

            rb.linearVelocity = direction.normalized * magnitude;
        }

        public static void ChangeDirection(this Rigidbody rb, float maxMagnitude, Vector3 direction)
        {
            float magnitude = rb.linearVelocity.magnitude;

            if (magnitude > maxMagnitude) magnitude = maxMagnitude;

            rb.linearVelocity = direction.normalized * magnitude;
        }

        public static void ChangeDirection(this Rigidbody rb, float maxMagnitude, float minMagnitude, Vector3 direction)
        {
            float magnitude = rb.linearVelocity.magnitude;

            if (magnitude > maxMagnitude) magnitude = maxMagnitude;
            else if (magnitude < minMagnitude) magnitude = minMagnitude;

            rb.linearVelocity = direction.normalized * magnitude;
        }

        public static void NormalizeVelocity(this Rigidbody rb, float magnitude = 1)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * magnitude;
        }
    }
}