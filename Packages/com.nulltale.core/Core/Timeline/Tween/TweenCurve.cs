using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace CoreLib.Timeline
{
    public class TweenCurve : Marker, INotification, INotificationOptionProvider
    {
        public Vector3              m_Amplidute;
        [Expandable]
        public AnimationCurvePreset m_Curve;

        public PropertyName id => new PropertyName(nameof(TweenPunch));

        public NotificationFlags flags => NotificationFlags.TriggerInEditMode | NotificationFlags.Retroactive;
    }
}