using ReactiveCollections.Abstract.Transactions;

namespace ReactiveCollections.Abstract.Collections
{
	public interface IObservableSet<T> : IObservableReadOnlySet<T>, ITransactional
	{
		void Add(T item);
		bool Remove(T item);
		void Clear();
	}
}
