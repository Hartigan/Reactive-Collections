using System.Collections.Generic;

namespace ReactiveCollections.Abstract.Collections
{
	public interface IObservableReadOnlySet<out T> : IEnumerable<T>, INotifySetChanged<T>
	{
		int Count { get; }
	}
}
