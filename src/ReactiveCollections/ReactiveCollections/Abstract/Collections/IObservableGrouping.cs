using System.Linq;

namespace ReactiveCollections.Abstract.Collections
{
	public interface IObservableGrouping<out TKey, out TValue> :
		IObservableReadOnlyCollection<TValue>,
		IGrouping<TKey, TValue>
	{
	}
}