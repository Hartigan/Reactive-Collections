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
	public abstract class CollectionToListOperationBase<T> : IObservableReadOnlyList<T>
	{
		[NotNull]
		private readonly Subject<IUpdateListQuery<T>> _subject = new Subject<IUpdateListQuery<T>>();
		[NotNull]
		private readonly IObservable<IUpdateListQuery<T>> _safetyObservable;
		[NotNull]
		private readonly IDisposable _sub;

		protected CollectionToListOperationBase([NotNull] IObservableReadOnlyCollection<T> source)
		{
			_safetyObservable = _subject.ToKeepAliveObservable(this);
			_sub = source.CollectionChanged.WeakSubscribe(ProcessQuery);
		}

		public abstract IEnumerator<T> GetEnumerator();

		private void ProcessQuery([NotNull] IUpdateCollectionQuery<T> query)
		{
			IEnumerable<IUpdateListQuery<T>> queries = query.Match(
				onInsert: OnInsert,
				onRemove: OnRemove,
				onReplace: OnReplace,
				onClear: OnClear,
				onEmpty: OnEmpty);

			foreach (IUpdateListQuery<T> updateListQuery in queries)
			{
				_subject.OnNext(updateListQuery);
			}
		}

		[NotNull, ItemNotNull]
		protected abstract IEnumerable<IUpdateListQuery<T>> OnEmpty([NotNull] ICollectionOnEmptyArgs obj);

		[NotNull, ItemNotNull]
		protected abstract IEnumerable<IUpdateListQuery<T>> OnClear([NotNull] ICollectionOnClearArgs<T> obj);

		[NotNull, ItemNotNull]
		protected abstract IEnumerable<IUpdateListQuery<T>> OnReplace([NotNull] ICollectionOnReplaceArgs<T> obj);

		[NotNull, ItemNotNull]
		protected abstract IEnumerable<IUpdateListQuery<T>> OnRemove([NotNull] ICollectionOnRemoveArgs<T> obj);

		[NotNull, ItemNotNull]
		protected abstract IEnumerable<IUpdateListQuery<T>> OnInsert([NotNull] ICollectionOnInsertArgs<T> obj);

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public IObservable<IUpdateCollectionQuery<T>> CollectionChanged => _safetyObservable;

		public IObservable<IUpdateListQuery<T>> ListChanged => _safetyObservable;

		protected abstract int Count { get; }

		int IObservableReadOnlyCollection<T>.Count => Count;

		int IReadOnlyCollection<T>.Count => Count;

		public abstract T this[int index] { get; }

		int IObservableReadOnlyList<T>.Count => Count;
	}
}