using System;
using System.Collections;
using System.Collections.Generic;
using System.Reactive.Subjects;
using JetBrains.Annotations;
using ReactiveCollections.Abstract.Collections;
using ReactiveCollections.Abstract.Transactions;
using ReactiveCollections.Abstract.Transactions.Arguments;
using ReactiveCollections.Extensions;

namespace ReactiveCollections.Implementation.Operations
{
	public abstract class CollectionToCollectionOperationBase<TIn, TOut> : IObservableReadOnlyCollection<TOut>, IDisposable
	{
		[NotNull]
		private readonly Subject<IUpdateCollectionQuery<TOut>> _subject = new Subject<IUpdateCollectionQuery<TOut>>();
		[NotNull]
		private readonly IObservable<IUpdateCollectionQuery<TOut>> _safetyObservable;
		[NotNull]
		private readonly IDisposable _sub;

		protected CollectionToCollectionOperationBase([NotNull] INotifyCollectionChanged<TIn> source)
		{
			_safetyObservable = _subject.ToKeepAliveObservable(this);
			_sub = source.CollectionChanged.WeakSubscribe(ProcessQuery);
		}

		public abstract IEnumerator<TOut> GetEnumerator();

		private void ProcessQuery([NotNull] IUpdateCollectionQuery<TIn> query)
		{
			IEnumerable<IUpdateCollectionQuery<TOut>> queries = query.Match(
				onInsert: OnInsert,
				onRemove: OnRemove,
				onReplace: OnReplace,
				onClear: OnClear,
				onEmpty: OnEmpty);

			foreach (IUpdateCollectionQuery<TOut> updateQuery in queries)
			{
				_subject.OnNext(updateQuery);
			}
		}

		[NotNull, ItemNotNull]
		protected abstract IEnumerable<IUpdateCollectionQuery<TOut>> OnEmpty([NotNull] ICollectionOnEmptyArgs arg);

		[NotNull, ItemNotNull]
		protected abstract IEnumerable<IUpdateCollectionQuery<TOut>> OnClear([NotNull] ICollectionOnClearArgs<TIn> arg);

		[NotNull, ItemNotNull]
		protected abstract IEnumerable<IUpdateCollectionQuery<TOut>> OnReplace([NotNull] ICollectionOnReplaceArgs<TIn> arg);

		[NotNull, ItemNotNull]
		protected abstract IEnumerable<IUpdateCollectionQuery<TOut>> OnRemove([NotNull] ICollectionOnRemoveArgs<TIn> arg);

		[NotNull, ItemNotNull]
		protected abstract IEnumerable<IUpdateCollectionQuery<TOut>> OnInsert([NotNull] ICollectionOnInsertArgs<TIn> arg);

		protected void RaiseCollectionChanged([NotNull] IUpdateCollectionQuery<TOut> arg)
		{
			_subject.OnNext(arg);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public IObservable<IUpdateCollectionQuery<TOut>> CollectionChanged => _safetyObservable;

		public abstract int Count { get; }

		public virtual void Dispose()
		{
			_sub.Dispose();
		}
	}
}