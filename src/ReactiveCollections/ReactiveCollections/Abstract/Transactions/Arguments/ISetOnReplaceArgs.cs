namespace ReactiveCollections.Abstract.Transactions.Arguments
{
	public interface ISetOnReplaceArgs<out T>
	{
		T OldItem { get; }
		T NewItem { get; }
	}
}
