using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using ObservableData.Querying.Core;

namespace ObservableData.Structures.Updates
{
    internal class BatchUpdate<T> : IDisposable, IUpdate<T>
    {
        [NotNull] private readonly Action<BatchUpdate<T>> _onDispose;
        private IQuickLinkableUpdate<T> _first;
        private IQuickLinkableUpdate<T> _last;

        public BatchUpdate([NotNull] Action<BatchUpdate<T>> onDispose)
        {
            _onDispose = onDispose;
        }

        public void Add(IQuickLinkableUpdate<T> update)
        {
            if (_last == null)
            {
                _first = update;
                _last = update;
            }
            else
            {
                _last.Next = update;
                _last = update;
            }
        }

        public void Clear()
        {
            _last = null;
            _first = null;
        }

        public void Dispose()
        {
            _onDispose(this);
        }

        public void Lock()
        {
            var next = _first;
            while (next != null)
            {
                next.Lock();
                next = next.Next;
            }
        }

        public IEnumerable<T> Operations() => this.Enumerate();

        [NotNull]
        private IEnumerable<T> Enumerate()
        {
            var next = _first;
            while (next != null)
            {
                if (next.IsSingle)
                {
                    yield return next.First;
                }
                else
                {
                    foreach (var update in next.Operations())
                    {
                        yield return update;
                    }
                }
                next = next.Next;
            }
        }
    }
}