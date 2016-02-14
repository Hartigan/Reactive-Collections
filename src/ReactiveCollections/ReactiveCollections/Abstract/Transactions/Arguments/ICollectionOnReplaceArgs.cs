namespace ReactiveCollections.Abstract.Transactions.Arguments
{
	public interface ICollectionOnReplaceArgs<out T> : IUpdateCollectionQuery<T>
	{
		T OldItem { get; }
		T NewItem { get; }
	}
}
