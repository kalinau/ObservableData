//using System;
//using System.Collections;
//using System.Collections.Generic;
//using JetBrains.Annotations;
//using ObservableData.Querying.Core;
//using ObservableData.Querying.Utils;

//namespace ObservableData.Querying.Select.Constant
//{
//    public sealed class UpdateForSetAdapter<TIn, TOut> : IUpdate<SetOperation<TOut>>
//    {
//        [NotNull] private readonly IMutableEnumerable<SetOperation<TIn>> _adaptee;
//        [NotNull] private readonly Func<TIn, TOut> _func;

//        public UpdateForSetAdapter([NotNull] IMutableEnumerable<SetOperation<TIn>> adaptee, [NotNull] Func<TIn, TOut> func)
//        {
//            _adaptee = adaptee;
//            _func = func;
//        }

//        public IEnumerable<SetOperation<TOut>> Operations()
//        {
//            foreach (var update in _adaptee)
//            {
//                switch (update.Type)
//                {
//                    case SetOperationType.Add:
//                        yield return SetOperation<TOut>.OnAdd(_func(update.Item));
//                        break;

//                    case SetOperationType.Remove:
//                        yield return SetOperation<TOut>.OnRemove(_func(update.Item));
//                        break;

//                    case SetOperationType.Clear:
//                        yield return SetOperation<TOut>.OnClear();
//                        break;

//                    default:
//                        throw new ArgumentOutOfRangeException();
//                }
//            }
//        }

//        public void Lock() => _adaptee.Lock();
//    }
//}
