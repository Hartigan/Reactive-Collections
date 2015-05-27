using System.Collections.Generic;

namespace ReactiveCollections.Abstract.Transactions.Arguments
{
	public interface IListOnClearArgs<out T>
	{
		IReadOnlyList<T> Items { get; } 
	}
}
