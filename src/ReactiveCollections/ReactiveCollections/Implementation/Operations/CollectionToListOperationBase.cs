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
	public abstract class CollectionToListOperationBase<TIn, TOut> : IObservableReadOnlyList<TOut>, IDisposable
	{
		[NotNull]
		private readonly Subject<IUpdateListQuery<TOut>> _subject = new Subject<IUpdateListQuery<TOut>>();
		[NotNull]
		private readonly IObservable<IUpdateListQuery<TOut>> _safetyObservable;
		[NotNull]
		private readonly IDisposable _sub;

		protected CollectionToListOperationBase([NotNull] INotifyCollectionChanged<TIn> source)
		{
			_safetyObservable = _subject.ToKeepAliveObservable(this);
			_sub = source.CollectionChanged.WeakSubscribe(ProcessQuery);
		}

		public abstract IEnumerator<TOut> GetEnumerator();

		private void ProcessQuery([NotNull] IUpdateCollectionQuery<TIn> query)
		{
			IEnumerable<IUpdateListQuery<TOut>> queries = query.Match(
				onInsert: OnInsert,
				onRemove: OnRemove,
				onReplace: OnReplace,
				onReset: OnClear,
				onEmpty: OnEmpty);

			foreach (IUpdateListQuery<TOut> updateListQuery in queries)
			{
				_subject.OnNext(updateListQuery);
			}
		}

		[NotNull, ItemNotNull]
		protected abstract IEnumerable<IUpdateListQuery<TOut>> OnEmpty([NotNull] ICollectionOnEmptyArgs arg);

		[NotNull, ItemNotNull]
		protected abstract IEnumerable<IUpdateListQuery<TOut>> OnClear([NotNull] ICollectionOnResetArgs<TIn> arg);

		[NotNull, ItemNotNull]
		protected abstract IEnumerable<IUpdateListQuery<TOut>> OnReplace([NotNull] ICollectionOnReplaceArgs<TIn> arg);

		[NotNull, ItemNotNull]
		protected abstract IEnumerable<IUpdateListQuery<TOut>> OnRemove([NotNull] ICollectionOnRemoveArgs<TIn> arg);

		[NotNull, ItemNotNull]
		protected abstract IEnumerable<IUpdateListQuery<TOut>> OnInsert([NotNull] ICollectionOnInsertArgs<TIn> arg);

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public IObservable<IUpdateCollectionQuery<TOut>> CollectionChanged => _safetyObservable;

		public IObservable<IUpdateListQuery<TOut>> ListChanged => _safetyObservable;

		protected void RaiseListChanged([NotNull] IUpdateListQuery<TOut> arg)
		{
			_subject.OnNext(arg);
		}

		public abstract int Count { get; }

		public abstract TOut this[int index] { get; }

		int IObservableReadOnlyList<TOut>.Count => Count;

		public virtual void Dispose()
		{
			_sub.Dispose();
		}
	}
}