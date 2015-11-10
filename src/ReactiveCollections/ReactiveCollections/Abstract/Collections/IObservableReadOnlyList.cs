using System.Collections.Generic;

namespace ReactiveCollections.Abstract.Collections
{
	public interface IObservableReadOnlyList<out T> : IObservableReadOnlyCollection<T>, IReadOnlyList<T>, INotifyListChanged<T>
	{
		new int Count { get; }
	}
}
