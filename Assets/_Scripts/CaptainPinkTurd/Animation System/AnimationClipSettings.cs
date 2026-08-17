#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace CaptainPinkTurd.AnimationSystem
{
    public static class AnimationClipSettings
    {
        [MenuItem("Tools/Enable Loop for Selected AnimationClips")]
        public static void EnableLoopOnSelectedClips() 
        {
            foreach (Object obj in Selection.objects) 
            {
                if (obj is AnimationClip clip) 
                {
                    var settings = AnimationUtility.GetAnimationClipSettings(clip);
                    settings.loopTime = true;
                    AnimationUtility.SetAnimationClipSettings(clip, settings);
                    Debug.Log($"Loop enabled for: {clip.name}");
                }
            }
        }
        [MenuItem("Tools/Disable Loop for Selected AnimationClips")]
        public static void DisableLoopOnSelectedClips() 
        {
            foreach (Object obj in Selection.objects) 
            {
                if (obj is AnimationClip clip)
                {
                    var settings = AnimationUtility.GetAnimationClipSettings(clip);
                    settings.loopTime = false;
                    AnimationUtility.SetAnimationClipSettings(clip, settings);
                    Debug.Log($"Loop disabled for: {clip.name}");
                }
            }
        }
    }
}
#endif
