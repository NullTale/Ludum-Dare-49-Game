using CoreLib.Module;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace CoreLib.Timeline
{
    [TrackColor(0.7568942f, 0.496f, 1f)]
    [TrackClipType(typeof(SpriteAnimationAsset), false)]
    [TrackBindingType(typeof(SpriteRenderer))]
    public class SpriteAnimationTrack : TrackAsset
    {
    }
}