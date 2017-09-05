using ObservableData.Querying;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Linq;
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

                    case "all":
                        All();
                        break;

                    case "any":
                        Any();
                        break;

                    case "count":
                        Count();
                        break;

                    case "contains":
                        Contains();
                        break;

                    case "select-constant":
                        SelectConstant();
                        break;

                    case "select":
                        Select();
                        break;

                    case "sum":
                        Sum();
                        break;

                    case "where":
                        Where();
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

        private static void Where()
        {
            var subject = new Subject<ICollectionChange<int>>();
            subject.WhereItems(x => x < 5).Subscribe(new SimpliestObserver<int>());

            Emulate(subject);
        }

        private static void Contains()
        {
            var subject = new Subject<ICollectionChange<int>>();
            subject.ContainsItem(5).Subscribe();
            Emulate(subject);
        }

        private static void Empty()
        {
            var subject = new Subject<ICollectionChange<int>>();
            subject.Subscribe(new SimpliestObserver<int>());

            Emulate(subject);
        }

        private static void Any()
        {
            var subject = new Subject<ICollectionChange<int>>();
            subject.AnyItem(x => x > 5).Subscribe();
            Emulate(subject);
        }

        private static void All()
        {
            var subject = new Subject<ICollectionChange<int>>();
            subject.AllItems(x => x > 5).Subscribe();
            Emulate(subject);
        }

        private static void Sum()
        {
            var subject = new Subject<ICollectionChange<int>>();
            subject.SumItems().Subscribe();
            Emulate(subject);
        }

        private static void Count()
        {
            var subject = new Subject<ICollectionChange<int>>();
            subject.CountItems().Subscribe();
            Emulate(subject);
        }

        private static void Select()
        {
            var subject = new Subject<ICollectionChange<int>>();

            subject.SelectFromItems(x => x * 10).Subscribe(new SimpliestObserver<int>());
            Emulate(subject);
        }

        private static void SelectConstant()
        {
            var subject = new Subject<ICollectionChange<int>>();

            subject.SelectConstantFromItems(x => x * 10).Subscribe(new SimpliestObserver<int>());
            Emulate(subject);
        }

        private static void Emulate([NotNull] Subject<ICollectionChange<int>> subject)
        {
            //var state = new StateChange<int>(new[] {1, 2, 3, 4});

            var add = new [] { GeneralChange<int>.OnAdd(10) };
            var addChange = new DeltaChange<int>(add);

            var remove = new[] { GeneralChange<int>.OnRemove(10) };
            var removeChange = new DeltaChange<int>(remove);
            var clearChange = new DeltaChange<int>(new[] { GeneralChange<int>.OnClear() });
            var batchChange = new DeltaChange<int>(
                new[] {
                    GeneralChange<int>.OnClear(),
                    GeneralChange<int>.OnAdd(1),
                    GeneralChange<int>.OnAdd(2),
                    GeneralChange<int>.OnAdd(3),
                    GeneralChange<int>.OnAdd(4),
                    });

            var d = new Dictionary<int, int>();
            for (int i = 0; i < 1000000; i++)
            {
                add[0] = GeneralChange<int>.OnAdd(i);
                remove[0] = GeneralChange<int>.OnRemove(i);
                //d[i] = i;
                //d.Clear();
                subject.OnNext(addChange);
                subject.OnNext(removeChange);
                //subject.OnNext(clearChange);
                subject.OnNext(batchChange);
            }
        }
    }

    public sealed class SimpliestObserver<T> : 
        IObserver<ICollectionChange<T>>,
        ICollectionChangeEnumerator<T>
    {
        public void OnStateChanged(IReadOnlyCollection<T> state)
        {
            if (state.Count < 0) throw new Exception();
        }

        public void OnClear()
        {
        }

        public void OnAdd(T item)
        {
        }

        public void OnRemove(T item)
        {
        }

        public void OnNext(ICollectionChange<T> value)
        {
            value?.Enumerate(this);
        }

        public void OnError(Exception error) { }

        public void OnCompleted() { }
    }

    public sealed class StateChange<T> : ICollectionChange<T>
    {
        [NotNull] private readonly IReadOnlyList<T> _list;

        public StateChange([NotNull] IReadOnlyList<T> list)
        {
            _list = list;
        }

        public void Enumerate(ICollectionChangeEnumerator<T> enumerator)
        {
            enumerator.OnStateChanged(_list);
        }
    }

    public sealed class DeltaChange<T> : ICollectionChange<T>
    {
        [NotNull] private readonly GeneralChange<T>[] _changes;

        public DeltaChange([NotNull] GeneralChange<T>[] changes)
        {
            _changes = changes;
        }

        public void Enumerate(ICollectionChangeEnumerator<T> enumerator)
        {
            for (int i = 0; i < _changes.Length; i++)
            {
                var change = _changes[i];
                switch (change.Type)
                {
                    case GeneralChangeType.Add:
                        enumerator.OnAdd(change.Item);
                        break;

                    case GeneralChangeType.Remove:
                        enumerator.OnRemove(change.Item);
                        break;

                    case GeneralChangeType.Clear:
                        enumerator.OnClear();
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}
