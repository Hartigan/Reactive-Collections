using System;
using System.Collections;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using JetBrains.Annotations;
using ReactiveCollections.Abstract.Collections;
using ReactiveCollections.Abstract.Transactions;
using ReactiveCollections.Abstract.Transactions.Arguments;
using ReactiveCollections.Extensions;

namespace ReactiveCollections.Implementation.Operations
{
	public abstract class ListToListOperationBase<TIn, TOut> : IObservableReadOnlyList<TOut>, IDisposable
	{
		[NotNull] private readonly Subject<IUpdateListQuery<TOut>> _subject = new Subject<IUpdateListQuery<TOut>>();
		[NotNull] private readonly IObservable<IUpdateListQuery<TOut>> _safetyObservable;
		[NotNull] private IDisposable _sub;

		protected ListToListOperationBase()
		{
			_safetyObservable = _subject.ToKeepAliveObservable(this);
			_sub = Disposable.Empty;
		}

		protected void Subscibe([NotNull] IObservable<IUpdateListQuery<TIn>> source)
		{
			_sub = source.WeakSubscribe(ProcessQuery);
		}

		public abstract IEnumerator<TOut> GetEnumerator();

		private void ProcessQuery([NotNull] IUpdateListQuery<TIn> query)
		{
			IEnumerable<IUpdateListQuery<TOut>> queries = query.Match(
				onInsert: OnInsert,
				onRemove: OnRemove,
				onReplace: OnReplace,
				onMove: OnMove,
				onReset: OnReset,
				onEmpty: OnEmpty);

			foreach (IUpdateListQuery<TOut> updateListQuery in queries)
			{
				_subject.OnNext(updateListQuery);
			}
		}

		[NotNull, ItemNotNull]
		protected abstract IEnumerable<IUpdateListQuery<TOut>> OnEmpty([NotNull] IListOnEmptyArgs arg);

		[NotNull, ItemNotNull]
		protected abstract IEnumerable<IUpdateListQuery<TOut>> OnReset([NotNull] IListOnResetArgs<TIn> arg);

		[NotNull, ItemNotNull]
		protected abstract IEnumerable<IUpdateListQuery<TOut>> OnMove([NotNull] IListOnMoveArgs<TIn> arg);

		[NotNull, ItemNotNull]
		protected abstract IEnumerable<IUpdateListQuery<TOut>> OnReplace([NotNull] IListOnReplaceArgs<TIn> arg);

		[NotNull, ItemNotNull]
		protected abstract IEnumerable<IUpdateListQuery<TOut>> OnRemove([NotNull] IListOnRemoveArgs<TIn> arg);

		[NotNull, ItemNotNull]
		protected abstract IEnumerable<IUpdateListQuery<TOut>> OnInsert([NotNull] IListOnInsertArgs<TIn> arg);

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		protected void RaiseListChanged([NotNull] IUpdateListQuery<TOut> arg)
		{
			_subject.OnNext(arg);
		}

		public IObservable<IUpdateCollectionQuery<TOut>> CollectionChanged => _safetyObservable;

		public IObservable<IUpdateListQuery<TOut>> ListChanged => _safetyObservable;

		public abstract int Count { get; }

		public abstract TOut this[int index] { get; }

		int IObservableReadOnlyList<TOut>.Count => Count;

		public virtual void Dispose()
		{
			_sub.Dispose();
		}
	}
}