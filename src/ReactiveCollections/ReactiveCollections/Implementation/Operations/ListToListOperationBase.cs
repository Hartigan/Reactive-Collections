﻿using System;
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
	public abstract class ListToListOperationBase<TIn, TOut> : IObservableReadOnlyList<TOut>
	{
		[NotNull] private readonly Subject<IUpdateListQuery<TOut>> _subject = new Subject<IUpdateListQuery<TOut>>();
		[NotNull] private readonly IObservable<IUpdateListQuery<TOut>> _safetyObservable;
		[NotNull] private readonly IDisposable _sub;

		protected ListToListOperationBase([NotNull] INotifyListChanged<TIn> source)
		{
			_safetyObservable = _subject.ToKeepAliveObservable(this);
			_sub = source.ListChanged.WeakSubscribe(ProcessQuery);
		}

		public abstract IEnumerator<TOut> GetEnumerator();

		private void ProcessQuery([NotNull] IUpdateListQuery<TIn> query)
		{
			IEnumerable<IUpdateListQuery<TOut>> queries = query.Match(
				onInsert: OnInsert,
				onRemove: OnRemove,
				onReplace: OnReplace,
				onMove: OnMove,
				onClear: OnClear,
				onEmpty: OnEmpty);

			foreach (IUpdateListQuery<TOut> updateListQuery in queries)
			{
				_subject.OnNext(updateListQuery);
			}
		}

		[NotNull, ItemNotNull]
		protected abstract IEnumerable<IUpdateListQuery<TOut>> OnEmpty([NotNull] IListOnEmptyArgs arg);

		[NotNull, ItemNotNull]
		protected abstract IEnumerable<IUpdateListQuery<TOut>> OnClear([NotNull] IListOnClearArgs<TIn> arg);

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

		public IObservable<IUpdateCollectionQuery<TOut>> CollectionChanged => _safetyObservable;

		public IObservable<IUpdateListQuery<TOut>> ListChanged => _safetyObservable;

		public abstract int Count { get; }

		public abstract TOut this[int index] { get; }

		int IObservableReadOnlyList<TOut>.Count => Count;
	}
}