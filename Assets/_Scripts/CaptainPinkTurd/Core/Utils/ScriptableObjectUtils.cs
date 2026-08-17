using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CaptainPinkTurd.Core.Utils
{
    public static class ScriptableObjectUtils
    {
        #if UNITY_EDITOR
        public static T[] GetAllScriptableObjects<T>() where T : ScriptableObject
        {
            //In Unity search syntax, "t:Something" means: “Find all assets of type Something”
            string[] guids = AssetDatabase.FindAssets("t:" + typeof(T).Name);
            T[] assets = new T[guids.Length];

            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                assets[i] = AssetDatabase.LoadAssetAtPath<T>(path);
            }

            return assets;
        }
        #endif
    }
}