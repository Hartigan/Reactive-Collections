using System;
using JetBrains.Annotations;
using ReactiveCollections.Abstract.Transactions;

namespace ReactiveCollections.Abstract.Collections
{
	public interface INotifyCollectionChanged<out T>
	{
		[NotNull]
		IObservable<IUpdateCollectionQuery<T>> CollectionChanged { get; }
	}
}
