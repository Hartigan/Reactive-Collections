namespace ReactiveCollections.Abstract.Transactions.Arguments
{
	public interface IListOnInsertArgs<out T> : IUpdateListQuery<T>, ICollectionOnInsertArgs<T>
	{
		int Index { get; }
	}
}
