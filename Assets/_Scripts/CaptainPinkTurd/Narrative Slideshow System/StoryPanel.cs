using System;
using CaptainPinkTurd.Core.Attributes;
using TMPro;
using UnityEngine;

namespace CaptainPinkTurd.NarrativeSlideshow
{
    [Serializable]
    public class StoryPanel
    {
        [Header("Panel Configs")]
        [SerializeField] internal Sprite image;
        
        [SerializeField] internal bool slidingImage;
        [ShowIf(nameof(slidingImage))]
        [SerializeField] internal Vector3 slideFromLocalPos;
        [ShowIf(nameof(slidingImage))]
        [SerializeField] internal Vector3 slideToLocalPos;
        [ShowIf(nameof(slidingImage))]
        [SerializeField] internal float slideDuration = 5f;
        
        [SerializeField] internal float fadeInTime = 1f;
        [SerializeField] internal float fadeOutTime = 1f;
        
        [Header("Text Configs")]
        [SerializeField][TextArea(3, 10)] internal string text;
        [SerializeField] internal bool disableTextOnFadeOut;
        [SerializeField] internal TextAlignmentOptions alignment = TextAlignmentOptions.TopLeft;
    }
}