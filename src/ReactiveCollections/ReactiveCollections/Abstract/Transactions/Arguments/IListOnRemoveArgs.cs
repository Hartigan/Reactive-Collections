namespace ReactiveCollections.Abstract.Transactions.Arguments
{
	public interface IListOnRemoveArgs<out T> : IUpdateListQuery<T>
	{
		T Item { get; }
		int Index { get; }
	}
}
