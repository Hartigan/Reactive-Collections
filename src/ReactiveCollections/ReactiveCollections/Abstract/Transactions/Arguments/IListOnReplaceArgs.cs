namespace ReactiveCollections.Abstract.Transactions.Arguments
{
	public interface IListOnReplaceArgs<out T>
	{
		T OldItem { get; }
		T NewItem { get; }
		int Index { get; }
	}
}
