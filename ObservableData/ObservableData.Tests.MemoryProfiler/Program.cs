using ObservableData.Querying;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Subjects;
using JetBrains.Annotations;

namespace ObservableData.Tests.MemoryProfiler
{

    //the simpliest project to track allocations via dotMemory
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                var line = Console.ReadLine();
                var sw = Stopwatch.StartNew();
                switch (line)
                {
                    case "sum":
                        Sum();
                        break;

                    case "sc":
                        SelectConstant();
                        break;

                    default:
                        Empty();
                        break;
                }
                sw.Stop();
                Console.WriteLine(sw.Elapsed);
                GC.Collect();
                GC.WaitForFullGCComplete();
            }
        }

        private static void Empty()
        {
            var subject = new Subject<ICollectionChange<int>>();
            subject.Subscribe(Use);
            Emulate(subject);
        }

        private static void Sum()
        {
            var subject = new Subject<ICollectionChange<int>>();
            subject.SumItems().Subscribe();
            Emulate(subject);
        }

        private static void SelectConstant()
        {
            var subject = new Subject<ICollectionChange<int>>();
            subject.SelectConstantFromItems(x => x).Subscribe(Use);
            Emulate(subject);
        }

        private static void Emulate([NotNull] Subject<ICollectionChange<int>> subject)
        {
            var state = new StateChange<int>(new[] {1, 2, 3, 4});
            var add = new DeltaChange<int>(new[] { GeneralChange<int>.OnAdd(10) });
            var remove = new DeltaChange<int>(new[] { GeneralChange<int>.OnRemove(10) });
            var clear = new DeltaChange<int>(
                new[] {
                    GeneralChange<int>.OnClear(),
                    GeneralChange<int>.OnAdd(1),
                    GeneralChange<int>.OnAdd(2),
                    GeneralChange<int>.OnAdd(3),
                    GeneralChange<int>.OnAdd(4)});

            for (int i = 0; i < 10000000; i++)
            {
                subject.OnNext(add);
                subject.OnNext(remove);
                subject.OnNext(clear);
            }
        }

        private static void Use(ICollectionChange<int> change)
        {
            if (change == null) throw new ArgumentNullException();

            var state = change.TryGetState();
            if (state != null)
            {
                if (state.Count < 0) throw new Exception();
            }
            else
            {
                var delta = change.TryGetDelta();
                delta?.Enumerate(UseChange);
            }
        }

        [NotNull] private static readonly Func<GeneralChange<int>, bool> UseChange = Use;
        private static bool Use(GeneralChange<int> change)
        {
            return true;
        }
    }

    public sealed class StateChange<T> : ICollectionChange<T>
    {
        private readonly IReadOnlyList<T> _list;

        public StateChange(IReadOnlyList<T> list)
        {
            _list = list;
        }

        public void Match(
            Action<IReadOnlyCollection<T>> onStateChanged, 
            Action<ITrickyEnumerable<GeneralChange<int>>> onDelta)
        {
            onStateChanged?.Invoke(_list);
        }

        public IReadOnlyCollection<T> TryGetState() => _list;

        public ITrickyEnumerable<GeneralChange<T>> TryGetDelta() => null;
    }

    public sealed class DeltaChange<T> : ICollectionChange<T>, ITrickyEnumerable<GeneralChange<T>>
    {
        [NotNull] private readonly GeneralChange<T>[] _changes;

        public DeltaChange([NotNull] GeneralChange<T>[] changes)
        {
            _changes = changes;
        }

        public void Match(
            Action<IReadOnlyCollection<T>> onStateChanged,
            Action<ITrickyEnumerable<GeneralChange<T>>> onDelta)
        {
            onDelta?.Invoke(this);
        }

        public void Enumerate(Func<GeneralChange<T>, bool> handle)
        {
            for (int i = 0; i < _changes.Length; i++)
            {
                handle(_changes[i]);
            }
        }

        public IReadOnlyCollection<T> TryGetState() => null;

        public ITrickyEnumerable<GeneralChange<T>> TryGetDelta() => this;
    }
}
