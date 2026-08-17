using System;
using System.Collections.Generic;
using CaptainPinkTurd.Core.Attributes;
using UnityEngine;

namespace CaptainPinkTurd.EffectSystem.ImpactEffect
{
    [CreateAssetMenu(fileName = "Surface", menuName = "Scriptable Objects/Impact System/Surface")]
    public class Surface : ScriptableObject
    {
        [Serializable]
        public class SurfaceImpactTypeEffect
        {
            public ImpactType impactType;
            [InlineScriptableObject] public SurfaceEffect surfaceEffect;
        }
        public List<SurfaceImpactTypeEffect> impactTypeEffects = new List<SurfaceImpactTypeEffect>();
    }
}