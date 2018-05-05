using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace svtz.Tanks.Common
{
    internal sealed class DelayedExecutor : ITickable
    {
        public interface IDelayedTask
        {
            float TimeRemaining { get; set; }
            void Cancel();
        }

        private readonly List<Entry> _entries = new List<Entry>();
        private readonly List<Entry> _newEntries = new List<Entry>();

        private sealed class Entry : IDelayedTask
        {
            private readonly Action _action;
            private float _timeout;

            public bool Complete { get; private set; }

            public Entry(Action action, float timeout)
            {
                _action = action;
                _timeout = timeout;
            }

            public void Tick()
            {
                if (Complete)
                    return;

                _timeout -= Time.deltaTime;
                if (_timeout <= 0)
                {
                    Complete = true;
                    _action();
                }
            }

            float IDelayedTask.TimeRemaining
            {
                get { return _timeout; }
                set { _timeout = value; }
            }

            void IDelayedTask.Cancel()
            {
                Complete = true;
            }
        }

        private bool _currentlyTicking = false;

        public IDelayedTask Add(Action action, float executeAfterSeconds)
        {
            var entry = new Entry(action, executeAfterSeconds);

            if (!_currentlyTicking)
            {
                _entries.Add(entry);
            }
            else
            {
                _newEntries.Add(entry);
            }

            return entry;
        }

        void ITickable.Tick()
        {
            _currentlyTicking = true;
            try
            {
                var completeEntries = new List<Entry>();
                foreach (var tickerEntry in _entries)
                {
                    tickerEntry.Tick();
                    if (tickerEntry.Complete)
                        completeEntries.Add(tickerEntry);
                }

                foreach (var completeEntry in completeEntries)
                {
                    _entries.Remove(completeEntry);
                }

            }
            finally
            {
                _entries.AddRange(_newEntries);
                _newEntries.Clear();

                _currentlyTicking = false;
            }
        }
    }
}