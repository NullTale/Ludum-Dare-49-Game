using UnityEngine;
using UnityEngine.Playables;


namespace CoreLib.Timeline
{
    [System.Serializable]
    public class MixerParameterAsset : PlayableAsset
    {
        [InplaceField(nameof(MixerParameterBehaviour.Value))]
        public MixerParameterBehaviour   m_Template;

        // =======================================================================
        public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
        {
            var playable = ScriptPlayable<MixerParameterBehaviour>.Create(graph, m_Template);
            return playable;
        }
    }
}


// [TrackColor(1.0f, 0.85f, 0.1f)]