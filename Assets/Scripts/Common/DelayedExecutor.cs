﻿using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace svtz.Tanks.Common
{
    internal sealed class DelayedExecutor : ITickable
    {
        public interface ICancellable
        {
            void Cancel();
        }

        private readonly List<Entry> _entries = new List<Entry>();

        private sealed class Entry : ICancellable
        {
            private readonly Action _action;
            private readonly float _timeout;
            private float _time;

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

                _time += Time.deltaTime;
                if (_time > _timeout)
                {
                    Complete = true;
                    _action();
                }
            }

            void ICancellable.Cancel()
            {
                Complete = true;
            }
        }

        private bool _currentlyTicking = false;

        public ICancellable Add(Action action, float executeAfterSeconds)
        {
            if (_currentlyTicking)
                throw new InvalidOperationException("Не хотелось, но придётся добавить блокировочек.");

            var entry = new Entry(action, executeAfterSeconds);
            _entries.Add(entry);

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
                _currentlyTicking = false;
            }
        }
    }
}