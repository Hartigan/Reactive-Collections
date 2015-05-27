namespace ReactiveCollections.Abstract.Collections
{
	public interface IObservableSet<T> : IObservableReadOnlySet<T>
	{
		void Add(T item);
		bool Remove(T item);
		void Clear();
	}
}
