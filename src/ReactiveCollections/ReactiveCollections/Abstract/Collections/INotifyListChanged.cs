using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using ReactiveCollections.Abstract.Transactions;

namespace ReactiveCollections.Abstract.Collections
{
	public interface INotifyListChanged<out T> : INotifyCollectionChanged<T>
	{
		[NotNull]
		IObservable<IEnumerable<IUpdateListQuery<T>>> ListChanged { get; } 
	}
}
