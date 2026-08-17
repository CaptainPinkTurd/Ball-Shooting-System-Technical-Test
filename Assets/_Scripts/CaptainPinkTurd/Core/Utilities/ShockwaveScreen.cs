using UnityEngine;
using System.Collections;

namespace CaptainPinkTurd.Core.Utilities
{
    public class ShockwaveScreen : MonoBehaviour
    {
        [SerializeField] private float shockWaveTime = .75f;
    
        private Coroutine shockWaveCoroutine;
        private Material material;
        private static readonly int waveDistanceFromCenter = Shader.PropertyToID("_WaveDistanceFromCenter");
    
        private void Awake()
        {
            material = GetComponent<SpriteRenderer>().material;
        }
        private void OnEnable()
        {
            CallShockWave();
        }

        //You only need to call this via OnEnable using ObjectPool
        private void CallShockWave() 
        {
            if (shockWaveCoroutine != null)
            {
                StopCoroutine(shockWaveCoroutine);
            }
            shockWaveCoroutine = StartCoroutine(ShockWaveAction(-0.1f, 1f));
        }
    
        private IEnumerator ShockWaveAction(float startPos, float endPos)
        {
            material.SetFloat(waveDistanceFromCenter, startPos);
    
            float elapsedTime = 0f;
            while (elapsedTime < shockWaveTime)
            {
                elapsedTime += Time.deltaTime;
                float lerpAmount = Mathf.Lerp(startPos, endPos, elapsedTime / shockWaveTime);
                material.SetFloat(waveDistanceFromCenter, lerpAmount);
                
                yield return null;
            }
            material.SetFloat(waveDistanceFromCenter, startPos);
            
            ObjectPoolManager.Instance.ReturnObjectToPool(gameObject);
        }
    }
}