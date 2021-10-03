using UnityEventBus;

namespace Game.PigStates
{
    public class Dead : PigStateComponent
    {
        public override PigState Label => PigState.Dead;

        // =======================================================================
        public override void OnEnter()
        {
            base.OnEnter();
            if (Owner.IsDead == false)
            {
                Owner.IsDraggabel.On();
                Owner.IsDead = true;
                Owner.m_Explotion.Play();
            }
        }
    }
}