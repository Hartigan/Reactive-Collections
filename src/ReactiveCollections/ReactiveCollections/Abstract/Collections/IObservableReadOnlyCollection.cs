using System.Collections.Generic;

namespace ReactiveCollections.Abstract.Collections
{
	public interface IObservableReadOnlyCollection<out T> : IEnumerable<T>, INotifyCollectionChanged<T>
	{
		int Count { get; }
	}
}
