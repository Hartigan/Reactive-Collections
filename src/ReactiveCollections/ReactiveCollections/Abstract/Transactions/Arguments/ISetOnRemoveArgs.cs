namespace ReactiveCollections.Abstract.Transactions.Arguments
{
	public interface ISetOnRemoveArgs<out T>
	{
		T Item { get; }
	}
}
