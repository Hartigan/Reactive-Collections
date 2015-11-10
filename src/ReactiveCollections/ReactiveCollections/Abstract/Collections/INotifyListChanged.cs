using System;
using JetBrains.Annotations;
using ReactiveCollections.Abstract.Transactions;

namespace ReactiveCollections.Abstract.Collections
{
	public interface INotifyListChanged<out T> : INotifyCollectionChanged<T>
	{
		[NotNull]
		IObservable<IUpdateListQuery<T>> ListChanged { get; } 
	}
}
