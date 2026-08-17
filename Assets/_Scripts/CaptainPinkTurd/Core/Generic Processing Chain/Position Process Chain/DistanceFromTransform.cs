using UnityEngine;

namespace CaptainPinkTurd.Core.ProcessingChain
{
    public class DistanceFromTransform : IProcessor<Vector3, float>
    {
        private readonly Transform transform;
        
        public DistanceFromTransform(Transform transform)
        {
            this.transform = transform;
        }
        public float Process(Vector3 point)
        {
            return Vector3.Distance(transform.position, point);
        }
    }
}