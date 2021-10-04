using System;

namespace CoreLib
{
    [Serializable]
    public class ToggleGroup
    {
        public ToggleObject m_Current;
        public ToggleObject Current
        {
            get => m_Current;
            set
            {
                if (ReferenceEquals(m_Current, value))
                    return;

                m_Current?._onEnter();
                m_Current = value;
                m_Current?._onEnter();

            }
        }

        public class ToggleObject
        {
            public ToggleGroup Group;
            public bool On
            {
                get => Group.Current == this;
                set
                {
                    // on/off self throw group
                    if (value)
                        Group.Current = this;
                    else
                    if (On)
                        Group.Current = null;
                }
            }

            //////////////////////////////////////////////////////////////////////////
            internal void _onExit() => OnExit();
            internal void _onEnter() => OnEnter();

            public void ToggleOn() => On = true;
            public void ToggleOff() => On = false;

            protected virtual void OnExit()
            {
            }

            protected virtual void OnEnter()
            {
            }
        }

        //////////////////////////////////////////////////////////////////////////
        public ToggleGroup() { }
        public ToggleGroup(params ToggleObject[] objects)
        {
            foreach (var toggleObject in objects)
                Add(toggleObject);
        }

        public void Add(ToggleObject toggleObject)
        {
            toggleObject.Group = this;
        }
    }
}