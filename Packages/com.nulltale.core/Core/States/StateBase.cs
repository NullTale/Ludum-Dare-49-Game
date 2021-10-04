using System;
using System.Collections;
using UnityEngine;

namespace CoreLib.States
{
    public abstract class GlobalState : ScriptableObject
    {
        public abstract bool IsOpen { get; }
        public bool IsClosed => !IsOpen;

        public event Action OnOpen;
        public event Action OnClose;

        public event Action OnChanged
        {
            add
            {
                OnOpen  += value;
                OnClose += value;
            }
            remove
            {
                OnOpen  -= value;
                OnClose -= value;
            }
        }

        // =======================================================================
        public abstract void Open();
        public abstract void Close();

        public void Close(float duration)
        {
            if (duration <= 0f)
                return;

            Core.Instance.StartCoroutine(_close(new WaitForSeconds(duration)));

            IEnumerator _close(object interval)
            {
                Close();
                yield return interval;
                Open();
            }
        }

        public void Open(float duration)
        {
            if (duration <= 0f)
                return;

            Core.Instance.StartCoroutine(_open(new WaitForSeconds(duration)));

            IEnumerator _open(object interval)
            {
                Open();
                yield return interval;
                Close();
            }
        }

        internal virtual void Init() { }

        protected internal void InvokeOnOpen()
        {
            OnOpen?.Invoke();
        }
        
        protected internal void InvokeOnClose()
        {
            OnClose?.Invoke();
        }
    }
}