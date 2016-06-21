using System;
using System.Collections;
using System.Collections.Generic;
using System.Reactive.Linq;
using JetBrains.Annotations;
using ReactiveCollections.Abstract.Collections;
using ReactiveCollections.Abstract.Transactions;

namespace ReactiveCollections.Implementation.Operations
{
	public sealed class CollectionDistinctOperation<T, TKey> : IObservableReadOnlyCollection<T>
	{
		[NotNull]
		private readonly IObservableReadOnlyCollection<T> _result; 

		public CollectionDistinctOperation(
			[NotNull] IObservable<IUpdateCollectionQuery<T>> source,
			[NotNull] Func<T, TKey> keySelector,
			[NotNull] IEqualityComparer<TKey> keyComparer,
			[NotNull] Func<T, IObservable<T>> keyUpdaterSelector)
		{
			var groupByKey = new CollectionGroupByOperation<TKey, T>(source, keySelector, keyComparer, keyUpdaterSelector);
			_result = groupByKey
				.SelectRc(x => x.SomeItemOrDefault(), x => Observable.Never<IObservableGrouping<TKey, T>>())
				.SelectRc(x => x.Value, x => x.ValueChanged.Select(_ => x));
		}

		public IObservable<IUpdateCollectionQuery<T>> CollectionChanged => _result.CollectionChanged;

		public IEnumerator<T> GetEnumerator() => _result.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public int Count => _result.Count;
	}
}