namespace ReactiveCollections.Abstract.Transactions.Arguments
{
	public interface ICollectionOnRemoveArgs<out T>
	{
		T Item { get; }
	}
}
