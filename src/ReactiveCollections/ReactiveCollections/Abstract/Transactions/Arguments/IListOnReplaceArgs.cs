namespace ReactiveCollections.Abstract.Transactions.Arguments
{
	public interface IListOnReplaceArgs<out T> : IUpdateListQuery<T>
	{
		T OldItem { get; }
		T NewItem { get; }
		int Index { get; }
	}
}
