namespace ReactiveCollections.Abstract.Collections
{
	public interface IObservableLookup<TKey, out TValue> :
		IObservableReadOnlyCollection<IObservableGrouping<TKey, TValue>>
	{
		bool Contains(TKey key);

		IObservableReadOnlyCollection<TValue> this[TKey key] { get; }
	}
}