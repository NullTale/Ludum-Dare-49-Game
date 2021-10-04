using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoreLib.CommandSystem
{
    public sealed class CommandSequencer : MonoBehaviour
    {
        [SerializeField]
        private bool                 m_AutoRun = true;

        private LinkedList<ICommand> m_Sequence = new LinkedList<ICommand>();
        private Coroutine            m_Coroutine;
        public  bool                 IsRunning => m_Coroutine != null;
        public  bool                 IsRunningCommand { get; private set; }
        public  bool                 IsPaused  { get; set; }

        private IEnumerator              m_Enumerator;
        private LinkedListNode<ICommand> m_Current;

        public bool AutoRun
        {
            get => m_AutoRun;
            set
            {
                if (m_AutoRun == value)
                    return;

                m_AutoRun = value;

                // run if auto run
                if (m_AutoRun && gameObject.activeInHierarchy)
                    Run();
            }
        }

        public IReadOnlyCollection<ICommand> Sequence => m_Sequence;
        public ICommand Current => m_Current?.Value;

        // =======================================================================
        private class CommandEnumerator : ICommand
        {
            private IEnumerator      m_Enumerator;

            // =======================================================================
            public CommandEnumerator(IEnumerator enumerator)
            {
                m_Enumerator = enumerator;
            }

            public IEnumerator Activate()
            {
                return m_Enumerator;
            }
        }

        private class CommandEnumeratorFunc : ICommand
        {
            private Func<IEnumerator> m_EnumeratorFunc;

            // =======================================================================
            public CommandEnumeratorFunc(Func<IEnumerator> enumeratorFunc)
            {
                m_EnumeratorFunc = enumeratorFunc;
            }

            public IEnumerator Activate()
            {
                return m_EnumeratorFunc();
            }
        }

        // =======================================================================
        private void OnEnable()
        {
            if (m_AutoRun)
                Run();
        }

        private void OnDisable()
        {
            Stop();
        }

        public void Push(ICommand command)
        {
            m_Sequence.AddLast(command);
        }
        
        public void Push(YieldInstruction inst)
        {
            Push(new CommandEnumerator(_yield(inst)));

            static IEnumerator _yield(YieldInstruction instruction)
            {
                yield return instruction;
            }
        }
        
        public void Push(IEnumerator enumerator)
        {
            Push(new CommandEnumerator(enumerator));
        }

        public void Push(Func<IEnumerator> enumeratorCall)
        {
            Push(new CommandEnumeratorFunc(enumeratorCall));
        }
        
        public void Push(Action action)
        {
            Push(new CommandAction(action));
        }

        public bool Remove(ICommand command)
        {
            return m_Sequence.Remove(command);
        }

        public void Run()
        {
            if (IsRunning == false)
                m_Coroutine = StartCoroutine(_run());
        }

        public void Stop()
        {
            if (IsRunning)
            {
                StopCoroutine(m_Coroutine);
                IsRunningCommand = false;
                m_Coroutine = null;
                m_Enumerator = null;
                m_Current = null;
            }
        }

        /// <summary> Restart currently executed coroutine </summary>
        public void RestartCurrent()
        {
            m_Enumerator = m_Current?.Value.Activate();
        }

        /// <summary> Clear sequence, stop then run coroutine, or just clear sequence </summary>
        public void Reset()
        {
            if (IsRunningCommand)
            {
                Stop();
                m_Sequence.Clear();
                Run();
            }
            else
            {
                m_Sequence.Clear();
            }
        }

        /// <summary> Clear sequence, currently executed coroutine won't stop </summary>
        public void Clear()
        {
            m_Sequence.Clear();
        }

        // =======================================================================
        private IEnumerator _run()
        {
            while (true)
            {
                // yield first command from list
                if ((m_Current = m_Sequence.First) != null)
                {
                    IsRunningCommand = true;

                    while (IsPaused)
                        yield return null;

                    // start enumerator
                    m_Sequence.RemoveFirst();
                    m_Enumerator = m_Current.Value.Activate();

                    // execute enumerator
                    while (m_Enumerator != null && m_Enumerator.MoveNext())
                    {
                        while (IsPaused)
                            yield return null;

                        yield return m_Enumerator.Current;
                    }
                }
                else
                {
                    // wait command
                    IsRunningCommand = false;
                    yield return null;
                }
            }
        }

    }
}