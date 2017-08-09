using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using ObservableData.Structures;
using ObservableData.Structures.Utils;

namespace ObservableData.Querying.Utils.Adapters
{
    internal static class UpdateExtensions
    {
        private sealed class ChangeAdapter<T, TAdaptee> : IChange<T>
        {
            [NotNull] private readonly IChange<TAdaptee> _adaptee;
            [NotNull] private readonly Func<IEnumerable<TAdaptee>, IEnumerable<T>> _enumerate;

            public ChangeAdapter(
                [NotNull] IChange<TAdaptee> adaptee,
                [NotNull] Func<IEnumerable<TAdaptee>, IEnumerable<T>> enumerate)
            {
                _adaptee = adaptee;
                _enumerate = enumerate;
            }

            public IEnumerable<T> Operations()
            {
                return _enumerate(_adaptee.Operations()).NotNull();
            }

            public void Lock() => _adaptee.Lock();
        }

        private sealed class ChangeAdapterWithLock<T, TAdaptee> : IChange<T>
        {
            [NotNull] private readonly IChange<TAdaptee> _adaptee;
            [NotNull] private readonly Func<IEnumerable<TAdaptee>, IEnumerable<T>> _enumerate;


            [CanBeNull] private IEnumerable<T> _locked;
            private ThreadId _threadId;

            public ChangeAdapterWithLock(
                [NotNull] IChange<TAdaptee> adaptee,
                [NotNull] Func<IEnumerable<TAdaptee>, IEnumerable<T>> enumerate)
            {
                _adaptee = adaptee;
                _enumerate = enumerate;
                _threadId = ThreadId.FromCurrent();
            }

            public void Lock()
            {
                if (_locked != null) return;
                _threadId.CheckIsCurrent();
                _locked = this.Enumerate().ToArray();
            }

            public IEnumerable<T> Operations()
            {
                if (_locked != null) return _locked;
                _threadId.CheckIsCurrent();
                return this.Enumerate();
            }

            [NotNull]
            private IEnumerable<T> Enumerate()
            {
                return _enumerate(_adaptee.Operations()).NotNull();
            }
        }

        public static IChange<T> Adapt<T, TAdaptee>(
            [NotNull] this IChange<TAdaptee> change,
            [NotNull] Func<IEnumerable<TAdaptee>, IEnumerable<T>> enumerate)
        {
            return new ChangeAdapter<T, TAdaptee>(change, enumerate);
        }

        [NotNull]
        public static IChange<T> AdaptWithLock<T, TAdaptee>(
            [NotNull] this IChange<TAdaptee> change,
            [NotNull] Func<IEnumerable<TAdaptee>, IEnumerable<T>> enumerate)
        {
            return new ChangeAdapterWithLock<T, TAdaptee>(change, enumerate);
        }

        //public static IChange<T> Adapt<T, TAdaptee>(
        //    [NotNull] this IChange<TAdaptee> change,
        //    [NotNull] Func<IEnumerable<TAdaptee>, IEnumerable<T>> enumerate)
        //{
        //    return new ChangeAdapter<T, TAdaptee>(change, enumerate);
        //}
    }
}