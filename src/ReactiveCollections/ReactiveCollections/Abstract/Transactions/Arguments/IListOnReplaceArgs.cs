namespace ReactiveCollections.Abstract.Transactions.Arguments
{
	public interface IListOnReplaceArgs<out T> : IUpdateListQuery<T>, ICollectionOnReplaceArgs<T>
	{
		int Index { get; }
	}
}
