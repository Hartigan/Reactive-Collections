namespace ReactiveCollections.Abstract.Transactions.Arguments
{
	public interface IListOnRemoveArgs<out T> : IUpdateListQuery<T>, ICollectionOnRemoveArgs<T>
	{
		int Index { get; }
	}
}
