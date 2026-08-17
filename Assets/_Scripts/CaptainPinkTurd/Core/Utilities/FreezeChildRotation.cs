using System.Collections.Generic;
using UnityEngine;

namespace CaptainPinkTurd.Core.Utilities
{
    public class FreezeChildRotation : MonoBehaviour
    {
        [SerializeField] private List<Transform> childToFreeze;
        
        private readonly List<Quaternion> initialRotations = new List<Quaternion>();

        private void Awake()
        {
            foreach (var child in childToFreeze)
            {
                initialRotations.Add(child.rotation);
            }
        }

        void LateUpdate()
        {
            for (int i = 0; i < childToFreeze.Count; i++)
            {
                childToFreeze[i].rotation = initialRotations[i];
            }
        }
    }
}