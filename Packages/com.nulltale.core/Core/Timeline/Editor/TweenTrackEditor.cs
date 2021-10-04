using UnityEngine;
using UnityEngine.Timeline;

namespace CoreLib.Timeline.Editor
{
    [UnityEditor.Timeline.CustomTimelineEditor(typeof(TweenTrack))]
    public class TweenTrackEditor : UnityEditor.Timeline.TrackEditor
    {
        public override void OnCreate(TrackAsset track, TrackAsset copiedFrom)
        {
            track.CreateCurves("FakeCurves");
            track.curves.SetCurve(string.Empty, typeof(GameObject), "m_FakeCurve", AnimationCurve.Linear(0,1,1,1));
            base.OnCreate(track, copiedFrom);
        }
    }
}