using System;
using System.Collections.Generic;
using ReactiveCollections.Abstract.Transactions;

namespace ReactiveCollections.Abstract.Collections
{
	public interface INotifySetChanged<out T>
	{
		IObservable<IEnumerable<IUpdateSetQuery<T>>> SetChanged { get; }
	}
}
