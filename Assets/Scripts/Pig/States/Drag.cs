using System;
using CoreLib;
using CoreLib.Module;
using UnityEngine;

namespace Game.PigStates
{
    public class Drag : PigStateComponent
    {
        public override PigState Label => PigState.Drag;

        public float    m_ClapmZ;
        public float    m_ClapmX;

        // =======================================================================
        public void Release()
        {
            Owner.SetState(Owner.IsDead ? PigState.Dead : PigState.Socialization);
        }

        public override void OnEnter()
        {
            base.OnEnter();
            Owner.IsKinematic.On();
            Owner.m_Audio.Play(Owner.m_DragAudio);
        }

        public override void OnExit()
        {
            base.OnExit();
            Owner.IsKinematic.Off();
            Owner.m_Audio.Play(Owner.m_ReleaseAudio);
        }

        private void Update()
        {
            var pos = PointerPosition.Instance.GetWordPosition(new Plane(Vector3.up, -GamePreferences.Instance.DragHeight));
            pos.z = pos.z.Clamp(-m_ClapmZ, m_ClapmZ);
            pos.x = pos.x.Clamp(-m_ClapmX, m_ClapmX);
            Owner.transform.position = pos;
        }
    }
}