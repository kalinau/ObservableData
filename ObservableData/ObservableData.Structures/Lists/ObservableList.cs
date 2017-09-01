using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using ObservableData.Querying;
using ObservableData.Querying.Utils;
using ObservableData.Structures.Lists.Utils;
using ObservableData.Structures.Utils;

namespace ObservableData.Structures.Lists
{
    public class ObservableList<T> : IObservableList<T>
    {
        [NotNull] private readonly List<T> _list;
        [NotNull] private readonly ListChangesSubject<T> _subject = new ListChangesSubject<T>();

        public ObservableList()
        {
            _list = new List<T>();
        }

        public ObservableList(IEnumerable<T> items)
        {
            _list = new List<T>(items);
        }

        public int Count => _list.Count;

        IObservable<IBatch<IListOperation<T>>> IObservableReadOnlyList<T>.WhenUpdated => this.WhenUpdated;

        IObservable<ICollectionChange<T>> IObservableReadOnlyCollection<T>.WhenUpdated => this.WhenUpdated;

        [NotNull]
        public IObservable<IListBatchChange<T>> WhenUpdated =>_subject;

        public bool IsReadOnly => false;

        public T this[int index]
        {
            get => _list[index];
            set
            {
                var changedItem = _list[index];
                _list[index] = value;
                _subject.OnReplace(value, changedItem, index);
            }
        }

        public IEnumerator<T> GetEnumerator() => _list.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        public bool Contains(T item) => _list.Contains(item);

        public void CopyTo(T[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);

        public int IndexOf(T item) => _list.IndexOf(item);

        public IDisposable StartBatchUpdate() => _subject.StartBatchUpdate(_list);

        public bool Replace(T oldItem, T newItem)
        {
            var index = this.IndexOf(oldItem);
            if (index == -1) return false;
            _list[index] = newItem;
            _subject.OnReplace(newItem, oldItem, index);
            return true;
        }

        public void Add(T item)
        {
            var index = _list.Count;
            _list.Add(item);
            _subject.OnAdd(item, index);
        }

        public void Clear()
        {
            if (_list.Count == 0) return;
            _list.Clear();
            _subject.OnClear(_list);
        }

        public bool Remove(T item)
        {
            var index = _list.IndexOf(item);

            if (index >= 0)
            {
                this.RemoveAt(index);
                return true;
            }
            return false;
        }

        public void Move(int from, int to)
        {
            Index.Check(from, this.Count);
            Index.Check(to, this.Count);
            if (from == to) return;

            var item = _list[from];
            _list.RemoveAt(from);
            _list.Insert(to, item);
            _subject.OnMove(item, from, to);
        }

        public void Insert(int index, T item)
        {
            Index.Check(index, this.Count + 1);
            _list.Insert(index, item);
            _subject.OnAdd(item, index);
        }

        public void RemoveAt(int index)
        {
            Index.Check(index, _list.Count);
            var item = _list[index];
            _list.RemoveAt(index);
            _subject.OnRemove(item, index);
        }

        public void Add(IReadOnlyCollection<T> items)
        {
            var index = _list.Count;
            _list.AddRange(items);
            _subject.OnAddBatch(items, index);
        }
    }
}
