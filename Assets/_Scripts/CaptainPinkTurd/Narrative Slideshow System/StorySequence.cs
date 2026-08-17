using System.Collections.Generic;
using UnityEngine;

namespace CaptainPinkTurd.NarrativeSlideshow
{
    [CreateAssetMenu(fileName = "Story Sequence", menuName = "Scriptable Objects/Narrative/Sequence")]
    public class StorySequence : ScriptableObject
    {
        [SerializeField] internal List<StoryPanel> panels;
    }
}