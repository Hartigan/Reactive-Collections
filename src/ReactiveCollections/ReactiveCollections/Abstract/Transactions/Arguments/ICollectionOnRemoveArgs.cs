namespace ReactiveCollections.Abstract.Transactions.Arguments
{
	public interface ICollectionOnRemoveArgs<out T> : IUpdateCollectionQuery<T>
	{
		T Item { get; }
	}
}
