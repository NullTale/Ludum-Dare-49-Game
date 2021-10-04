using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace CoreLib.Timeline
{
    public class TweenPunch : Marker, INotification, INotificationOptionProvider
    {
        public Vector3    m_Amplidute;

        public PropertyName id => new PropertyName(nameof(TweenPunch));

        public NotificationFlags flags => NotificationFlags.TriggerInEditMode | NotificationFlags.Retroactive;
    }
}