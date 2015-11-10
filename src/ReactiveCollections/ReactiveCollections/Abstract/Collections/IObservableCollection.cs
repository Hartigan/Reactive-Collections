using System.Collections.Generic;
using ReactiveCollections.Abstract.Transactions;

namespace ReactiveCollections.Abstract.Collections
{
	public interface IObservableCollection<T> : IObservableReadOnlyCollection<T>, ICollection<T>, ITransactional
	{
		new int Count { get; }
	}
}
