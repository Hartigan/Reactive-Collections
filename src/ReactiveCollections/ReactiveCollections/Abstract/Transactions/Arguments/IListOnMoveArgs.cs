namespace ReactiveCollections.Abstract.Transactions.Arguments
{
	public interface IListOnMoveArgs<out T>
	{
		T Item { get; }
		int OldIndex { get; }
		int NewIndex { get; }
	}
}
