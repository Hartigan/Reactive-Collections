namespace ReactiveCollections.Abstract.Transactions.Arguments
{
	public interface ISetOnInsertArgs<out T>
	{
		T Item { get; }
	}
}
