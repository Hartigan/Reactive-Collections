using System.Collections.Generic;

namespace ReactiveCollections.Abstract.Collections
{
	public interface IObservableReadOnlyCollection<out T> : INotifyCollectionChanged<T>, IReadOnlyCollection<T>
	{
	}
}
