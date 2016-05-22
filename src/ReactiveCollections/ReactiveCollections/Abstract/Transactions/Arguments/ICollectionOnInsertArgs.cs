namespace ReactiveCollections.Abstract.Transactions.Arguments
{
	public interface ICollectionOnInsertArgs<out T> : IUpdateCollectionQuery<T>
	{
		T Item { get; }
	}
}
