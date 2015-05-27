using System;
using System.Collections.Generic;
using ReactiveCollections.Abstract.Transactions;

namespace ReactiveCollections.Abstract.Collections
{
	public interface INotifyListChanged<out T> : INotifySetChanged<T>
	{
		IObservable<IEnumerable<IUpdateListQuery<T>>> ListChanged { get; } 
	}
}
