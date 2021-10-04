using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace CoreLib.Timeline
{
    public class TweenInvoker : INotificationReceiver
    {
        public static readonly AnimationCurve k_ShakeCurve = new AnimationCurve(new []
        {
            new Keyframe(0f, 0f, 6.341484069824219f, 6.341484069824219f, 0f, 0.1284462809562683f),
            new Keyframe(0.17228232324123383f, 0.4458194971084595f, -8.050700187683106f, -8.050700187683106f, 0.3333333432674408f, 0.3333333432674408f),
            new Keyframe(0.23011745512485505f, -0.6350686550140381f, -1.3906861543655396f, -1.3906861543655396f, 0.6949369311332703f, 0.2897818982601166f),
            new Keyframe(0.4502943456172943f, 0.23750358819961549f, -7.918263912200928f, -7.918263912200928f, 0.3333333432674408f, 0.0317528061568737f),
            new Keyframe(1f, 0f, -2.167879343032837f, -2.167879343032837f, 0.05451901629567146f, 0f)
        });

        public GameObject   m_Target;

        // =======================================================================
        public void OnNotify(Playable origin, INotification notification, object context)
        {
            if (m_Target == null)
                return;

            switch (notification)
            {
                case TweenPunch punch:
                {
                    LeanTween.move(m_Target, m_Target.transform.position + punch.m_Amplidute, 0.6f).setEasePunch();
                } break;
                
                case TweenShake shake:
                {
                    var tween = LeanTween.move(m_Target, m_Target.transform.position + shake.m_Amplidute, 0.6f).setEasePunch();
                    tween._optional.animationCurve = k_ShakeCurve;
                } break;

                case TweenCurve curve:
                {
                    var tween = LeanTween.move(m_Target, m_Target.transform.position + curve.m_Amplidute, curve.m_Curve.Duration).setEasePunch();
                    tween._optional.animationCurve = curve.m_Curve.Curve;
                } break;
                
                case TweenPop pop:
                {
                    LeanTween.scale(m_Target, m_Target.transform.position + pop.m_Amplidute, 0.6f).setEasePunch();
                } break;
            }
        }
    }

    [TrackBindingType(typeof(GameObject))]
    public class TweenTrack : MarkerTrack
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            return ScriptPlayable<ReceiverSetup>.Create(graph, inputCount);
        }
    }

    public class ReceiverSetup : PlayableBehaviour
    {
        // =======================================================================
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            var tweenInvoker = info.output.GetNotificationReceivers().OfType<TweenInvoker>().FirstOrDefault();
            if (tweenInvoker == null)
            {
                tweenInvoker = new TweenInvoker();
                info.output.AddNotificationReceiver(tweenInvoker);
            }

            tweenInvoker.m_Target = (GameObject)playerData;
        }
    }
}