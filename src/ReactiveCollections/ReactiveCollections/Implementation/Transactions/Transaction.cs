using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using ReactiveCollections.Abstract.Transactions;

namespace ReactiveCollections.Implementation.Transactions
{
	internal class Transaction<T, TItem> : IDisposable where T : IUpdateCollectionQuery<TItem>
	{
		[NotNull]
		private readonly IObserver<IEnumerable<T>> _observer;

		[NotNull]
		private readonly Action _callback;

		[NotNull]
		private readonly List<T> _queries = new List<T>();

		public Transaction([NotNull] IObserver<IEnumerable<T>> observer, [NotNull] Action callback)
		{
			_observer = observer;
			_callback = callback;
		}

		public void AddQuery(T query)
		{
			_queries.Add(query);
		}

		public void Dispose()
		{
			_observer.OnNext(_queries);
			_callback();
		}
	}
}
