namespace ReactiveCollections.Abstract.Transactions.Arguments
{
	public interface IListOnInsertArgs<out T> : IUpdateListQuery<T>
	{
		T Item { get; }
		int Index { get; }
	}
}
