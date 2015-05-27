using System.Collections.Generic;

namespace ReactiveCollections.Abstract.Transactions.Arguments
{
	public interface ISetOnClearArgs<out T>
	{
		IReadOnlyList<T> Items { get; }
	}
}
