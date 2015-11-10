namespace ReactiveCollections.Abstract.Transactions.Arguments
{
	public interface ICollectionOnInsertArgs<out T>
	{
		T Item { get; }
	}
}
