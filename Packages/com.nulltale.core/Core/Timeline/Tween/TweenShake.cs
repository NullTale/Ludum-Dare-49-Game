using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace CoreLib.Timeline
{
    public class TweenShake : Marker, INotification, INotificationOptionProvider
    {
        public Vector3 m_Amplidute;

        public PropertyName id => new PropertyName(nameof(TweenShake));

        public NotificationFlags flags => NotificationFlags.TriggerInEditMode | NotificationFlags.Retroactive;
    }
}