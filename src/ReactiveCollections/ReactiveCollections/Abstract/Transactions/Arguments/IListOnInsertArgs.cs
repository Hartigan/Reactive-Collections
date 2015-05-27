namespace ReactiveCollections.Abstract.Transactions.Arguments
{
	public interface IListOnInsertArgs<out T>
	{
		T Item { get; }
		int Index { get; }
	}
}
