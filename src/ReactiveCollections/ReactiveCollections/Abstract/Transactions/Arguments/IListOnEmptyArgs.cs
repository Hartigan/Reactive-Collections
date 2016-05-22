namespace ReactiveCollections.Abstract.Transactions.Arguments
{
	public interface IListOnEmptyArgs<out T> : ICollectionOnEmptyArgs<T>, IUpdateListQuery<T>
	{
	}
}
