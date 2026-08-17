using System.Collections.Generic;
using UnityEngine;

namespace CaptainPinkTurd.Core.SO
{
    public abstract class RuntimeScriptableObject : ScriptableObject
    {
        private static readonly List<RuntimeScriptableObject> instances = new();

        private void OnEnable() => instances.Add(this);
        private void OnDisable() => instances.Remove(this);

        protected abstract void OnReset();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void ResetAllInstance()
        {
            foreach (var instance in instances)
            {
                instance.OnReset();
            }
        }
    }
}