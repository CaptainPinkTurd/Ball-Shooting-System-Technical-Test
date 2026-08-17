using System;
using System.Collections;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CaptainPinkTurd.Core.Utilities
{
    public static class HitStop
    {
        public static bool IsWaiting => running;

        private static bool running;
        private static float remainingTime;
        private static float oldTimeScale;

        // Queue all callbacks
        private static Action pendingCallbacks;

        private class HitStopRunner : MonoBehaviour { }

        private static HitStopRunner runner;

        private static void EnsureRunner()
        {
            if (runner) return;

            var go = new GameObject("[HitStopRunner]");
            runner = go.AddComponent<HitStopRunner>();
            Object.DontDestroyOnLoad(go);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void ResetStaticVariables()
        {
            running = false;
            remainingTime = 0f;
            pendingCallbacks = null;
        }

        public static void Stop(float duration, Action onStopEnd = null)
        {
            EnsureRunner();

            // Always queue callback (even if already running)
            if (onStopEnd != null)
            {
                pendingCallbacks -= onStopEnd; //prevent duplicates
                pendingCallbacks += onStopEnd; 
            }
            
            // Stack duration
            remainingTime = Mathf.Max(remainingTime, duration);

            // If already running, return after extending remainingTime
            if (running) return;
            
            running = true;
            oldTimeScale = Time.timeScale;
            Time.timeScale = 0.0f;

            runner.StartCoroutine(WaitLoop());
        }

        private static IEnumerator WaitLoop()
        {
            do
            {
                while (remainingTime > 0f)
                {
                    remainingTime -= Time.unscaledDeltaTime;
                    yield return null;
                }

                Time.timeScale = oldTimeScale;

                // Snapshot and clear before invoking so a Stop() call made
                // re-entrantly from within a callback (e.g. OnDeath triggered
                // by this callback's damage) queues into a fresh delegate
                // instead of being wiped out by the reset below.
                var callbacksToRun = pendingCallbacks;
                pendingCallbacks = null;

                try
                {
                    callbacksToRun?.Invoke();
                }
                catch (Exception e)
                {
                    Debug.LogError($"HitStop callback error: {e}");
                }

                // If a re-entrant Stop() requested another hit-stop window,
                // re-pause and loop again instead of resetting now.
                if (remainingTime > 0f)
                {
                    Time.timeScale = 0f;
                }
            } while (remainingTime > 0f || pendingCallbacks != null);

            running = false;
        }
    }
}