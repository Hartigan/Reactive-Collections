namespace ReactiveCollections.Abstract.Transactions.Arguments
{
	public interface IListOnResetArgs<out T> : IUpdateListQuery<T>, ICollectionOnResetArgs<T>
	{
	}
}
