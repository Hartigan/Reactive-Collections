using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using ReactiveCollections.Abstract.Collections;
using ReactiveCollections.Abstract.Transactions;
using ReactiveCollections.Implementation.Collections;

namespace ReactiveCollections.Implementation.Operations
{
	public sealed class CollectionUnionOperation<T, TKey> : IObservableReadOnlyCollection<T>
	{
		[NotNull]
		private readonly IObservableReadOnlyCollection<T> _result;

		public CollectionUnionOperation(
			[NotNull] IObservableReadOnlyCollection<T> first,
			[NotNull] IObservableReadOnlyCollection<T> second,
			[NotNull] Func<T, TKey> keySelector,
			[NotNull] IEqualityComparer<TKey> keyComparer,
			[NotNull] Func<T, IObservable<T>> keyUpdaterSelector)
		{
			var collection = new ObservableCollection<IObservableReadOnlyCollection<T>>(new[] {first, second});
			_result = collection.SelectManyRc(x => x).DistinctRc(keySelector, keyComparer, keyUpdaterSelector);
		}

		public IObservable<IUpdateCollectionQuery<T>> CollectionChanged => _result.CollectionChanged;

		public IEnumerator<T> GetEnumerator() => _result.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public int Count => _result.Count;
	}
}