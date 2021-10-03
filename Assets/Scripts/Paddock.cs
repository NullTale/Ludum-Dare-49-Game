using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEventBus;

namespace Game
{
    public class Paddock : EventBusBase
    {
        public List<Pig>    m_Pigs;

        // =======================================================================
        private void OnTriggerEnter(Collider other)
        {
            var pig = other.attachedRigidbody?.GetComponent<Pig>();
            if (pig != null && m_Pigs.Contains(pig) == false)
            {
                m_Pigs.Add(pig);
                pig.Paddock = this;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            var pig = other.attachedRigidbody?.GetComponent<Pig>();
            if (pig != null && m_Pigs.Contains(pig))
            {
                m_Pigs.Remove(pig);
                if (pig.Paddock == this)
                    pig.Paddock = null;
            }
        }
    }
}