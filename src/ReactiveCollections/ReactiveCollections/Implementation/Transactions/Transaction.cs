using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using ReactiveCollections.Abstract.Transactions;

namespace ReactiveCollections.Implementation.Transactions
{
	internal class Transaction<T, TItem> : IDisposable where T : IUpdateCollectionQuery<TItem>
	{
		[NotNull]
		private readonly IObserver<T> _observer;

		[NotNull]
		private readonly Action _callback;

		[NotNull]
		private readonly List<T> _queries = new List<T>();

		public Transaction([NotNull] IObserver<T> observer, [NotNull] Action callback)
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
			foreach (T query in _queries)
			{
				_observer.OnNext(query);
			}
			_callback();
		}
	}
}
