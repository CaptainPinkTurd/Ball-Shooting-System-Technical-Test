using UnityEngine;

namespace CaptainPinkTurd.Core.DesignPattern.Singleton
{
    /// <summary>
    /// Persistent Regulator singleton, will destroy any other older components of the same type it finds on awake
    /// </summary>
    public class RegulatorSingleton<T> : MonoBehaviour where T : Component 
    {
        protected static T instance;

        public static bool HasInstance => instance;

        public float InitializationTime { get; private set; }

        public static T Instance 
        {
            get
            {
                if (instance == null)
                {
                    instance = FindAnyObjectByType<T>();
                    if (instance == null)
                    {
                        Debug.LogError($"No {typeof(T).Name} singleton found, Auto-Generating one.");
                        var go = new GameObject(typeof(T).Name + " Auto-Generated");
                        go.hideFlags = HideFlags.HideAndDontSave;
                        instance = go.AddComponent<T>();
                    }
                }

                return instance;
            }
        }
        static RegulatorSingleton()
        {
            SingletonResetRegistry.Register(ResetStaticState);
        }

        // Clears the cached instance (and destroys any leftover DontDestroyOnLoad/auto-generated
        // GameObject) from a previous Play session. See SingletonResetRegistry.
        private static void ResetStaticState()
        {
            if (instance)
            {
                Destroy(instance.gameObject);
            }
            instance = null;
        }

        /// <summary>
        /// Make sure to call base.Awake() in override if you need awake.
        /// </summary>
        protected virtual void Awake()
        {
            InitializeSingleton();
        }

        protected virtual void InitializeSingleton()
        {
            if (!Application.isPlaying) return;
            
            InitializationTime = Time.time;
            DontDestroyOnLoad(gameObject);

            T[] oldInstances = FindObjectsByType<T>(FindObjectsSortMode.None);
            foreach (T old in oldInstances) 
            {
                if (old.GetComponent<RegulatorSingleton<T>>().InitializationTime < InitializationTime) 
                {
                    Destroy(old.gameObject);
                }
            }

            if (instance == null) 
            {
                instance = this as T;
            }
        }
    }
}