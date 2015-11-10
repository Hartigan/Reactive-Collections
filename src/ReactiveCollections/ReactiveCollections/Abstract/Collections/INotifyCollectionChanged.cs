using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using ReactiveCollections.Abstract.Transactions;

namespace ReactiveCollections.Abstract.Collections
{
	public interface INotifyCollectionChanged<out T>
	{
		[NotNull]
		IObservable<IEnumerable<IUpdateCollectionQuery<T>>> CollectionChanged { get; }
	}
}
