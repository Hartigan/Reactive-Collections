using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using ReactiveCollections.Abstract.Transactions;

namespace ReactiveCollections.Abstract.Collections
{
	public interface INotifySetChanged<out T>
	{
		[NotNull]
		IObservable<IEnumerable<IUpdateSetQuery<T>>> SetChanged { get; }
	}
}
